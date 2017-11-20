using Contracts;
using ForumModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SecurityManager;
using System.Collections.ObjectModel;

namespace ClientApp
{

    public class WCFClient : DuplexChannelFactory<IService>,IService
    {
        IService factory;

        private InstanceContext instanceContext;

        NetTcpBinding binding;

        EndpointAddress address;

        public WCFClient(InstanceContext instanceContext,NetTcpBinding binding, EndpointAddress address)
            :base(instanceContext,binding,address)
        {
            this.address = address;
            this.binding = binding;
            this.binding.MaxConnections = 500;
            this.binding.OpenTimeout = new TimeSpan(0, 10, 0);
            this.binding.CloseTimeout = new TimeSpan(0, 10, 0);
            this.binding.SendTimeout = new TimeSpan(0, 1, 0);
            this.binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
            this.instanceContext = instanceContext;
           factory = this.CreateChannel();    
        }

        public EndpointAddress Address
        {
            get { return address; }
        }

        public NetTcpBinding Binding
        {
            get { return binding; }
        }

        public InstanceContext InstanceContext
        {
            get { return instanceContext; }
            set { this.instanceContext = value; }
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

        public bool CloseRoom(string roomName)
        {
            bool retVal = false;
            try
            {
                retVal = factory.CloseRoom(roomName);
                Console.WriteLine("CloseRoom executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to CloseRoom(). {0}", e.Message);
            }
            return retVal;
        }

        public PrivateChat CreatePrivateChat(string firstEmail, string secondEmail)
        {
            PrivateChat retVal = null;
            try
            {
                retVal = factory.CreatePrivateChat(firstEmail, secondEmail);
                Console.WriteLine("CreatePrivateChat executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to CreatePrivateChat(). {0}", e.Message);
            }
            return retVal;
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

        public ForumModels.GroupChat GetGroupChat()
        {
            ForumModels.GroupChat groupChat = null;
            try
            {
                groupChat = factory.GetGroupChat();
                Console.WriteLine("GetGroupChat executed");
                return groupChat;
                
            }
            catch (CommunicationException commProblem)
            {
                Console.WriteLine("There was a communication problem. " + commProblem.Message + commProblem.StackTrace);
                return null;
            }


        }

        public Room GetThemeRoom(string roomName)
        {
            Room room = new Room("");
            try
            {
                room = factory.GetThemeRoom(roomName);
                Console.WriteLine("GetThemeRoom executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to GetThemeRoom(). {0}", e.Message);
            }

            return room;
        }

        public void KeepConnection()
        {
            try
            {
                factory.KeepConnection();
                Console.WriteLine("KeepConnection");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to KeepConnection(). {0}", e.Message);
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

        public bool RemoveBlockGroupChat(string unblockEmail)
        {
            bool retVal = false;

            try
            {
                retVal = factory.RemoveBlockGroupChat(unblockEmail);
                Console.WriteLine("RemoveBlockGroupChat executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to RemoveBlockGroupChat(). {0}", e.Message);
            }

            return retVal;
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

        public void SendGroupMessage(string userName, string message)
        {
            try
            {
                factory.SendGroupMessage(userName, message);
                Console.WriteLine("SendGroupMessage executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to SendGroupMessage(). {0}", e.Message);
            }

        }

        public bool SendPrivateMessage(string sendEmail, string reciveEmail, string message)
        {
            bool retVal = false;
            try
            {
                retVal = factory.SendPrivateMessage(sendEmail, reciveEmail, message);
                Console.WriteLine("SendPrivateMessage executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to SendPrivateMessage(). {0}", e.Message);
            }

            return retVal;
        }

        public bool SendRoomMessage(string userName, string roomName, string message)
        {
            bool retVal = false;
            try
            {
                retVal = factory.SendRoomMessage(userName, roomName, message);
                Console.WriteLine("SendRoomMessage executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to SendRoomMessage(). {0}", e.Message);
            }

            return retVal;
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

        public void Subscribe(string email)
        {
            try
            {
                factory.Subscribe(email);
                Console.WriteLine("subscribe executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Subscribe(). {0}", e.Message);
            }
        }

        public void SubscribeAllUsers(string email)
        {
            try
            {
                factory.SubscribeAllUsers(email);
                Console.WriteLine("SubscribeAllUsers executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to SubscribeAllUsers(). {0}", e.Message);
            }
        }

        public ObservableCollection<User> GetAllUsers(string email)
        {
            try
            {
                return factory.GetAllUsers(email);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to GetAllUsers(). {0}", e.Message);
                return null;
            }
        }

        public PrivateChat GetPrivateChat(Guid code)
        {
            try
            {
                return factory.GetPrivateChat(code);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to GetPrivateChat(). {0}", e.Message);
                return null;
            }
        }
    }
}
