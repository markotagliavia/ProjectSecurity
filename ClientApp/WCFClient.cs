﻿using Contracts;
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

    public class WCFClient : DuplexChannelFactory<IService>,IService,IDisposable
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

        public void AddAdmin(byte[] emailBytes, string emailHash)
        {
            try
            {
                factory.AddAdmin(emailBytes, emailHash);
                Console.WriteLine("AddAdmin executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to AddAdmin(). {0}", e.Message);
            }
         
        }

        public void BlockGroupChat(byte[] blockEmailBytes, string blockEmailHash)
        {
            try
            {
                factory.BlockGroupChat(blockEmailBytes, blockEmailHash);
                Console.WriteLine("BlockGroupChat executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to BlockGroupChat(). {0}", e.Message);
            }
        }
    
        public void BlockUser(byte[] requestEmailBytes, byte[] blokEmailBytes, string requestEmailHash, string blockEmailHash)
        {
            
            try
            {
                factory.BlockUser(requestEmailBytes, blokEmailBytes, requestEmailHash, blockEmailHash);
                Console.WriteLine("BlockUser executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to BlockUser(). {0}", e.Message);
            }
            
        }

        public void BlockUserFromRoom(byte[] blokEmailBytes, byte[] roomNameBytes, string blockEmailHash, string roomNameHash)
        {
            try
            {
                factory.BlockUserFromRoom(blokEmailBytes, roomNameBytes, blockEmailHash, roomNameHash);
                Console.WriteLine("BlockUserFromRoom executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to BlockUserFromRoom(). {0}", e.Message);
            }
        }

        public bool ChangePassword(byte[] emailBytes, byte[] oldPasswordBytes, byte[] newPasswordBytes, string emailHash, string oldPassowrdHash, string newPasswordHash)
        {
            bool retVal = false;
            try
            {
                retVal = factory.ChangePassword(emailBytes, oldPasswordBytes, newPasswordBytes, emailHash, oldPassowrdHash, newPasswordHash);
                Console.WriteLine("ChangePassword executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to ChangePassword(). {0}", e.Message);
            }
            return retVal;
        }

        public void CloseRoom(byte[] roomNameBytes, string roomNameHash)
        {
            
            try
            {
                factory.CloseRoom(roomNameBytes, roomNameHash);
                Console.WriteLine("CloseRoom executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to CloseRoom(). {0}", e.Message);
            }
            
        }

        public PrivateChat CreatePrivateChat(byte[] firstEmailBytes, byte[] secondEmailBytes, string firstEmailHash, string secondEmailHash)
        {
            PrivateChat retVal = null;
            try
            {
                retVal = factory.CreatePrivateChat(firstEmailBytes, secondEmailBytes, firstEmailHash, secondEmailHash);
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
                factory.CreateRoom(roomNameBytes, roomNameHash);
                Console.WriteLine("CreateRoom executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to CreateRoom(). {0}", e.Message);
            }
           
        }

        public void DeleteAdmin(byte[] emailBytes, string emailHash)
        {
            try
            {
                factory.DeleteAdmin(emailBytes, emailHash);
                Console.WriteLine("DeleteAdmin executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to DeleteAdmin(). {0}", e.Message);
            }
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
                room = factory.GetThemeRoom(roomNameBytes, emailBytes, roomNameHash, emailHash);
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
                retVal = factory.LogOut(emailBytes, emailHash);
                Console.WriteLine("LogOut executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to LogOut(). {0}", e.Message);
            }
            return retVal;
        }

        public bool Registration(byte[] nameBytes, byte[] snameBytes, byte[] dateBytes, byte[] genderBytes, byte[] emailBytes, byte[] passwordBytes, string nameHash, string snameHash, string dateHash, string genderHash, string emailHash, string passwordHash)
        {
            try
            {
                factory.Registration(nameBytes, snameBytes, dateBytes, genderBytes, emailBytes, passwordBytes, nameHash, snameHash, dateHash, genderHash, emailHash, passwordHash);
                Console.WriteLine("Registration executed");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Registration(). {0}", e.Message);
                return false;
            }
        }

        public void RemoveBlockGroupChat(byte[] unblockEmailBytes, string unblockEmailHash)
        {
            try
            {
                factory.RemoveBlockGroupChat(unblockEmailBytes, unblockEmailHash);
                Console.WriteLine("RemoveBlockGroupChat executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to RemoveBlockGroupChat(). {0}", e.Message);
            }

        }

        public void RemoveBlockUser(byte[] requestEmailBytes, byte[] unblockEmailBytes, string requestEmailHash, string unblockEmailHash)
        {
            try
            {
                factory.RemoveBlockUser(requestEmailBytes, unblockEmailBytes, requestEmailHash, unblockEmailHash);
                Console.WriteLine("RemoveBlockUser executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to RemoveBlockUser(). {0}", e.Message);
            }
        }

        public void RemoveBlockUserFromRoom(byte[] unblockEmailBytes, byte[] roomNameBytes, string unblockEmailHash, string roomNameHash)
        {
            try
            {
                factory.RemoveBlockUserFromRoom(unblockEmailBytes, roomNameBytes, unblockEmailHash, roomNameHash);
                Console.WriteLine("RemoveBlockUserFromRoom executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to RemoveBlockUserFromRoom(). {0}", e.Message);
            }

        }

        public int ResetPassword(byte[] emailBytes, string emailHash)
        {
            int ret = -1;
            try
            {
                ret = factory.ResetPassword(emailBytes, emailHash);
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
                factory.SendGroupMessage(userNameBytes, messageBytes, userNameHash, messageHash);
                Console.WriteLine("SendGroupMessage executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to SendGroupMessage(). {0}", e.Message);
            }

        }

        public void SendPrivateMessage(byte[] sendEmailBytes, byte[] reciveEmailBytes, byte[] messageBytes, string sendEmailHash, string reciveEmailHash, string messageHash)
        {
            try
            {
               factory.SendPrivateMessage(sendEmailBytes,reciveEmailBytes,messageBytes,sendEmailHash,reciveEmailHash,messageHash);
                Console.WriteLine("SendPrivateMessage executed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to SendPrivateMessage(). {0}", e.Message);
            }
        }

        public void SendRoomMessage(byte[] userNameBytes, byte[] roomNameBytes, byte[] messageBytes, string userNameHash, string roomNameHash, string messageHash)
        {
            
            try
            {
                factory.SendRoomMessage(userNameBytes, roomNameBytes, messageBytes, userNameHash, roomNameHash, messageHash);
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
                ok = factory.SendVerificationKey(keyBytes, keyHash);
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
                factory.Subscribe(emailBytes, emailHash);
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
                factory.SubscribeAllUsers(emailBytes, emailHash);
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
                return factory.GetAllUsers(emailBytes, emailHash);
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
                return factory.GetPrivateChat(codeByte, codeHash);
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
                factory.Unsubscribe(emailBytes, emailHash);
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
                factory.SubscribeUserTheme(emailBytes, themeBytes, emailHash, themeHash);
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
                factory.UnsubscribeUserTheme(emailBytes, themeBytes, emailHash, themeHash);
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
                factory.UnsubscribeAllUsers(emailBytes, emailHash);
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
                factory.LeaveRoom(themeBytes, themeHash);
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
                factory.LeavePrivateChat(codeByte, codeHash);
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
                factory.LogInTheme(themeBytes, emailBytes, themeHash,emailHash);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to LogInTheme(). {0}", e.Message);

            }
        }

        public void LogInPrivateChat(byte[] emailBytes, string emailHash,byte[] codeByte, string codeHash)
        {
            try
            {
                factory.LogInPrivateChat(emailBytes, emailHash,codeByte, codeHash);
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

        public void SessionKey(Guid code)
        {
            try
            {
                factory.SessionKey(code);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sending session key failed because {ex.Message}");
            }
        }

        public void SubscribeUserChat(byte[] emailBytes, byte[] themeBytes, string emailHash, string themeHash)

        {
            try
            {
                factory.SubscribeUserChat(emailBytes, themeBytes, emailHash, themeHash);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"SubscribeUserChat failed because {ex.Message}");
            }
        }

        public void UnsubscribeUserChat(byte[] emailBytes, byte[] themeBytes, string emailHash, string themeHash)
        {
            try
            {
                factory.UnsubscribeUserChat(emailBytes, themeBytes, emailHash, themeHash);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UnsubscribeUserChat failed because {ex.Message}");
            }
        }
    }
}

