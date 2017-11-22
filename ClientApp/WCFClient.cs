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
using System.Security.Cryptography;

namespace ClientApp
{

    public class WCFClient : DuplexChannelFactory<IService>,IService
    {
        IService factory;

        Guid guid;

        GenerateAesKey aes;

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

        public Guid Guid
        {
            get { return guid; }
            set { guid = value; }
        }

        public GenerateAesKey Aes
        {
            get
            {
                return aes;
            }
            set
            {
                aes = value;
            }
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

        public bool AddAdmin(byte[] emailBytes, string emailHash)
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

        public bool BlockGroupChat(byte[] blockEmailBytes, string blockEmailHash)
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
    
        public bool BlockUser(byte[] requestEmailBytes, byte[] blokEmailBytes, string requestEmailHash, string blockEmailHash)
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

        public bool BlockUserFromRoom(byte[] blokEmailBytes, byte[] roomNameBytes, string blockEmailHash, string roomNameHash)
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

        public bool ChangePassword(byte[] emailBytes, byte[] oldPasswordBytes, byte[] newPasswordBytes, string emailHash, string oldPassowrdHash, string newPasswordHash)
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

        public bool CloseRoom(byte[] roomNameBytes, string roomNameHash)
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

        public PrivateChat CreatePrivateChat(byte[] firstEmailBytes, byte[] secondEmailBytes, string firstEmailHash, string secondEmailHash)
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

        public void CreateRoom(byte[] roomNameBytes, string roomNameHash)
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

        public bool DeleteAdmin(byte[] emailBytes, string emailHash)
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

        public Room GetThemeRoom(byte[] roomNameBytes, byte[] emailBytes, string roomNameHash,string emailHash)
        {
            Room room = new Room("");
            try
            {
                room = factory.GetThemeRoom(roomName,email);
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

        public int LogIn(byte[] email, byte[] password, string emailHash, string passwordHash)
        {
            int i = 0;
            try
            {
                i = factory.LogIn(email, password, emailHash, passwordHash);
                Console.WriteLine("LogIn executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to LogIn(). {0}", e.Message);
            }

            return i;
        }

        public bool LogOut(byte[] emailBytes, string emailHash)
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

        public bool RemoveBlockGroupChat(byte[] unblockEmailBytes, string unblockEmailHash)
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

        public bool RemoveBlockUser(byte[] requestEmailBytes, byte[] unblockEmailBytes, string requestEmailHash, string unblockEmailHash)
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

        public bool RemoveBlockUserFromRoom(byte[] unblockEmailBytes, byte[] roomNameBytes, string unblockEmailHash, string roomNameHash)
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

        public int ResetPassword(byte[] emailBytes, string emailHash)
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

        public void SendGroupMessage(byte[] userNameBytes, byte[] messageBytes, string userNameHash, string messageHash)
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

        public bool SendPrivateMessage(byte[] sendEmailBytes, byte[] reciveEmailBytes, byte[] messageBytes, string sendEmailHash, string reciveEmailHash, string messageHash)
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

        public void SendRoomMessage(byte[] userNameBytes, byte[] roomNameBytes, byte[] messageBytes, string userNameHash, string roomNameHash, string messageHash)
        {
            
            try
            {
                factory.SendRoomMessage(userName, roomName, message);
                Console.WriteLine("SendRoomMessage executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to SendRoomMessage(). {0}", e.Message);
            }

            
        }

        public bool SendVerificationKey(byte[] keyBytes, string keyHash)
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

        public void Subscribe(byte[] emailBytes, string emailHash)
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

        public void SubscribeAllUsers(byte[] emailBytes, string emailHash)
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

        public ObservableCollection<User> GetAllUsers(byte[] emailBytes, string emailHash)
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

        public PrivateChat GetPrivateChat(byte[] codeByte, string codeHash)
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

        public void Unsubscribe(byte[] emailBytes, string emailHash)
        {
            try
            {
                factory.Unsubscribe(email);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Unsubscribe(). {0}", e.Message);
                
            }
        }

        public void SubscribeUserTheme(byte[] emailBytes, byte[] themeBytes, string emailHash, string themeHash)
        {
            try
            {
                factory.SubscribeUserTheme(email, theme);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to SubscribeUserTheme(). {0}", e.Message);

            }
        }

        public void UnsubscribeUserTheme(byte[] emailBytes, byte[] themeBytes, string emailHash, string themeHash)
        {
            try
            {
                factory.UnsubscribeUserTheme(email, theme);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to UnsubscribeUserTheme(). {0}", e.Message);

            }
        }

        public void UnsubscribeAllUsers(byte[] emailBytes, string emailHash)
        {
            try
            {
                factory.UnsubscribeAllUsers(email);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to UnsubscribeAllUsers(). {0}", e.Message);

            }
        }

        public void LeaveRoom(byte[] themeBytes, string themeHash)
        {
            try
            {
                factory.LeaveRoom(theme);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to LeaveRoom(). {0}", e.Message);

            }
        }

        public void LeavePrivateChat(byte[] codeByte, string codeHash)
        {
            try
            {
                factory.LeavePrivateChat(code);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to LeavePrivateChat(). {0}", e.Message);

            }
        }

        public void LogInTheme(byte[] themeBytes, byte[] emailBytes, string themeHash, string emailHash)
        {
            try
            {
                factory.LogInTheme(theme,email);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to LogInTheme(). {0}", e.Message);

            }
        }

        public void LogInPrivateChat(byte[] codeByte, string codeHash)
        {
            try
            {
                factory.LogInPrivateChat(code);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to LogInPrivateChat(). {0}", e.Message);

            }
        }

        public RSAParameters GetPublicKey(Guid code)
        {
            RSAParameters a = new RSAParameters();
            try
            {
                a = factory.GetPublicKey(code);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to GetAesKey(). {0}", e.Message);

            }
                 ;
            return a;
        }

        public bool SendSessionKey(byte[] crypted)
        {
            bool retVal = false;
            try
            {
                retVal = factory.SendSessionKey(crypted);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sending session key failed because {ex.Message}");
            }
            return retVal;
        }
    }
}

