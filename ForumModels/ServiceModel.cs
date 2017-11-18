using Forum;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumModels
{
    public sealed class ServiceModel
    {
        private static ServiceModel instance;

        private static readonly object padlock = new object();

        private ObservableCollection<User> loggedIn;

        private ObservableCollection<PrivateChat> privateChats;

        private GroupChat groupChat;

        private ObservableCollection<Room> roomList;

        private ObservableCollection<PrivateChat> privateChatList;

        private ServiceModel()
        {

            loggedIn = new ObservableCollection<User>();

            privateChats = new ObservableCollection<PrivateChat>();

            groupChat = new GroupChat();

            roomList = new ObservableCollection<Room>();

            privateChatList = new ObservableCollection<PrivateChat>();
        }

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

        public ObservableCollection<User> LoggedIn { get => loggedIn; set => loggedIn = value; }
        public ObservableCollection<PrivateChat> PrivateChats { get => privateChats; set => privateChats = value; }
        public GroupChat GroupChat { get => groupChat; set => groupChat = value; }
        public ObservableCollection<Room> RoomList { get => roomList; set => roomList = value; }
        public ObservableCollection<PrivateChat> PrivateChatList { get => privateChatList; set => privateChatList = value; }

    }
}
