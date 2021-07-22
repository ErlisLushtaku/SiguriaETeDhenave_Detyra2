using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.Net;
using DataSecurity_pr2;
using System.Threading;
using DataSecurity_pr2.Repositories;
using DataSecurity_pr2.Models;
using System.Text.RegularExpressions;
using JWT.Builder;

namespace Siguri_Projekti2
{
    class ServerSide
    {
       public static X509Certificate2 certifikata = new X509Certificate2("../../DS.pfx", "123456");
        private static string secret = "enesh";
        public static string getPrivateKey()
        {
            string privKeyFileToBeRead = File.ReadAllText("../../private-key.pem");
            string privKey = Regex.Replace(privKeyFileToBeRead, "-----BEGIN ENCRYPTED PRIVATE KEY-----|-----END ENCRYPTED PRIVATE KEY-----|\\r|\\n", "");
            return privKey;
        }
        private DESCryptoServiceProvider des;
        private readonly RSACryptoServiceProvider rsa;
        private byte[] desKey;
        public string Encrypt(string response)
        {
            byte[] byteResponse = Encoding.UTF8.GetBytes(response);
            des = new DESCryptoServiceProvider();
              des.Key = desKey;
           // des.GenerateKey();
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.Zeros;
            des.GenerateIV(); //gjenerimi i IV`
            byte[] IV = des.IV;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(byteResponse, 0, byteResponse.Length);
            cs.Close();
            byte[] encryptedResponse = ms.ToArray();
            byte[] concatenatedResponse = IV.Concat(encryptedResponse).ToArray();
            return Convert.ToBase64String(concatenatedResponse);

        }
        public string Decrypt(string clientMessage)
        {
            byte[] fullMsgData = Convert.FromBase64String(clientMessage);
 
            byte[] IV = new byte[8];
            byte[] enDesKey = new byte[128];
            byte[] enMessage = new byte[fullMsgData.Length - IV.Length - enDesKey.Length];
            Array.Copy(fullMsgData, IV, 8);
            Array.Copy(fullMsgData, IV.Length, enDesKey, 0, 128);
            Array.Copy(fullMsgData, IV.Length + enDesKey.Length, enMessage, 0, fullMsgData.Length - IV.Length - enDesKey.Length);                       
             desKey = rsa.Decrypt(enDesKey, false);
            DES des = DES.Create();
            des.IV = IV;
            des.Key = desKey;
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.Zeros;

            MemoryStream memoryStream = new MemoryStream(enMessage);
            byte[] decryptedMessage = new byte[memoryStream.Length];
            CryptoStream cryptoStream = new CryptoStream(memoryStream, des.CreateDecryptor(), CryptoStreamMode.Read);
            cryptoStream.Read(enMessage, 0, enMessage.Length);
            cryptoStream.Close();
            string decryptedData = Encoding.UTF8.GetString(decryptedMessage);
            return decryptedData;
            // login-...
        }

        public void createResponseToUser(UdpClient user,string data)
        {           
            string plainData = Decrypt(data);
            string logOrRegOrBill = plainData.Split('-')[0];
            string command = plainData.Split('-')[1];

            //komanda per login=emaili,pw
            //per register=emri,mbi,imella,id,pw
            //per fatura=id,viti,muji,...,...
            //login-command
            switch (logOrRegOrBill)
            {
                case "login":
                    string emaili = command.Split('>')[0];
                    if (UserRepository.findUser(emaili) == null)
                    {
                        string encryptedResponse = Encrypt("ERROR");
                        user.Send(Encoding.UTF8.GetBytes(encryptedResponse), Encoding.UTF8.GetBytes(encryptedResponse).Length);
                    }
                    else
                    {
                        user.Send(Encoding.UTF8.GetBytes(Encrypt(JWTSignature(emaili))), Encoding.UTF8.GetBytes(Encrypt(JWTSignature(emaili))).Length);
                    }
                    break;

                case "register":
                    string userEmail = command.Split('>')[2];
                    if (UserRepository.findUser(userEmail) == null)
                    {
                        string name = command.Split('>')[0];
                        string surname = command.Split('>')[1];
                        string email = command.Split('>')[2];
                        int id = Convert.ToInt32(command.Split('>')[3]);
                        string password = command.Split('>')[4];
                        string salt = command.Split('>')[5];
                        User useri = new User(name, surname, email, id, password, salt);
                        if (UserRepository.createUser(useri))
                        {
                            user.Send(Encoding.UTF8.GetBytes(Encrypt("OK")), Encoding.UTF8.GetBytes(Encrypt("OK")).Length);
                        }
                        else
                        {
                            byte[] a = new byte[2];
                            user.Send(Encoding.UTF8.GetBytes(Encrypt("ERROR")), Encoding.UTF8.GetBytes(Encrypt("ERROR")).Length);
                            
                        }
                    }
                    else
                    {

                        user.Send(Encoding.UTF8.GetBytes(Encrypt("ERROR")), Encoding.UTF8.GetBytes(Encrypt("ERROR")).Length);
                    }
                    break;

                case "registerbill":
                    string type = command.Split('>')[0];
                    int year = Convert.ToInt32(command.Split('>')[1]);
                    string month = command.Split('>')[2];
                    double value = Convert.ToDouble(command.Split('>')[3]);
                    int userId = Convert.ToInt32(command.Split('>')[4]);
                    Bill bill = new Bill(type, year, month, value, userId);
                    BillRepository.addBill(bill);
                    user.Send(Encoding.UTF8.GetBytes(Encrypt("OK")), Encoding.UTF8.GetBytes(Encrypt("OK")).Length);
                    break;
                default:
                    break;
            }

        }

        public static string createJwtToken(string email)
        {
            User useri = UserRepository.findUser(email);
            IJwtAlgorithm alg = new RS256Algorithm(certifikata);

            var token = JwtBuilder.Create()
                                   .WithAlgorithm(alg)      
                                   .AddClaim("UserId", useri.getId())
                                   .AddClaim("Name", useri.getName())
                                   .AddClaim("Surname", useri.getSurname())
                                   .AddClaim("Email", useri.getEmail())
                                   .AddClaim("Password", useri.getPassword())
                                   .AddClaim("Salt", useri.getSalt())
                                   .Encode();
            return token;
        }
        public  ServerSide()
        {
            rsa = (RSACryptoServiceProvider)certifikata.PrivateKey;
            serverThread();
        }
        private void ServerMultiThread()
        {
            Thread thdUDPServer = new Thread(new
            ThreadStart(serverThread));
            thdUDPServer.Start();
        }
        public void serverThread()
        {
            UdpClient udpClient = new UdpClient(8080);
            while (true)
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.UTF8.GetString(receiveBytes);                
                createResponseToUser(udpClient,returnData);           
            }
        }

    }
}
