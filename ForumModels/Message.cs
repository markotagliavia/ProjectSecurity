using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ForumModels
{
    [Serializable]
    public class Message
    {
        private string text;

        private Guid code;

        private string user;

        private DateTime creationTime;

        public Message(string text, string user)
        {
            this.Text = text;
            this.User = user;
            //this.Code = System.Runtime.InteropServices.Marshal.GenerateGuidForType(typeof(int));
            this.Code = Guid.NewGuid();
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

        public string User
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

        public override string ToString()
        {
            return $"****************************************************************************\n{User} wrote on {CreationTime.ToString()} :\n------------------------------------------------------------------------------------------\n{Text}\n****************************************************************************\n";
        }
    }
}
