using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.Net;
using DataSecurity_pr2;
using System.Threading;
using DataSecurity_pr2.Repositories;
using DataSecurity_pr2.Models;
using JWT.Builder;

namespace Siguri_Projekti2
{
    public class ClientSide
    {
        public static X509Certificate2 certifikata = new X509Certificate2("../../SFC.pfx", "123456");

        private DESCryptoServiceProvider des;
        private RSACryptoServiceProvider rsa;
        static byte[] DesKey;
        static byte[] initialVector;

        public UdpClient udpClient;

        public ClientSide()
        {
            try
            {
             

                udpClient = new UdpClient();
                udpClient.Connect("localhost", 8080);

                des = new DESCryptoServiceProvider();
                rsa = (RSACryptoServiceProvider)certifikata.PublicKey.Key;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void requestToServer(string request)
        {

            des.GenerateKey();
            DesKey = des.Key;
            des.GenerateIV();
            initialVector = des.IV;
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.Zeros;

            byte[] bytePlainMsg = Encoding.UTF8.GetBytes(request);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.FlushFinalBlock();
            cs.Write(bytePlainMsg, 0, bytePlainMsg.Length);
            cs.Close();
            byte[] byteCipherMsg = ms.ToArray();
            byte[] byteCipherDesKey = rsa.Encrypt(DesKey, true);
            //byte[] fullMessage = initialVector.Concat(byteCipherDesKey).Concat(byteCipherMsg).ToArray();
            string sendData = Convert.ToBase64String(initialVector.Concat(byteCipherDesKey).Concat(byteCipherMsg).ToArray());

            udpClient.Send(Convert.FromBase64String(sendData), Convert.FromBase64String(sendData).Length);
            //user.Shutdown(SocketShutdown.Both);
            //user.Close();
        }

        

        public string responseFromServer()
        {

            IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] byteResponse = udpClient.Receive(ref remoteIPEndPoint);

            byte[] IV = new byte[8];
            Array.Copy(byteResponse, IV, 8);
            byte[] enMessage = new byte[byteResponse.Length - 8];
            Array.Copy(byteResponse, 8, enMessage, 0, enMessage.Length);

            DES des = DES.Create();
            des.IV = IV;
            des.Key = DesKey;
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

        public static string getJwtPayload(string token)
        {
            IJwtAlgorithm alg = new RS256Algorithm(certifikata);
            try
            {
                var payload = JwtBuilder.Create()
                         .WithAlgorithm(alg)
                         .MustVerifySignature()
                         .Decode(token);
                return payload;
            }
            catch (Exception ex)
            {
                return "invalidSignature";
            }
        }
        public static string computeHash(string saltedpassword)
        {
            byte[] byteSaltedPassword = Encoding.UTF8.GetBytes(saltedpassword);
            SHA1CryptoServiceProvider obj = new SHA1CryptoServiceProvider();
            byte[] saltedHashPassword = obj.ComputeHash(byteSaltedPassword);
            return Convert.ToBase64String(saltedHashPassword);
        }
    }
}

