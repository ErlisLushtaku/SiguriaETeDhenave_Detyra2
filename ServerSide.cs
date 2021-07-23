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
        public static X509Certificate2 certifikata = new X509Certificate2("../../SFC.pfx", "123456");
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
            Array.Copy(fullMsgData, IV.Length + enDesKey.Length, enMessage, 0, enMessage.Length);
            desKey = rsa.Decrypt(enDesKey, true);
            DES des = DES.Create();
            des.IV = IV;
            des.Key = desKey;
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.Zeros;

            MemoryStream memoryStream = new MemoryStream(enMessage);
            byte[] decryptedMessage = new byte[memoryStream.Length];
            CryptoStream cryptoStream = new CryptoStream(memoryStream, des.CreateDecryptor(), CryptoStreamMode.Read);
            cryptoStream.Read(decryptedMessage, 0, decryptedMessage.Length);
            cryptoStream.Close();
            string decryptedData = Encoding.UTF8.GetString(decryptedMessage);
            return decryptedData;
            // login-...
        }

        public void createResponseToUser(UdpClient user, byte[] fullMsgData, IPEndPoint RemoteIpEndPoint)
        {
            string plainData = Decrypt(fullMsgData);
            plainData = Regex.Replace(plainData, @"[\0]+", "");
            string logOrRegOrBill = plainData.Split('*')[0];
            string command = plainData.Split('*')[1];
            command = Regex.Replace(command, @"[\0]+", "");
            //komanda per login=emaili,pw
            //per register=emri,mbi,imella,id,pw
            //per fatura=id,viti,muji,...,...
            //login-command
            switch (logOrRegOrBill)
            {
                case "login":
                    string emaili = command.Split('>')[0];
                    string passwordi = command.Substring(emaili.Length+1);
                    User usr = UserRepository.findUser(emaili);
                    if (usr == null)
                    {
                        string encryptedResponse = Encrypt("ERROR");
                        user.Send(Convert.FromBase64String(encryptedResponse), Convert.FromBase64String(encryptedResponse).Length, RemoteIpEndPoint);
                    }
                    else
                    {
                        if (usr.getPassword() != computeHash(passwordi + usr.getSalt()))
                        {
                            user.Send(Convert.FromBase64String(Encrypt("invalidCredentials")), Convert.FromBase64String(Encrypt("invalidCredentials")).Length, RemoteIpEndPoint);
                        }
                        else
                        {
                            user.Send(Convert.FromBase64String(Encrypt(createJwtToken(usr))), Convert.FromBase64String(Encrypt(createJwtToken(usr))).Length, RemoteIpEndPoint);
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
                        string salt = command.Split('>')[4];
                        string password = command.Substring(name.Length + surname.Length + email.Length + command.Split('>')[3].Length + salt.Length + 5);
                        User useri = new User(name, surname, email, id, password, salt);
                        if (UserRepository.createUser(useri))
                        {
                            user.Send(Convert.FromBase64String(Encrypt("OK")), Convert.FromBase64String(Encrypt("OK")).Length, RemoteIpEndPoint);
                        }
                        else
                        {
                          
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

        public static string createJwtToken(User useri)
        {
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
        public ServerSide()
        {
            rsa = (RSACryptoServiceProvider)certifikata.PrivateKey;
            ServerMultiThread();
        }
        public static string computeHash(string saltedpassword)
        {
            byte[] byteSaltedPassword = Encoding.UTF8.GetBytes(saltedpassword);
            SHA1CryptoServiceProvider obj = new SHA1CryptoServiceProvider();
            byte[] saltedHashPassword = obj.ComputeHash(byteSaltedPassword);
            return Convert.ToBase64String(saltedHashPassword);
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
