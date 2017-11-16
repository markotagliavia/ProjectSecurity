using SecurityManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum
{
    public class GroupChat
    {
        private static ObservableCollection<User> blocked;

        private static ObservableCollection<User> logged;

        private static  ObservableCollection<Message> allMessages;


        static GroupChat()
        {
            Blocked = new ObservableCollection<User>();
            Logged = new ObservableCollection<User>();
            allMessages = new ObservableCollection<Message>();
        }

        public static ObservableCollection<User> Blocked
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

        public static ObservableCollection<User> Logged
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

        public static ObservableCollection<Message> AllMessages
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
    }
}
