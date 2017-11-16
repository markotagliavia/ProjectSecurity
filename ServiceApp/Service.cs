using Contracts;
using Forum;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
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

        public int LogIn(string email, string password)
        {
            //TODO ubaciti u listo ulogovanih

            int ok = 0;

            Thread.CurrentPrincipal = loggedIn.Single(i => i.Email == email);

            User u = loggedIn.Single(i => i.Email == email);

            u.SecureCode = Guid.NewGuid();

            if (u != null)
            {
                if (!u.Verify)
                {
                    string your_id = "forumblok@gmail.com";
                    string your_password = "sifra123";
                    try
                    {
                        SmtpClient client = new SmtpClient();
                        client.Port = 587;
                        client.Host = "smtp.gmail.com";
                        client.EnableSsl = true;
                        client.Timeout = 10000;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new System.Net.NetworkCredential(your_id, your_password);

                        MailMessage mm = new MailMessage(your_id, "max.tagliavia@gmail.com");
                        mm.BodyEncoding = UTF8Encoding.UTF8;
                        mm.Subject = "CODE FOR FORUM";
                        mm.Body = "Secure code :" + u.SecureCode;
                        mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                        client.Send(mm);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Could not end email\n\n" + e.ToString());
                    }
                    return 0;
                }
                else
                {

                    if (password.Equals(u.Password)) //cuvati ih sifrovane u registraciji
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            else
            {
                return -1;
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



        public void Registration(string name, string sname, DateTime date, string gender, string email, string password)
        {
            /*if (loggedIn.Select(i => i.Email == email) != null)
            { //ispraviti da radi sa bool
                return;
            }*/
            //za test
            User u1 = new User("Marko", "Tagliavia", DateTime.Now, "max.tagliavia@gmail.com", "1234567", Roles.User, true);
            User u2 = new User("Tijana", "Lalosevic", DateTime.Now, "tijana.vdn@gmail.com", "1234567", Roles.Admin, true);
            User u3 = new User("Pijana", "Lalosevic", DateTime.Now, "marko@gmail.com", "1234567", Roles.Admin, false);
            loggedIn.Add(u1);
            loggedIn.Add(u2);
            loggedIn.Add(u3);

            if (!File.Exists("Users"))
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

        public bool SendVerificationKey(string key)
        {
            User user = Thread.CurrentPrincipal as User;
            bool ok = false;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.SendVerificationKey.ToString()))
            {
                if (user.SecureCode.ToString().Equals(key))
                {
                    ok = true;
                }

            }
            else
            {
                //TODO greske
            }

            return ok;

        }

        public string Sha256encrypt(string phrase)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            System.Security.Cryptography.SHA256Managed sha256hasher = new System.Security.Cryptography.SHA256Managed();
            byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(phrase));
            return Convert.ToBase64String(hashedDataBytes);
        }
    }
}
