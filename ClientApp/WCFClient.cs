using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Forum;

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

        public bool AddAdmin(string email)
        {
            bool retVal = false;
            try
            {
                retVal = factory.AddAdmin(email);
                Console.WriteLine("AddAdmin executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to AddAdmin(). {0}", e.Message);
            }
            return retVal;
        }

        public bool BlockGroupChat(string blockEmai)
        {
            bool retVal = false;
            try
            {
                retVal = factory.BlockGroupChat(blockEmai);
                Console.WriteLine("BlockGroupChat executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to BlockGroupChat(). {0}", e.Message);
            }

            return retVal;
        }

        public bool BlockUser(string requestEmail, string blockEmail)
        {
            bool retVal = false;
            try
            {
                retVal = factory.BlockUser(requestEmail, blockEmail);
                Console.WriteLine("BlockUser executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to BlockUser(). {0}", e.Message);
            }
            return retVal;
        }

        public bool BlockUserFromRoom(string blockEmail, string roomName)
        {
            bool retVal = false;
            try
            {
                retVal = factory.BlockUserFromRoom(blockEmail, roomName);
                Console.WriteLine("BlockUserFromRoom executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to BlockUserFromRoom(). {0}", e.Message);
            }

            return retVal;
        }

        public bool ChangePassword(string email, string oldPassowrd, string newPassword)
        {
            bool retVal = false;
            try
            {
                retVal = factory.ChangePassword(email, oldPassowrd, newPassword);
                Console.WriteLine("ChangePassword executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to ChangePassword(). {0}", e.Message);
            }
            return retVal;
        }

        public bool CloseRoom(string roomName, string email)
        {
            throw new NotImplementedException();
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

        public bool CreateRoom(string roomName)
        {
            bool retVal = false;
            try
            {
                retVal = factory.CreateRoom(roomName);
                Console.WriteLine("CreateRoom executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to CreateRoom(). {0}", e.Message);
            }
            return retVal;
        }

        public bool DeleteAdmin(string email)
        {
            bool retVal = false;
            try
            {
                retVal = factory.DeleteAdmin(email);
                Console.WriteLine("DeleteAdmin executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to DeleteAdmin(). {0}", e.Message);
            }

            return retVal;
        }

        public Forum.GroupChat GetGroupChat()
        {
            Forum.GroupChat groupChat = new Forum.GroupChat();
            try
            {
                groupChat = factory.GetGroupChat();
                Console.WriteLine("GetGroupChat executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to GetGroupChat(). {0}", e.Message);
            }

            return groupChat;
        }

        public Room GetPrivateRoom(string roomName)
        {
            Room room = new Room();
            try
            {
                room = factory.GetPrivateRoom(roomName);
                Console.WriteLine("GetPrivateRoom executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to GetPrivateRoom(). {0}", e.Message);
            }

            return room;
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

        public bool LogOut(string email)
        {
            bool retVal = false;
            try
            {
                retVal = factory.LogOut(email);
                Console.WriteLine("LogOut executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to LogOut(). {0}", e.Message);
            }
            return retVal;
        }

        public bool Registration(string name, string sname, DateTime date, string gender, string email, string password)
        {
            try
            {
                factory.Registration(name, sname, date, gender, email, password);
                Console.WriteLine("Registration executed");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Registration(). {0}", e.Message);
                return false;
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

        public bool RemoveBlockUser(string requestEmail, string unblockEmail)
        {
            bool retVal = false;
            try
            {
                retVal = factory.RemoveBlockUser(requestEmail, unblockEmail);
                Console.WriteLine("RemoveBlockUser executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to RemoveBlockUser(). {0}", e.Message);
            }
            return retVal;
        }

        public bool RemoveBlockUserFromRoom(string unblockEmail, string roomName)
        {
            bool retVal = false;
            try
            {
                retVal = factory.RemoveBlockUserFromRoom(unblockEmail, roomName);
                Console.WriteLine("RemoveBlockUserFromRoom executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to RemoveBlockUserFromRoom(). {0}", e.Message);
            }

            return retVal;
        }

        public int ResetPassword(string email)
        {
            int ret = -1;
            try
            {
                ret = factory.ResetPassword(email);
                Console.WriteLine("ResetPassword executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to ResetPassword(). {0}", e.Message);
            }

            return ret;
        }

        public bool SendGroupMessage(string email, string message)
        {
            throw new NotImplementedException();
        }

        public bool SendPrivateMessage(string firstEmail, string secondEmail, string message)
        {
            throw new NotImplementedException();
        }

        public bool SendRoomMessage(string email, string roomName, string message)
        {
            throw new NotImplementedException();
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
