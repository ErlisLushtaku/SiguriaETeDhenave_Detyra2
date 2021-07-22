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

namespace Siguri_Projekti2
{
    class ClientSide
    {
        public static X509Certificate2 certifikata = new X509Certificate2("../../Siguri_Projekti2.cer", "123456");

        private const String secret = "enesh";
        private DESCryptoServiceProvider des;
        private RSACryptoServiceProvider rsa;
        static byte[] DesKey;
        static byte[] initialVector;

        public UdpClient udpClient;

        private Socket server;


        public ClientSide()
        {
            try {
                //IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                //IPAddress ipAddr = ipHost.AddressList[0];
                //IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
                //server = new Socket(ipAddr.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                //server.Bind(localEndPoint);
                // rsa = (RSACryptoServiceProvider)certifikata.PrivateKey;

                udpClient = new UdpClient();
                udpClient.Connect("localhost", 8080);

                des = new DESCryptoServiceProvider();
                rsa = (RSACryptoServiceProvider)certifikata.PublicKey.Key;

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
        public void requestToServer(String request) {
            
            des.GenerateKey();
            DesKey = des.Key;
            des.GenerateIV();
            initialVector = des.IV;
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.Zeros;

            byte[] bytePlainMsg = Encoding.UTF8.GetBytes(request);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(bytePlainMsg, 0, bytePlainMsg.Length);
            cs.Close();

            byte[] byteCipherMsg = ms.ToArray();
            byte[] byteCipherDesKey = rsa.Encrypt(DesKey, true);

            string sendData = Convert.ToBase64String(initialVector.Concat(byteCipherDesKey).Concat(byteCipherMsg).ToArray());

            udpClient.Send(Encoding.ASCII.GetBytes(sendData), sendData.Length);
            //user.Shutdown(SocketShutdown.Both);
            //user.Close();
        }    
            
        //public string Encrypt(string response)
        //{
        //    byte[] byteResponse = Encoding.UTF8.GetBytes(response);
        //    des = new DESCryptoServiceProvider();
        //    //  des.Key = DesKey;
        //    des.GenerateKey();
        //    des.Mode = CipherMode.CBC;
        //    des.Padding = PaddingMode.Zeros;
        //    des.GenerateIV(); //gjenerimi i IV`
        //    byte[] IV = des.IV;
        //    MemoryStream ms = new MemoryStream();
        //    CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
        //    cs.Write(byteResponse, 0, byteResponse.Length);
        //    cs.Close();
        //    byte[] encryptedResponse = ms.ToArray();
        //    byte[] concatenatedResponse = IV.Concat(encryptedResponse).ToArray();
        //    return Convert.ToBase64String(concatenatedResponse);

        //}
        public string responseFromServer(string response)
        {
            string[] explodedMessages = response.Split(',');
            byte[] byteResponse = Convert.FromBase64String(response);
            byte[] IV = new byte[8];
            Array.Copy(byteResponse, IV, 8);
            byte[] enMessage = new byte[byteResponse.Length-8];
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


        //public static string JWTSignature(string email)
        //{
        //    User useri = UserRepository.findUser(email);

        //    IJwtAlgorithm alg = new RS256Algorithm(certifikata);
        //    IJsonSerializer serializer = new JsonNetSerializer();
        //    IBase64UrlEncoder base64 = new JwtBase64UrlEncoder();
        //    //IJwtValidator jwt = new JwtValidator(serializer,new UtcDateTimeProvider());
        //    IJwtEncoder ije = new JwtEncoder(alg, serializer, base64);
        //    var payload = new Dictionary<string, object>
        //    {
        //        {"Userid",useri.getId()},
        //        {"Name",useri.getName()},
        //        {"Surname",useri.getSurname()},
        //        {"Email",useri.getEmail()},
        //        {"Password",useri.getPassword()},
        //        {"Salt",useri.getSalt()}
        //    };

        //    var token = ije.Encode(payload, secret);
        //    string signedMessage = token;

        //    return signedMessage;
        //}
        //public void listen()
        //{
        //    server.Listen(10);

        //    while (true)
        //    {

        //        Socket client = server.Accept();
        //        Thread thread = new Thread(() => this.sendResponseToUser(client));
        //        thread.Start();


        //    }
        //}
    }
}
