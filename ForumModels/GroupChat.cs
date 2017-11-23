using SecurityManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ForumModels
{
    [Serializable]
    public class GroupChat
    {
        private ObservableCollection<User> blocked;

        private ObservableCollection<User> logged;

        private ObservableCollection<Message> allMessages;

        private ObservableCollection<string> themeRooms;

        private ObservableCollection<string> privateChatsNames;


        public GroupChat()
        {
            Blocked = new ObservableCollection<User>();
            Logged = new ObservableCollection<User>();
            allMessages = new ObservableCollection<Message>();
            themeRooms = new ObservableCollection<string>();
            privateChatsNames = new ObservableCollection<string>();
        }

        public ObservableCollection<User> Blocked
        {
            get
            {
                return blocked;
            }

            set
            {
                blocked = value;
            }
        }

        public ObservableCollection<User> Logged
        {
            get
            {
                return logged;
            }

            set
            {
                logged = value;
            }
        }

        public ObservableCollection<Message> AllMessages
        {
            get
            {
                return allMessages;
            }

            set
            {
                allMessages = value;
            }
        }

        public ObservableCollection<string> ThemeRooms
        {
            get
            {
                return themeRooms;
            }

            set
            {
                themeRooms = value;
            }
        }

        public ObservableCollection<string> PrivateChatsNames
        {
            get
            {
                return privateChatsNames;
            }

            set
            {
                privateChatsNames = value;
            }
        }
    }
}
