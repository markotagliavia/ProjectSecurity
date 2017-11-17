using SecurityManager;
using System;

namespace Forum
{
    public class PrivateChat
    {
        private Guid uid;

        private string user1;

        private string user2;

        public PrivateChat(string user1, string user2)
        {
            this.User1 = user1;
            this.user2 = user2;
            this.uid = System.Runtime.InteropServices.Marshal.GenerateGuidForType(typeof(int));
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
