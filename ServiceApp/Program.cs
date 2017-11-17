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
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace ServiceApp
{
    class Program
    {
        static void Main(string[] args)
        {

            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:27019/ServiceApp";

            ServiceHost host = new ServiceHost(typeof(Service));
            host.AddServiceEndpoint(typeof(IService), binding, address);

            host.Open();

            List<User> lista = new List<User>();

            User u1 = new User("admin", "adminovic", DateTime.Now, "admin@gmail.com", "admin", Roles.User, "Male");
            lista.Add(u1);

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
                Console.WriteLine("{0} ovo je mail", u.Email);
            }



            Console.WriteLine("WCFService is opened. Press <enter> to finish...");
            Console.ReadLine();
            host.Close();
        }
    }
}
