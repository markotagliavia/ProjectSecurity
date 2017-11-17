using Contracts;
using Forum;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceApp
{
    public class Service : IService
    {
        public static ObservableCollection<User> loggedIn = new ObservableCollection<User>();

        public static ObservableCollection<PrivateChat> privateChats = new ObservableCollection<PrivateChat>();

        public static GroupChat groupChat = new GroupChat();

        public static ObservableCollection<Room> roomList = new ObservableCollection<Room>();

        public List<User> DeserializeUsers()
        {
            List<User> lista = new List<User>();

            FileStream fs = new FileStream("Users.dat", FileMode.Open);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                lista = (List<User>)formatter.Deserialize(fs);

            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }


            return lista;
        }

        public bool AddAdmin(string email)
        {
            User user = Thread.CurrentPrincipal as User;
            bool retVal = false;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.AddAdmin.ToString()))
            {
                //TODO fje
                if (user.Logged)
                {
                    User u = loggedIn.Single(i => i.Email == email);
                    u.Role = Roles.Admin;
                    retVal = true;
                }else
                {
                    Console.WriteLine("User {0} is not logged in!", user.Name);
                }
            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }
            return retVal;
        }

        public bool BlockGroupChat(string blockEmail)
        {
            User user = Thread.CurrentPrincipal as User;
            bool retVal = false;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.BlockGroupChat.ToString()))
            {
                if (user.Logged)
                {
                    //TODO fje
                    User u = loggedIn.Single(i => i.Email == blockEmail);
                    if (groupChat.Blocked.Single(i => i.Email == blockEmail) == null)
                    {
                        groupChat.Blocked.Add(u);
                       retVal = true;
                    }
                }else
                {
                    Console.WriteLine("User {0} is not logged in!", user.Name);
                }

            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }
            return retVal;
        }

        public bool BlockUser(string requestEmail, string blockEmail)
        {
            User user = Thread.CurrentPrincipal as User;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.BlockUser.ToString()))
            {
                if (user.Logged)
                {
                    if (user.Blocked.Single(i => i.Email == blockEmail) == null)
                    {
                        User blockUser = loggedIn.Single(i => i.Email == blockEmail);
                        if (blockUser != null)
                        {
                            user.Blocked.Add(blockUser);
                            return true;
                        }
                    }
                }

            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }

            return false;
        }

        public bool BlockUserFromRoom(string blockEmail, string roomName)
        {
            User user = Thread.CurrentPrincipal as User;
            bool retVal = false;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.BlockUserFromRoom.ToString()))
            {
                //TODO fje
                if (user.Logged)
                {
                    Room room = roomList.Single(r => r.Theme == roomName);
                    User u = loggedIn.Single(i => i.Email == blockEmail);
                    room.Blocked.Add(u);
                    retVal = true;
                }
                else
                {
                    Console.WriteLine("User {0} is not logged in!", user.Name);
                }
            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }

            return retVal;
        }

        public bool ChangePassword(string email, string oldPassowrd, string newPassword)
        {
            User user = Thread.CurrentPrincipal as User;
            bool retVal = false;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.ChangePassword.ToString()))
            {
                //TODO fje
                if (user.Logged)
                {
                    User u = loggedIn.Single(i => i.Email == email && i.Password == oldPassowrd);
                    u.Password = newPassword;
                    retVal = true;
                }
                else
                {
                    Console.WriteLine("User {0} is not logged in!", user.Name);
                }
            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }

            return retVal;
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

        public bool CreateRoom(string roomName)
        {
            User user = Thread.CurrentPrincipal as User;
            bool retVal = false;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.CreateRoom.ToString()))
            {
                //TODO fje
                if (user.Logged)
                {
                    if (roomList.Single(i => i.Theme == roomName) == null)
                    {
                        Room room = new Room(roomName);
                        roomList.Add(room);
                        retVal = true;
                    }
                }
                else
                {
                    Console.WriteLine("User {0} is not logged in!", user.Name);
                }
            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }
            return retVal;
        }

        public bool DeleteAdmin(string email)
        {
            User user = Thread.CurrentPrincipal as User;
            bool retVal = true;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.DeleteAdmin.ToString()))
            {
                //TODO fje
                if (user.Logged)
                {
                    User u = loggedIn.Single(i => i.Email == email);
                    u.Role = Roles.User;
                    retVal = true;
                }
                else
                {
                    Console.WriteLine("User {0} is not logged in!", user.Name);
                }
            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }

            return retVal;
        }

        public int LogIn(string email, string password)
        {
            //TODO ubaciti u listo ulogovanih

            Thread.CurrentPrincipal = loggedIn.Single(i => i.Email == email);

            User u = loggedIn.Single(i => i.Email == email);

            u.SecureCode = Guid.NewGuid();

            if (u != null)
            {
                if (password.Equals(u.Password))
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

                            MailMessage mm = new MailMessage(your_id, u.Email);
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
                        if (!u.Logged)
                        {
                            u.Logged = true;
                            loggedIn.Add(u);
                            return 1;
                        }
                        else
                        {
                            return -2; //neko je vec logovan sa tim nalogom
                        }
                        
                    }
                    
                }
                else
                {
                    return -1;    
                }
            }
            else
            {
                return -1;
            }

            
        }


        public bool LogOut(string email)
        {
            User user = Thread.CurrentPrincipal as User;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.LogOut.ToString()))
            {
                if (user.Logged)
                {
                    user.Logged = false;
                    loggedIn.Remove(user);
                    return true;
                }
                
            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }
            return false;
        }



        public bool Registration(string name, string sname, DateTime date, string gender, string email, string password)
        {
            bool exists = false;
            List<User> lista = new List<User>();

            User u1 = new User(name, sname, date, email, password, Roles.User, gender);
            lista.Add(u1);

            BinaryFormatter bf = new BinaryFormatter();

            if (!File.Exists("Users.dat"))   // ako fajl ne postoji nema sta da se proverava i samo se dodaje
            {
                Stream s = File.Open("Users.dat", FileMode.Create);
                try
                {
                    bf.Serialize(s, lista);
                    lista.Remove(u1);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                    throw;
                }
                finally
                {
                    s.Close();
                }

            }
            else
            {
                if (File.Exists("Users.dat"))
                {


                    // Open the file containing the data that you want to deserialize.
                    FileStream fs = new FileStream("Users.dat", FileMode.Open);

                    try
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        lista = (List<User>)formatter.Deserialize(fs);

                    }
                    catch (SerializationException e)
                    {
                        Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                        throw;
                    }
                    finally
                    {
                        fs.Close();
                    }

                    foreach (User u in lista)
                    {
                        if (u.Email == email)
                        {
                            exists = true;
                        }
                    }
                }
                if (exists != true)       // ako ne postoji upisi ga u fajl
                {
                    Stream s = File.Open("Users.dat", FileMode.Create);

                    try
                    {
                        lista.Add(u1);
                        bf.Serialize(s, lista);
                    }
                    catch (SerializationException e)
                    {
                        Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                        throw;
                    }
                    finally
                    {
                        s.Close();
                    }
                }
            }

            return exists;
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


        public bool RemoveBlockUser(string requestEmail, string unblockEmail)
        {
            User user = Thread.CurrentPrincipal as User;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.RemoveBlockUser.ToString()))
            {
                //TODO fje
                if (user.Logged)
                {
                    User blocked = user.Blocked.Single(i => i.Email == unblockEmail);
                    if (blocked != null)
                    {
                        user.Blocked.Add(blocked);
                        return true;
                    }
                }
            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }
            return false;
        }

        public bool RemoveBlockUserFromRoom(string unblockEmail, string roomName)
        {
            User user = Thread.CurrentPrincipal as User;
            bool retval = false;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.RemoveBlockRoom.ToString()))
            {
                //TODO fje
                if (user.Logged)
                {
                    Room room = roomList.Single(r => r.Theme == roomName);
                    User u = loggedIn.Single(i => i.Email == unblockEmail);
                    room.Blocked.Remove(u);
                    retval = true;
                }
                else
                {
                    Console.WriteLine("User {0} don't have permission!", user.Name);
                }
            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }

            return retval;
        }

        public int ResetPassword(string email)
        {
            User user = Thread.CurrentPrincipal as User;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.ResetPassword.ToString()))
            {
                //TODO fje

                if (user.Logged)
                {
                    string pass = Guid.NewGuid().ToString();
                    user.Password = Sha256encrypt(pass);
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

                        MailMessage mm = new MailMessage(your_id, user.Email);
                        mm.BodyEncoding = UTF8Encoding.UTF8;
                        mm.Subject = "CODE FOR FORUM";
                        mm.Body = "New password :" + pass;
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
                    return -2; // neulogovan je
                }

            }
            else
            {
                //TODO greske
            }

            return -1;
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

                    if (!user.Logged)
                    {
                        ok = true;
                        user.Logged = true;
                        loggedIn.Add(user);
                    }
                    
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

        public bool SendPrivateMessage(string firstEmail, string secondEmail, string message)
        {
            throw new NotImplementedException();
        }

        public bool SendGroupMessage(string email, string message)
        {
            throw new NotImplementedException();
        }

        public bool SendRoomMessage(string email, string roomName, string message)
        {
            throw new NotImplementedException();
        }

        public GroupChat GetGroupChat()
        {
            return groupChat;
        }

        public Room GetPrivateRoom(string roomName)
        {
            Room room = roomList.Single(r => r.Theme == roomName);
            return room;
        }

        public bool CloseRoom(string roomName, string email)
        {
            throw new NotImplementedException();
        }
    }
}
