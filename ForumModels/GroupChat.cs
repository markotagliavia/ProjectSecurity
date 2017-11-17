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
        private ObservableCollection<User> blocked;

        private ObservableCollection<User> logged;

        private ObservableCollection<Message> allMessages;


        public GroupChat()
        {
            Blocked = new ObservableCollection<User>();
            Logged = new ObservableCollection<User>();
            allMessages = new ObservableCollection<Message>();
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
    }
}
