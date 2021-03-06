﻿using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using SecurityManager;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Reflection;
using System.Xml;

namespace ServiceApp
{
    class Program
    {
        public string Sha256encrypt(string phrase)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            System.Security.Cryptography.SHA256Managed sha256hasher = new System.Security.Cryptography.SHA256Managed();
            byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(phrase));
            return Convert.ToBase64String(hashedDataBytes);
        } 

        static void Main(string[] args)
        {
            string ip = "";
            string port = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load((Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"NetworkConfig.xml")).ToString());
                ip = doc.GetElementsByTagName("IPAddress")[0].InnerXml;
                port = doc.GetElementsByTagName("Port")[0].InnerXml;
            }
            catch (Exception ex)
            {
                Console.WriteLine("NetworkConfig file is missing!");
                Console.ReadLine();
            }
            NetTcpBinding binding = new NetTcpBinding();
            string address = $"net.tcp://{ip}:{port}/ServiceApp";

            ServiceHost host = new ServiceHost(typeof(Service));
            host.AddServiceEndpoint(typeof(IService), binding, address);
            try
            {
                host.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("Address or port is already in use. Please configure your network different.");
                Console.ReadLine();
                Environment.Exit(0);
            }
            List<User> lista = new List<User>();
            if (!File.Exists("Users.dat"))
            {
                Program p = new Program();
                User u1 = new User("Adminko", "Adminic", DateTime.Now, "forumblok@gmail.com", p.Sha256encrypt("sifra123"), Roles.Admin, "Male");
                u1.Verify = true;
                User u2 = new User("Adminica", "Adminska", DateTime.Now, "forumblok1@gmail.com", p.Sha256encrypt("sifra1234"), Roles.Admin, "Female");
                u2.Verify = true;
                lista.Add(u1);
                lista.Add(u2);

                //izbrisati posle
                User u3 = new User("Adminko1", "Adminic1", DateTime.Now, "user1@gmail.com", p.Sha256encrypt("sifra123"), Roles.User, "Male");
                u3.Verify = true;
                User u4 = new User("Adminica1", "Adminska1", DateTime.Now, "user2@gmail.com", p.Sha256encrypt("sifra123"), Roles.User, "Female");
                u4.Verify = true;
                lista.Add(u3);
                lista.Add(u4);


                BinaryFormatter bf = new BinaryFormatter();

                Stream s = File.Open("Users.dat", FileMode.Create);
                try
                {
                    bf.Serialize(s, lista);

                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                    throw;
                }
                finally
                {
                    s.Close();
                }

                FileStream fs = new FileStream("Users.dat", FileMode.Open);  // cisto read bzvz da vidim jel dobro

                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    lista = (List<User>)formatter.Deserialize(fs);

                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                    throw;
                }
                finally
                {
                    fs.Close();
                }

                foreach (User u in lista)
                {
                    Console.WriteLine("{0} ovo je adminski mail", u.Email);
                }


            }



            
            Console.WriteLine("WCFService is opened. Press <enter> to finish...");
            Console.ReadLine();
            host.Close();
        }

    }

}
