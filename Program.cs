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
namespace DataSecurity_pr2
{
    static class Program
    {

        private const string secret = "enesh";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            //User user = new User("Enes", "Hasani", "enes*hasani@student.uni-pr.edu", 2, "12345678", "12345678");
            //UserRepository.createUser(user);
            //Bill bill = new Bill("E parregullt", 2019, "January", 800,2);
            //BillRepository.addBill(bill);
            //List<Bill> userBills = BillRepository.listUserBills(user.getId());
            // X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            //store.Open(OpenFlags.OpenExistingOnly);

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

        }
    }
}
