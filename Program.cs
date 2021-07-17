using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataSecurity_pr2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            User user = new User("Enes", "Hasani", "enes*hasani@student.uni-pr.edu", 2, "12345678", "12345678");
            UserRepository.createUser(user);
            Bill bill = new Bill("E parregullt", 2019, "January", 800);
            BillRepository.addBill(bill);
            //Console.WriteLine(UserRepository.countUsers());
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
           
        }
    }
}
