using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    public class WCFClient : ChannelFactory<IService>, IService
    {
        IService factory;

        public WCFClient(NetTcpBinding binding, EndpointAddress address)
            : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public void ChangePassword(string oldPassowrd, string newPassword)
        {
            try
            {
                  factory.ChangePassword(oldPassowrd, newPassword);
                  Console.WriteLine("ChangePassword executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to ChangePassword(). {0}", e.Message);
            }
        }

        public void LogIn(string email, string password, string code)
        {
            try
            {
                factory.LogIn(email, password, code);
                Console.WriteLine("LogIn executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to LogIn(). {0}", e.Message);
            }
        }

        public void LogOut(string email)
        {
            try
            {
                factory.LogOut(email);
                Console.WriteLine("LogOut executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to LogOut(). {0}", e.Message);
            }
        }

        public void Registration(string name, string sname, DateTime date, char gender, string email, string password)
        {
            try
            {
                factory.Registration(name, sname, date, gender, email, password);
                Console.WriteLine("Registration executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Registration(). {0}", e.Message);
            }
        }

        public void ResetPassword(string email)
        {
            try
            {
                factory.ResetPassword(email);
                Console.WriteLine("ResetPassword executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to ResetPassword(). {0}", e.Message);
            }
        }
    }
}
