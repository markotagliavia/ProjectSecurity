
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ForumModels
{
    [DataContract]
    public sealed class ServiceModel
    {
        private static ServiceModel instance;

        private static readonly object padlock = new object();

        private Dictionary<string, IChatServiceCallback> clients;

        private ObservableCollection<User> loggedIn;

        private ObservableCollection<PrivateChat> privateChats;

        private GroupChat groupChat;

        private ObservableCollection<Room> roomList;

        private ObservableCollection<PrivateChat> privateChatList;

        

        private ServiceModel()
        {
            Console.WriteLine("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            loggedIn = new ObservableCollection<User>();

            privateChats = new ObservableCollection<PrivateChat>();

            groupChat = new GroupChat();

            roomList = new ObservableCollection<Room>();

            privateChatList = new ObservableCollection<PrivateChat>();

            clients = new Dictionary<string, IChatServiceCallback>();
        }
        [DataMember]
        public Dictionary<string, IChatServiceCallback> Clients
        {
            get { return clients; }
            set { clients = value; }
        }
        [DataMember]
        public static ServiceModel Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ServiceModel();
                    }
                    return instance;
                }


            }
        }
        [DataMember]
        public ObservableCollection<User> LoggedIn
        {

            get
            {
                return loggedIn;
            }

            set
            {
                loggedIn = value;
            }

        }
        [DataMember]
        public ObservableCollection<PrivateChat> PrivateChats
        {
            get
            {
                return privateChats;
            }

            set
            {
                privateChats = value;
            }

          


        }
        [DataMember]
        public GroupChat GroupChat
        {
            get
            {
                return groupChat;
            }

            set
            {
                groupChat = value;
            }
          
        }
        [DataMember]
        public ObservableCollection<Room> RoomList
        {
            get
            {
                return roomList;
            }

            set
            {
                roomList = value;
            }
            
        }
        [DataMember]
        public ObservableCollection<PrivateChat> PrivateChatList
        {
            get
            {
                return privateChatList;
            }

            set
            {
                privateChatList = value;
            }
           
        }

       
    }
}
