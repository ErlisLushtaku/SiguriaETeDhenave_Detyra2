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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
