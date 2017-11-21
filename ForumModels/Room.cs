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
    public class Room
    {
        private string theme;

        private Guid code;

        private ObservableCollection<User> blocked;

        private ObservableCollection<User> logged;

        private ObservableCollection<Message> allMessages;


        public Room(string theme)
        {
            this.theme = theme;
            this.code = Guid.NewGuid();
            this.Blocked = new ObservableCollection<User>();
            this.Logged = new ObservableCollection<User>();
            this.AllMessages = new ObservableCollection<Message>();
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

        public string Theme
        {
            get
            {
                return theme;
            }

            set
            {
                theme = value;
            }
        }

        public Guid Code
        {
            get
            {
                return code;
            }

            set
            {
                code = value;
            }
        }
    }
}
