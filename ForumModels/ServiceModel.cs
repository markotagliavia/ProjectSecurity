﻿
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

        private Dictionary<string, IChatServiceCallback> clientsforViewAdmins;

        private Dictionary<string, Dictionary<string, IChatServiceCallback>> clientsforThemeRoom;

        private Dictionary<Guid, Dictionary<string, IChatServiceCallback>> clientsforPrivateChat;

        private ObservableCollection<User> loggedIn;

        private GroupChat groupChat;

        private ObservableCollection<Room> roomList;

        private ObservableCollection<PrivateChat> privateChatList;

        private object lockGroupChat;

        private object lockPrivateChat;

        private object lockUsers;

        private object themeChatLock;

        private GennerateRsaKey rsa;

        private ServiceModel()
        {
            //Console.WriteLine("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

            loggedIn = new ObservableCollection<User>();

            groupChat = new GroupChat();

            roomList = new ObservableCollection<Room>();

            privateChatList = new ObservableCollection<PrivateChat>();

            clientsforThemeRoom = new Dictionary<string, Dictionary<string, IChatServiceCallback>>();

            clientsforPrivateChat = new Dictionary<Guid, Dictionary<string, IChatServiceCallback>>();

            clients = new Dictionary<string, IChatServiceCallback>();//ne cuvaj u fajl, to mi cuva proxyje u runtime-u

            clientsforViewAdmins = new Dictionary<string, IChatServiceCallback>();//ne cuvaj u fajl, to mi cuva proxyje u runtime-u

            lockGroupChat = new object();

            lockPrivateChat = new object();

            lockUsers = new object();

            themeChatLock = new object();

            rsa = new GennerateRsaKey(2048);
        }
        [DataMember]
        public GennerateRsaKey RSA
        {
            get { return rsa; }
            set { rsa = value; }
        }
        [DataMember]
        public object LockUsers
        {
            get { return lockUsers; }
            set { lockUsers = value; }
        }

        [DataMember]
        public object LockGroupChat
        {
            get { return lockGroupChat; }
            set { lockGroupChat = value; }
        }

        [DataMember]
        public object LockPrivateChat
        {
            get { return lockPrivateChat; }
            set { lockPrivateChat = value; }
        }

        [DataMember]
        public object ThemeChatLock
        {
            get { return themeChatLock; }
            set { themeChatLock = value; }
        }

        [DataMember]
        public Dictionary<string, Dictionary<string, IChatServiceCallback>> ClientsForThemeRoom
        {
            get { return clientsforThemeRoom; }
            set { clientsforThemeRoom = value; }
        }
        [DataMember]
        public Dictionary<Guid, Dictionary<string, IChatServiceCallback>> ClientsForPrivateChat
        {
            get { return clientsforPrivateChat; }
            set { clientsforPrivateChat = value; }
        }
        [DataMember]
        public Dictionary<string, IChatServiceCallback> Clients
        {
            get { return clients; }
            set { clients = value; }
        }
        [DataMember]
        public Dictionary<string, IChatServiceCallback> ClientsForViewAdmins
        {
            get { return clientsforViewAdmins; }
            set { clientsforViewAdmins = value; }
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
