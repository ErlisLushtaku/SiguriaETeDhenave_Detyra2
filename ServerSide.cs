using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.Net;
using System.IdentityModel.Tokens;

namespace Siguri_Projekti2
{
    class ServerSide
    {
        private DESCryptoServiceProvider des;
        private RSACryptoServiceProvider rsa;
        static byte[] DesKey;
        private Socket server;

        public string Encrypt(string response) {
            byte[] byteResponse = Encoding.UTF8.GetBytes(response);
             des = new DESCryptoServiceProvider();
            des.Key = DesKey;
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.Zeros;
            des.GenerateIV(); //gjenerimi i IV`
            byte[] IV = des.IV;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms,des.CreateEncryptor(),CryptoStreamMode.Write);
            cs.Write(byteResponse,0, byteResponse.Length);
            cs.Close();
            byte[] encryptedResponse = ms.ToArray();
            byte[] concatenatedResponse = IV.Concat(encryptedResponse).ToArray();
            return Convert.ToBase64String(concatenatedResponse);
            
        }
        public string Decrypt(string clientMessage) {
            string[] explodedMessages = clientMessage.Split(',');
            byte[] IV = Convert.FromBase64String(explodedMessages[0]);
            byte[] enDesKey = Convert.FromBase64String(explodedMessages[1]);
            byte[] enMessage = Convert.FromBase64String(explodedMessages[2]);
            byte[] desKey = rsa.Decrypt(enDesKey,false);
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

        }
        public string JWTSignature(int userId,string name,string surname,string email) {
           

            return "";
        }
        public void serverconn()
        {
            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
                server = new Socket(ipAddr.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                server.Bind(localEndPoint);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }




        }

}
