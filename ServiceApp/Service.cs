using Contracts;
using Forum;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceApp
{
    public class Service : IService
    {
        public static ObservableCollection<User> loggedIn = new ObservableCollection<User>();

        public static ObservableCollection<Room> rooms = new ObservableCollection<Room>();

        public static ObservableCollection<PrivateChat> privateChats = new ObservableCollection<PrivateChat>();


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
            //TODO ubaciti u listo ulogovanih

            Thread.CurrentPrincipal = loggedIn.Single(i => i.Email == email);
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



        public void Registration(string name, string sname, DateTime date, string gender, string email, string password)
        {
            User user = Thread.CurrentPrincipal as User;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.Registration.ToString()))
            {

                if(!File.Exists("Users"))
                {
                    using (BinaryWriter writer = new BinaryWriter(File.Open("Users", FileMode.Create)))
                    {
                        writer.Write(name);
                        writer.Write(sname);
                        writer.Write(date.ToBinary());
                        writer.Write(gender);
                        writer.Write(email);
                        writer.Write(password);

                    }

                }
                else
                {
                    using (BinaryWriter writer = new BinaryWriter(File.Open("Users", FileMode.Append)))
                    {
                        writer.Write(name);
                        writer.Write(sname);
                        writer.Write(date.ToBinary());
                        writer.Write(gender);
                        writer.Write(email);
                        writer.Write(password);

                    }

                }        
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
