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

        public void AddAdmin(string email)
        {
            try
            {
                factory.AddAdmin(email);
                Console.WriteLine("AddAdmin executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to AddAdmin(). {0}", e.Message);
            }
        }

        public void BlockGroupChat(string blockEmai)
        {
            try
            {
                factory.BlockGroupChat(blockEmai);
                Console.WriteLine("BlockGroupChat executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to BlockGroupChat(). {0}", e.Message);
            }
        }

        public void BlockUser(string requestEmail, string blockEmail)
        {
            try
            {
                factory.BlockUser(requestEmail, blockEmail);
                Console.WriteLine("BlockUser executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to BlockUser(). {0}", e.Message);
            }
        }

        public void BlockUserFromRoom(string blockEmail, string roomName)
        {
            try
            {
                factory.BlockUserFromRoom(blockEmail, roomName);
                Console.WriteLine("BlockUserFromRoom executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to BlockUserFromRoom(). {0}", e.Message);
            }
        }

        public void ChangePassword(string oldPassowrd, string newPassword)
        {
            try
            {
                  factory.ChangePassword(oldPassowrd, newPassword);
                  Console.WriteLine("ChangePassword executed");
                Console.WriteLine("asdasdasdasdads");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to ChangePassword(). {0}", e.Message);
            }
        }

        public void CreatePrivateChat(string firstEmail, string secondEmail)
        {
            try
            {
                factory.CreatePrivateChat(firstEmail, secondEmail);
                Console.WriteLine("CreatePrivateChat executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to CreatePrivateChat(). {0}", e.Message);
            }
        }

        public void CreateRoom(string roomName)
        {
            try
            {
                factory.CreateRoom(roomName);
                Console.WriteLine("CreateRoom executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to CreateRoom(). {0}", e.Message);
            }
        }

        public void DeleteAdmin(string email)
        {
            try
            {
                factory.DeleteAdmin(email);
                Console.WriteLine("DeleteAdmin executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to DeleteAdmin(). {0}", e.Message);
            }
        }

        public int LogIn(string email, string password)
        {
            int i = 0;
            try
            {
                i = factory.LogIn(email, password);
                Console.WriteLine("LogIn executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to LogIn(). {0}", e.Message);
            }

            return i;
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

        public void Registration(string name, string sname, DateTime date, string gender, string email, string password)
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

        public void RemoveBlockGroupChat(string unblockEmail)
        {
            try
            {
                factory.RemoveBlockGroupChat(unblockEmail);
                Console.WriteLine("RemoveBlockGroupChat executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to RemoveBlockGroupChat(). {0}", e.Message);
            }
        }

        public void RemoveBlockUser(string requestEmail, string unblockEmail)
        {
            try
            {
                factory.RemoveBlockUser(requestEmail, unblockEmail);
                Console.WriteLine("RemoveBlockUser executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to RemoveBlockUser(). {0}", e.Message);
            }
        }

        public void RemoveBlockUserFromRoom(string unblockEmail, string roomName)
        {
            try
            {
                factory.RemoveBlockUserFromRoom(unblockEmail, roomName);
                Console.WriteLine("RemoveBlockUserFromRoom executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to RemoveBlockUserFromRoom(). {0}", e.Message);
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

        public bool SendVerificationKey(string key)
        {
            bool ok = false;
            try
            {
                ok = factory.SendVerificationKey(key);
                Console.WriteLine("SendVerificationKey executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to SendVerificationKey(). {0}", e.Message);
            }

            return ok;
        }

       
    }
}
