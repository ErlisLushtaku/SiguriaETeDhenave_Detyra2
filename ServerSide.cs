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
using System;
namespace Siguri_Projekti2
{
    class ServerSide
    {
        public static X509Certificate2 certifikata;
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
        public string Decrypt(byte[] fullMsgData)
        {
 
 
            byte[] IV = new byte[8];
            byte[] enDesKey = new byte[256];
            byte[] enMessage = new byte[fullMsgData.Length - IV.Length - enDesKey.Length];
            Array.Copy(fullMsgData, IV, 8);
            Array.Copy(fullMsgData, IV.Length, enDesKey, 0, 256);
            Array.Copy(fullMsgData, IV.Length + enDesKey.Length, enMessage, 0, fullMsgData.Length - IV.Length - enDesKey.Length);
            DES des = DES.Create();
            des.IV = IV;
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.Zeros;
                       
            desKey = rsa.Decrypt(enDesKey, true);            
            des.Key = desKey;
            
            MemoryStream memoryStream = new MemoryStream(enMessage);
            byte[] decryptedMessage = new byte[memoryStream.Length];
            CryptoStream cryptoStream = new CryptoStream(memoryStream, des.CreateDecryptor(), CryptoStreamMode.Read);
            cryptoStream.Read(decryptedMessage, 0, decryptedMessage.Length);
            cryptoStream.FlushFinalBlock();
            cryptoStream.Close();
            string decryptedData = Encoding.UTF8.GetString(decryptedMessage);
            return decryptedData;
            // login-...
        }

        public void createResponseToUser(UdpClient user, byte[] fullMsgData, IPEndPoint RemoteIpEndPoint)
        {           
            string plainData = Decrypt(fullMsgData);
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
                    string pw = command.Split('>')[1];
                    if (UserRepository.findUser(emaili) == null)
                    {
                        string encryptedResponse = Encrypt("ERROR");
                        user.Send(Convert.FromBase64String(encryptedResponse), Convert.FromBase64String(encryptedResponse).Length, RemoteIpEndPoint);
                    }
                    else
                    {
                        User useri = UserRepository.findUser(emaili);
                        
                        if (useri.getPassword()==pw)
                        {
                            user.Send(Convert.FromBase64String(Encrypt(createJwtToken(emaili))), Convert.FromBase64String(Encrypt(createJwtToken(emaili))).Length, RemoteIpEndPoint);
                        }
                        else {
                            user.Send(Convert.FromBase64String(Encrypt("ERROR")), Convert.FromBase64String(Encrypt("ERROR")).Length, RemoteIpEndPoint);
                        }
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
                            user.Send(Convert.FromBase64String(Encrypt("OK")), Convert.FromBase64String(Encrypt("OK")).Length, RemoteIpEndPoint);
                            
                        }
                        else
                        {
                            byte[] a = new byte[2];
                            user.Send(Convert.FromBase64String(Encrypt("ERROR")), Convert.FromBase64String(Encrypt("ERROR")).Length, RemoteIpEndPoint);
                            
                        }
                    }
                    else
                    {

                        user.Send(Convert.FromBase64String(Encrypt("ERROR")), Convert.FromBase64String(Encrypt("ERROR")).Length, RemoteIpEndPoint);
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
                    user.Send(Convert.FromBase64String(Encrypt("OK")), Convert.FromBase64String(Encrypt("OK")).Length, RemoteIpEndPoint);
                    break;
                default:
                    break;
            }
        }
        
        private string computeHash(string saltedpassword) {
            byte[] byteSaltedPassword = Encoding.UTF8.GetBytes(saltedpassword);
            SHA1CryptoServiceProvider obj = new SHA1CryptoServiceProvider();
            byte[] saltedHashPassword = obj.ComputeHash(byteSaltedPassword);
            return Convert.ToBase64String(saltedHashPassword);
        }

        public static string createJwtToken(string email)
        {
            User useri = UserRepository.findUser(email);
            IJwtAlgorithm alg = new RS256Algorithm(certifikata);
            var token = JwtBuilder.Create()
                                   .WithAlgorithm(alg)
                                   .AddClaim("name", useri.getName())
                                   .AddClaim("surname", useri.getSurname())
                                   .AddClaim("email", useri.getEmail())
                                   .AddClaim("id", useri.getId())
                                   .AddClaim("password", useri.getPassword())
                                   .AddClaim("salt", useri.getSalt())
                                   .Encode();
            return token;
        }
        //awgasgweasfaw
        public  ServerSide()
        {
            certifikata = new X509Certificate2("../../SFC.pfx", "123456");
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
                byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint); 
                createResponseToUser(udpClient, receiveBytes, RemoteIpEndPoint);           
            }
        }

    }
}
