using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using SecurityManager;
using System.IO;

namespace ServiceApp
{
    class Program
    {
        static void Main(string[] args)
        {

            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/ServiceApp";

            ServiceHost host = new ServiceHost(typeof(Service));
            host.AddServiceEndpoint(typeof(IService), binding, address);

            host.Open();
            using (BinaryWriter writer = new BinaryWriter(File.Open("Users", FileMode.Create)))
            {
                writer.Write("admin");
                writer.Write("adminovic");
                writer.Write(DateTime.Now.ToString());
                writer.Write("Male");
                writer.Write("admin@gmail.com");
                writer.Write("admin");

            }

           /* string name;                       // CITANJE CISTO DA VIDIM JEL DOBRO 
            string surname;
            string date;
            string gender;
            string mail;
            string pass;

            if (File.Exists("Users"))
            {
                using (BinaryReader reader = new BinaryReader(File.Open("Users", FileMode.Open)))
                {
                    name = reader.ReadString();
                    surname = reader.ReadString();
                    date = reader.ReadString();
                    gender = reader.ReadString();
                    mail = reader.ReadString();
                    pass = reader.ReadString();
                }

                Console.WriteLine("Name: " + name);
                Console.WriteLine("surname: " + surname);
                Console.WriteLine("date: " + date);
                Console.WriteLine("gender: " + gender);
                Console.WriteLine("mail: " + mail);
                Console.WriteLine("pass: " + pass);

            }
            */

            Console.WriteLine("WCFService is opened. Press <enter> to finish...");
            Console.ReadLine();
            host.Close();
        }
    }
}
