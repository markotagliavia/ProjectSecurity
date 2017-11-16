using Contracts;
using Forum;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceApp
{
    public class Service : IService
    {
        private ObservableCollection<User> loggedIn;

        private ObservableCollection<Room> rooms;

        private ObservableCollection<PrivateChat> privateChats;


        public Service()
        {
            this.LoggedIn = new ObservableCollection<User>();
            this.Rooms = new ObservableCollection<Room>();
            this.PrivateChats = new ObservableCollection<PrivateChat>();
        }

        public ObservableCollection<PrivateChat> PrivateChats
        {
            get
            {
                return privateChats;
            }

            set
            {
                privateChats = value;
            }
        }

        public ObservableCollection<Room> Rooms
        {
            get
            {
                return rooms;
            }

            set
            {
                rooms = value;
            }
        }

        public ObservableCollection<User> LoggedIn
        {
            get
            {
                return loggedIn;
            }

            set
            {
                loggedIn = value;
            }
        }

        public void AddAdmin(string email)
        {
            User user = Thread.CurrentPrincipal as User;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.AddAdmin.ToString()))
            {
                //TODO fje
            }
            else
            {
                //TODO greske
            }
        }

        public void BlockGroupChat(string blockEmail)
        {
            User user = Thread.CurrentPrincipal as User;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.BlockGroupChat.ToString()))
            {
                //TODO fje
            }
            else
            {
                //TODO greske
            }
        }

        public void BlockUser(string requestEmail, string blockEmail)
        {
            User user = Thread.CurrentPrincipal as User;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.BlockUser.ToString()))
            {
                //TODO fje
            }
            else
            {
                //TODO greske
            }
        }

        public void BlockUserFromRoom(string blockEmail, string roomName)
        {
            User user = Thread.CurrentPrincipal as User;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.BlockUserFromRoom.ToString()))
            {
                //TODO fje
            }
            else
            {
                //TODO greske
            }
        }

        public void ChangePassword(string oldPassowrd, string newPassword)
        {
            User user = Thread.CurrentPrincipal as User;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.ChangePassword.ToString()))
            {
                //TODO fje
            }
            else
            {
                //TODO greske
            }
        }

        public void CreatePrivateChat(string firstEmail, string secondEmail)
        {
            User user = Thread.CurrentPrincipal as User;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.CreatePrivateChat.ToString()))
            {
                //TODO fje
            }
            else
            {
                //TODO greske
            }
        }

        public void CreateRoom(string roomName)
        {
            User user = Thread.CurrentPrincipal as User;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.CreateRoom.ToString()))
            {
                //TODO fje
            }
            else
            {
                //TODO greske
            }
        }

        public void DeleteAdmin(string email)
        {
            User user = Thread.CurrentPrincipal as User;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.DeleteAdmin.ToString()))
            {
                //TODO fje
            }
            else
            {
                //TODO greske
            }
        }

        public void LogIn(string email, string password, string code)
        {
            User user = Thread.CurrentPrincipal as User;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.LogIn.ToString()))
            {
                //TODO fje
            }
            else
            {
                //TODO greske
            }
        }

        public void LogOut(string email)
        {
            User user = Thread.CurrentPrincipal as User;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.LogOut.ToString()))
            {
                //TODO fje
            }
            else
            {
                //TODO greske
            }
        }

        public void Registration(string name, string sname, DateTime date, char gender, string email, string password)
        {
            User user = Thread.CurrentPrincipal as User;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.Registration.ToString()))
            {
                //TODO fje
            }
            else
            {
                //TODO greske
            }
        }

        public void RemoveBlockGroupChat(string unblockEmail)
        {
            User user = Thread.CurrentPrincipal as User;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.RemoveBlockGroupChat.ToString()))
            {
                //TODO fje
            }
            else
            {
                //TODO greske
            }
        }

        public void RemoveBlockUser(string requestEmail, string unblockEmail)
        {
            User user = Thread.CurrentPrincipal as User;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.RemoveBlockUser.ToString()))
            {
                //TODO fje
            }
            else
            {
                //TODO greske
            }
        }

        public void RemoveBlockUserFromRoom(string unblockEmail, string roomName)
        {
            User user = Thread.CurrentPrincipal as User;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.RemoveBlockRoom.ToString()))
            {
                //TODO fje
            }
            else
            {
                //TODO greske
            }
        }

        public void ResetPassword(string email)
        {
            User user = Thread.CurrentPrincipal as User;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.ResetPassword.ToString()))
            {
                //TODO fje
            }
            else
            {
                //TODO greske
            }
        }
    }
}
