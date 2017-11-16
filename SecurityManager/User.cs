using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class User : IPrincipal
    {
        private GenericIdentity identity;

        private Guid code;

        private bool gender;

        private bool logged;

        private string firstName;

        private string lastName;

        private DateTime birthDate;

        private string email;

        private string password;

        private Roles role;

        private Guid secureCode;

        private ObservableCollection<User> blocked;

        private bool verify;


        public User(string firstname, string lastname, DateTime birthDate, string email, string password, Roles role, bool gender)
        {
            this.Logged = false;
           
            this.code = System.Runtime.InteropServices.Marshal.GenerateGuidForType(typeof(int));
            this.firstName = firstname;
            this.LastName = lastName;
            this.gender = gender;
            this.email = email;
            this.password = password;
            this.Role = role;
            this.birthDate = birthDate;
            this.Blocked = new ObservableCollection<User>();
            this.verify = false;

            this.identity = new GenericIdentity(email);
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

        public bool Verify
        {
            get
            {
                return verify;
            }

            set
            {
                verify = value;
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

        public Guid SecureCode
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

        public string Name
        {
            get
            {
                return identity.Name;
            }

        }

        public string AuthenticationType
        {
            get
            {
                return identity.AuthenticationType;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return identity.IsAuthenticated;
            }
        }

        public IIdentity Identity
        {
            get { return this.identity; }
        }

        public bool IsInRole(string perms)
        {
            bool IsAuthz = false;
            foreach (string p in RolesConfiguration.GetPermissions(Role.ToString()))
            {
                if (p.Equals(perms))
                {
                    IsAuthz = true;
                    break;
                }
            }

            return IsAuthz;
        }
    }
}
