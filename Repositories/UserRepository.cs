﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DataSecurity_pr2.Models;

namespace DataSecurity_pr2.Repositories
{
    class UserRepository
    {
        private static XmlDocument objXml = new XmlDocument();
        public static bool createUser(User user)
        {
            createUserDbIfItDoesntExists();
            objXml.Load("../../Database/users.fxml");
            if (findUser(user.getEmail())==null)
            {
                XmlElement rootNode = objXml.DocumentElement;
                XmlElement userNode = objXml.CreateElement("user");
                XmlElement nameNode = objXml.CreateElement("name");
                XmlElement surnameNode = objXml.CreateElement("surname");
                XmlElement emailNode = objXml.CreateElement("email");
                XmlElement idNode = objXml.CreateElement("id");
                XmlElement passwordNode = objXml.CreateElement("password");
                XmlElement saltNode = objXml.CreateElement("salt");

                nameNode.InnerText = user.getName();
                surnameNode.InnerText = user.getSurname();
                emailNode.InnerText = user.getEmail();
                idNode.InnerText = user.getId().ToString();
                passwordNode.InnerText = user.getPassword();
                saltNode.InnerText = user.getSalt();

                userNode.AppendChild(nameNode);
                userNode.AppendChild(surnameNode);
                userNode.AppendChild(emailNode);
                userNode.AppendChild(idNode);
                userNode.AppendChild(passwordNode);
                userNode.AppendChild(saltNode);
                rootNode.AppendChild(userNode);

                objXml.Save("../../Database/users.fxml");
                return true;

            }
            return false;
        }
        public static User findUser(string email) {
            objXml.Load("../../Database/users.fxml");
            XmlNodeList userList = objXml.GetElementsByTagName("user");
            foreach (XmlNode user in userList)
            {
                if (string.Equals(user["email"].InnerText, email))
                {
                    return new User(user["name"].InnerText, user["surname"].InnerText, user["email"].InnerText, int.Parse(user["id"].InnerText), user["password"].InnerText, user["salt"].InnerText);
                }
            }

            return null;
        }
    
        public static int countUsers()
        {
            var nodeCount = 0;
            var reader = XmlReader.Create("../../Database/users.fxml");
            
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element &&
                        reader.Name == "user")
                    {
                        nodeCount++;
                    }
                }
            reader.Close();
            return nodeCount;
        }
        private static void createUserDbIfItDoesntExists()
        {
            if (!File.Exists("../../Database/users.fxml"))
            {
                XmlTextWriter xmlTextWriter = new XmlTextWriter("../../Database/users.fxml", Encoding.UTF8);
                xmlTextWriter.WriteStartElement("users");
                xmlTextWriter.Close();
            }
        }
    }
}
