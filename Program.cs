using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Siguri_Projekti2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

using System.Threading.Tasks;
using System.Windows.Forms;
using DataSecurity_pr2.Repositories;
using DataSecurity_pr2.Models;
using System.Security.Cryptography;
using System.Text;

namespace DataSecurity_pr2
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //if(args.Length < 1)
            //{
            //    return;
            //}
            //else if(args[0]=="c")
            //{

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            //}
            //else if(args[0]=="s")
            //{
            //    //Application.EnableVisualStyles();
            //    //Application.SetCompatibleTextRenderingDefault(false);
            //    //Application.Run();
            ServerSide server = new ServerSide();

            //}
            //else
            //{
            //    return;
            //}
            //User user = new User("Enes", "Hasani", "enes.hasani@student.uni-pr.edu", 2, "12345678", "12345678");
            //UserRepository.createUser(user);
            //Bill bill = new Bill("E parregullt", 2019, "January", 800, 2);
            //BillRepository.addBill(bill);
            //List<Bill> userBills = BillRepository.listUserBills(user.getId());
            //X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            //store.Open(OpenFlags.OpenExistingOnly);
            //X509Certificate2Collection cert = X509Certificate2UI.SelectFromCollection(store.Certificates, "a", "b", X509SelectionFlag.SingleSelection);
            //X509Certificate2 certi = cert[0];
            //X509Certificate2 certifikata = new X509Certificate2("../../Siguri_Projekti2.cer", "123456");
            //Console.WriteLine(certifikata.HasPrivateKey);

            //RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)certifikata.PublicKey.Key;
            //  Console.WriteLine(Convert.ToBase64String((rsa.Encrypt(Encoding.UTF8.GetBytes("aa"), false))));

            // RSACryptoServiceProvider rsai = (RSACryptoServiceProvider)certi.PublicKey.Key;
            //Console.WriteLine(Convert.ToBase64String((rsai.Encrypt(Encoding.UTF8.GetBytes("aa"), false))));
            //if (Convert.ToBase64String((rsai.Encrypt(Encoding.UTF8.GetBytes("aa"), false))) == Convert.ToBase64String((rsa.Encrypt(Encoding.UTF8.GetBytes("aa"), false))))
            //{
            //    Console.WriteLine("true");
            //}
            //else {
            //    Console.WriteLine("f");
            //}


            //X509Certificate2 certifikata = new X509Certificate2("C:\\Users\\Amigos\\Desktop\\semestri 4\\siguria e t dhenave\\Siguri_Projekti2\\Siguri_Projekti2\\SRV.pfx", "123456");
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            //ClientSide.getJwtPayload(ServerSide.createJwtToken("enes.hasani@student.uni-pr.edu"));
        }
    }
}
