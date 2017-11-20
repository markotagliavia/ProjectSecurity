using SecurityManager;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ForumModels
{
    [Serializable]
    public class PrivateChat
    {
        
        private Guid uid;

        private string user1;

        private string user2;

        private ObservableCollection<Message> messages;

        public PrivateChat(string user1, string user2)
        {
            this.User1 = user1;
            this.user2 = user2;
            this.uid =  Guid.NewGuid();//System.Runtime.InteropServices.Marshal.GenerateGuidForType(typeof(int));
            messages = new ObservableCollection<Message>();
        }

        public ObservableCollection<Message> Messages
        {
            get
            {
                return messages;
            }

            set
            {
                messages = value;
            }
        }

        public Guid Uid
        {
            get
            {
                return uid;
            }

            set
            {
                uid = value;
            }
        }

        public string User1
        {
            get
            {
                return user1;
            }

            set
            {
                user1 = value;
            }
        }

        public string User2
        {
            get
            {
                return user2;
            }

            set
            {
                user2 = value;
            }
        }
    }
}
