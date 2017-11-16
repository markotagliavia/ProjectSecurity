using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum
{
    public class Message
    {
        private string text;

        private Guid code;

        private Guid user;

        private DateTime creationTime;

        public Message(string text, Guid user)
        {
            this.Text = text;
            this.User = user;
            this.Code = System.Runtime.InteropServices.Marshal.GenerateGuidForType(typeof(int));
            this.CreationTime = DateTime.Now;
        }

        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
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

        public Guid User
        {
            get
            {
                return user;
            }

            set
            {
                user = value;
            }
        }

        public DateTime CreationTime
        {
            get
            {
                return creationTime;
            }

            set
            {
                creationTime = value;
            }
        }
    }
}
