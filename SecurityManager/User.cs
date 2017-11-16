using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class User
    {
        private Guid code;

        private bool gender;

        private bool logged;

        private string firstName;

        private string lastName;

        private DateTime birthDate;

        private string email;

        private string password;

        private Roles role;

        private int secureCode;

        private ObservableCollection<User> blocked;


        public User(string firstname, string lastname, DateTime birthDate, string email, string password, Roles role, bool gender)
        {
            this.Logged = false;
            this.secureCode = 0; //TO DO
            this.code = System.Runtime.InteropServices.Marshal.GenerateGuidForType(typeof(int));
            this.firstName = firstname;
            this.LastName = lastName;
            this.gender = gender;
            this.email = email;
            this.password = password;
            this.Role = role;
            this.birthDate = birthDate;
            this.Blocked = new ObservableCollection<User>();
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

        public string FirstName
        {
            get
            {
                return firstName;
            }

            set
            {
                firstName = value;
            }
        }

        public string LastName
        {
            get
            {
                return lastName;
            }

            set
            {
                lastName = value;
            }
        }

        public DateTime BirthDate
        {
            get
            {
                return birthDate;
            }

            set
            {
                birthDate = value;
            }
        }

        public string Email
        {
            get
            {
                return email;
            }

            set
            {
                email = value;
            }
        }

        public string Password
        {
            get
            {
                return password;
            }

            set
            {
                password = value;
            }
        }

        public Roles Role
        {
            get
            {
                return role;
            }

            set
            {
                role = value;
            }
        }

        public int SecureCode
        {
            get
            {
                return secureCode;
            }

            set
            {
                secureCode = value;
            }
        }

        public bool Gender
        {
            get
            {
                return gender;
            }

            set
            {
                gender = value;
            }
        }

        public bool Logged
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
    }
}
