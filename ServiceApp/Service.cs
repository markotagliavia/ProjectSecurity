using Contracts;
using ForumModels;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceApp
{
    
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.PerSession)]
    public class Service : IService
    {
        private User userOnSession;     //logged user
        public static string AppRoot;
        /// <summary>
        /// THis function notifies all logged clients about changes in forum
        /// </summary>
        private void NotifyAll()
        {
            ThreadPool.QueueUserWorkItem
            (
                delegate
                {
                    lock (ServiceModel.Instance.Clients)
                    {
                        List<string> disconnectedClientGuids = new List<string>();

                        foreach (KeyValuePair<string, IChatServiceCallback> client in ServiceModel.Instance.Clients)
                        {
                            try
                            {
                                client.Value.HandleGroupChat(ServiceModel.Instance.GroupChat);
                            }
                            catch (Exception)
                            {
                                // TODO: Better to catch specific exception types.                     

                                // If a timeout exception occurred, it means that the server
                                // can't connect to the client. It might be because of a network
                                // error, or the client was closed  prematurely due to an exception or
                                // and was unable to unregister from the server. In any case, we 
                                // must remove the client from the list of clients.

                                // Another type of exception that might occur is that the communication
                                // object is aborted, or is closed.

                                // Mark the key for deletion. We will delete the client after the 
                                // for-loop because using foreach construct makes the clients collection
                                // non-modifiable while in the loop.
                                disconnectedClientGuids.Add(client.Key);
                            }
                        }

                        foreach (string clientGuid in disconnectedClientGuids)
                        {
                            ServiceModel.Instance.Clients.Remove(clientGuid);
                        }
                    }
                }
            );
        }

        private void NotifyViewforAdmins()
        {
            ThreadPool.QueueUserWorkItem
            (
                delegate
                {
                    lock (ServiceModel.Instance.Clients)
                    {
                        List<string> disconnectedClientGuids = new List<string>();

                        foreach (KeyValuePair<string, IChatServiceCallback> client in ServiceModel.Instance.Clients)
                        {
                            try
                            {
                                client.Value.HandleGroupChat(ServiceModel.Instance.GroupChat);
                            }
                            catch (Exception)
                            {
                                // TODO: Better to catch specific exception types.                     

                                // If a timeout exception occurred, it means that the server
                                // can't connect to the client. It might be because of a network
                                // error, or the client was closed  prematurely due to an exception or
                                // and was unable to unregister from the server. In any case, we 
                                // must remove the client from the list of clients.

                                // Another type of exception that might occur is that the communication
                                // object is aborted, or is closed.

                                // Mark the key for deletion. We will delete the client after the 
                                // for-loop because using foreach construct makes the clients collection
                                // non-modifiable while in the loop.
                                disconnectedClientGuids.Add(client.Key);
                            }
                        }

                        foreach (string clientGuid in disconnectedClientGuids)
                        {
                            ServiceModel.Instance.Clients.Remove(clientGuid);
                        }
                    }
                }
            );
        }
        public void SerializeGroupChat(GroupChat gc)
        {
            Stream s = File.Open("GroupChat.dat", FileMode.Create);
            try
            {         
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(s, gc);
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

        }   // serialize GroupChat

        public GroupChat DeserializeGroupChat()
        {
            GroupChat gc = new GroupChat();

            FileStream fs = new FileStream("GroupChat.dat", FileMode.Open);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                gc = (GroupChat)formatter.Deserialize(fs);

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


            return gc;

        }        // deserialize GroupChat

        public void CloseupRoom(Room room)
        {
            AppRoot = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            if (File.Exists(room.Code.ToString() + ".dat"))
            {
                try
                {
                    System.IO.File.Delete(AppRoot + room.Code.ToString() + ".dat");
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }
        }             // apsolutna putanja 

        public void SerializeRoom(Room room)     // Serialize Room
        {
            Stream s = File.Open(room.Code.ToString() + ".dat", FileMode.Create);
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(s, room);
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

        public Room DeserializeRoom(Room room)   // DEserialize Room
        {
            Room pc1;

            FileStream fs = new FileStream(room.Code.ToString() + ".dat", FileMode.Open);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                pc1 = (Room)formatter.Deserialize(fs);

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

            return pc1;



        }

        public void SerializePrivateChat(PrivateChat pc)
        {        
                Stream s = File.Open(pc.Uid.ToString()+".dat", FileMode.Create);
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(s, pc);
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
        }             // serialize PrivateChat datoteka

        public PrivateChat DeserializePrivateChat(PrivateChat pc)
        {
            PrivateChat pc1;

            FileStream fs = new FileStream(pc.Uid.ToString()+".dat", FileMode.Open);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                pc1 = (PrivateChat)formatter.Deserialize(fs);

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

            return pc1;
        }    // deserialize PrivateChat datoteka

        public void SerializeUsers(List<User> lista)
        {
            Stream s = File.Open("Users.dat", FileMode.Create);
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
               /* foreach(User u in lista)
                {
                    Sha256encrypt(u.Password);
                }*/

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
        }        // serialize user datoteka

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
        }               // deserialize user datoteka

        public bool AddAdmin(string email)
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
            bool retVal = false;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.AddAdmin.ToString()))
            {
                //TODO fje
                if (user.Logged)
                {
                    List<User> lista = DeserializeUsers();

                    foreach (User u in lista)
                    {
                        if (u.Email == email)
                        {
                            u.Role = Roles.Admin;//?proveriti da li se menja i u group cjhatu obj
                            SerializeUsers(lista); // dodaj u fajl
                            retVal = true;
                            break;
                        }
                    }

                }
                else
                {
                    Console.WriteLine("User {0} is not logged in!", user.Name);
                    retVal = false;
                }
            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
                retVal = false;
            }
            return retVal;
        }                 // uzima iz datoteke User.dat  DONE

        public bool BlockGroupChat(string blockEmail)
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
            bool retVal = false;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.BlockGroupChat.ToString()))
            {
                if (user.Logged)
                {
                    if (user.Email != blockEmail)
                    {
                        //TODO fje
                        User u = ServiceModel.Instance.LoggedIn.Single(i => i.Email == blockEmail);
                        if (ServiceModel.Instance.GroupChat.Blocked.Single(i => i.Email == blockEmail) == null)
                        {
                            ServiceModel.Instance.GroupChat.Blocked.Add(u);
                            SerializeGroupChat(ServiceModel.Instance.GroupChat); // ser
                            retVal = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("You can not block yourself retard!");
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
        }     // DONE

        public bool BlockUser(string requestEmail, string blockEmail)   // blokira usera citajuci ga iz User.dat DONE
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
            bool retVal = false;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.BlockUser.ToString()))
            {
                if (user.Logged)
                {
                    if (requestEmail != blockEmail)
                    {
                        if (user.Blocked.Single(i => i.Email == blockEmail) == null)
                        {

                            List<User> lista = DeserializeUsers();
                            User blokator = new User(null, null, DateTime.Now, null, null, Roles.User, null);
                            User blokirani = new User(null, null, DateTime.Now, null, null, Roles.User, null);

                            foreach (User u in lista)
                            {
                                if (u.Email == requestEmail)
                                {
                                    blokator = u;

                                }
                                if (u.Email == blockEmail)
                                {
                                    blokirani = u;
                                }
                            }

                            if (blokirani.Email != null)      // ako je pronasao blokiranog u tom fajlu tj zna da postoji i ima ga
                            {
                                blokator.Blocked.Add(blokirani);
                                if (ServiceModel.Instance.GroupChat.Logged.Single(i => i.Email.Equals(user.Email)) != null)
                                {
                                    ServiceModel.Instance.GroupChat.Logged.Single(i => i.Email.Equals(user.Email)).Blocked.Add(ServiceModel.Instance.GroupChat.Logged.Single(i => i.Email.Equals(blockEmail)));
                                    SerializeGroupChat(ServiceModel.Instance.GroupChat);
                                }
                                SerializeUsers(lista); // dodaj u fajl
                                retVal = true;

                            }

                        }
                    }
                    else
                    {
                        Console.WriteLine("You can not block yourself retard!");
                    }
                }

            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
                retVal = false;
            }

            return retVal;
        }

        public bool BlockUserFromRoom(string blockEmail, string roomName)
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
            bool retVal = false;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.BlockUserFromRoom.ToString()))
            {
                //TODO fje
                if (user.Logged)
                {
                    if (user.Email != blockEmail)
                    {
                        Room room = ServiceModel.Instance.RoomList.Single(r => r.Theme == roomName);
                        User u = ServiceModel.Instance.LoggedIn.Single(i => i.Email == blockEmail);
                        room.Blocked.Add(u);

                        SerializeRoom(room);

                        retVal = true;
                    }
                    else
                    {
                        Console.WriteLine("You can not block yourself retard!");
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
        }  // DONE

        public bool ChangePassword(string email, string oldPassowrd, string newPassword)    // menjanje passworda i upisivanje promene u fajl DONE
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
            bool retVal = false;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.ChangePassword.ToString()))
            {
                //TODO fje
                if (user.Logged)
                {

                    User u = ServiceModel.Instance.LoggedIn.Single(i => i.Email == email && i.Password == oldPassowrd);
                    u.Password = newPassword;

                    List<User> lista = DeserializeUsers();
                    foreach (User ur in lista)
                    {
                        if (ur.Email == email && ur.Password==oldPassowrd)
                        {
                            ur.Password = newPassword;

                            SerializeUsers(lista); // upisi u fajl
                        }
            
                    }

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

        public int CreatePrivateChat(string firstEmail, string secondEmail)
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.CreatePrivateChat.ToString()))
            {
                if (user.Logged)
                {
                    User user2 = ServiceModel.Instance.LoggedIn.Single(i => i.Email == secondEmail);
                    if (user2 != null)
                    {
                        if (ServiceModel.Instance.PrivateChatList.Single(i => (i.User1.Equals(user.Email) && i.User2.Equals(user2.Email))) == null)
                        {
                            PrivateChat pc = new PrivateChat(user.Email, user2.Email);
                            SerializePrivateChat(pc);  // kreiraj fajl
                            ServiceModel.Instance.PrivateChatList.Add(pc);
                            return 0;
                        }
                        else
                        {
                            Console.WriteLine("Chat already exists!");
                            return -4;
                        }
                    }
                    else
                    {
                        Console.WriteLine("User {0} is not logged in!", secondEmail);
                        return -3;
                    }
                }
                else
                {
                    Console.WriteLine("User {0} is not logged in!", user.Name);
                    return -2; //nije logovan user
                }
            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }

            return -1;
        }        // kreiraj fajl sa privatnim chatom ako ne postoji DONE

        public bool CreateRoom(string roomName)
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
            bool retVal = false;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.CreateRoom.ToString()))
            {
                //TODO fje
                if (user.Logged)
                {
                    if (!ServiceModel.Instance.RoomList.Any(i => i.Theme == roomName))
                    {
                        Room room = new Room(roomName);
                        ServiceModel.Instance.RoomList.Add(room);
                        ServiceModel.Instance.GroupChat.ThemeRooms.Add(room.Theme);
                        SerializeRoom(room);
                        NotifyAll();
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
        }    // DONE

        public bool DeleteAdmin(string email)                 // promena uloge na obicnog sa admina i upisivanje u fajl DONE
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
            bool retVal = true;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.DeleteAdmin.ToString()))
            {
                //TODO fje
                if (user.Logged)
                {
                    User u = ServiceModel.Instance.LoggedIn.Single(i => i.Email == email);
                    u.Role = Roles.User;

                    List<User> lista = DeserializeUsers();
                    foreach (User ur in lista)
                    {
                        if (ur.Email == email)
                        {
                            ur.Role = Roles.User;
                            SerializeUsers(lista); // upisi u fajl
                        }

                    }
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

            List<User> lista = DeserializeUsers();

            if (lista == null)
            {
                return -2;
            }

            if (lista.Any(p => p.Email == email) == false)
            {
                return -2;
            }
            User u = lista.Single(i => i.Email == email);

            u.SecureCode = Guid.NewGuid();

            if (u != null)
            {
                if(string.Equals(password,u.Password))
                {
                  
                    if (!u.Verify)
                    {
                        //Thread.CurrentPrincipal = lista.Single(i => i.Email == email);
                        userOnSession = u;
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
                            ServiceModel.Instance.LoggedIn.Add(u);
                            ServiceModel.Instance.GroupChat.Logged.Add(u);
                            //Thread.CurrentPrincipal = lista.Single(i => i.Email == email);
                            userOnSession = u;
                            /*IChatServiceCallback callback = OperationContext.Current.GetCallbackChannel<IChatServiceCallback>();

                            Guid clientId = Guid.NewGuid();

                            if (callback != null)
                            {
                                lock (ServiceModel.Instance.Clients)
                                {
                                    ServiceModel.Instance.Clients.Add(u.Email, callback);
                                }
                                
                            }*/
                            NotifyAll();
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


        }    // ne treba da se upisuje promena u fajl

        public bool LogOut(string email)
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.LogOut.ToString()))
            {
                if (user.Logged)
                {
                    user.Logged = false;
                    ServiceModel.Instance.LoggedIn.Remove(user);
                    ServiceModel.Instance.GroupChat.Logged.Remove(user);
                    lock (ServiceModel.Instance.Clients)
                    {
                        if (ServiceModel.Instance.Clients.ContainsKey(user.Email))
                        {
                            ServiceModel.Instance.Clients.Remove(user.Email);
                            NotifyAll();
                        }
                    }
                    return true;
                }
                else
                {
                    Console.WriteLine("User {0} is already logged out!", user.Name);
                }

            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }
            return false;
        }                   // ne treba da se upisuje promena u fajl

        public bool Registration(string name, string sname, DateTime date, string gender, string email, string password)
        {
            bool exists = false;
            List<User> lista = new List<User>();

            User u1 = new User(name, sname, date, email, password, Roles.User, gender);
            User u2 = new User("Adminko", "Adminic", DateTime.Now, "forumblok@gmail.com", Sha256encrypt("sifra123"), Roles.Admin, "Male");
            User u3 = new User("Adminica", "Adminska", DateTime.Now, "forumblok1@gmail.com", Sha256encrypt("sifra1234"), Roles.Admin, "Female");
            lista.Add(u1);
            lista.Add(u2);
            lista.Add(u3);

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
        }  //Upise u fajl registruje koristnika DONE

        public bool RemoveBlockGroupChat(string unblockEmail)
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
            bool retVal = false;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.RemoveBlockGroupChat.ToString()))
            {
                //TODO fje
                if (user.Logged)
                {
                    if (user.Email != unblockEmail)
                    {
                        User u = ServiceModel.Instance.LoggedIn.Single(i => i.Email == unblockEmail);
                        if (u != null)
                        {
                            ServiceModel.Instance.GroupChat.Blocked.Remove(u);
                            SerializeGroupChat(ServiceModel.Instance.GroupChat); // ser
                            retVal = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("You can not unblock yourself!");
                    }
                }
                else
                {
                    Console.WriteLine("User {0} is already logged out!", user.Name);
                }
            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }

            return retVal;
        } // DONE

        public bool RemoveBlockUser(string requestEmail, string unblockEmail)        // unblock nalazi ga u fajlu i menja tu promenu DONE
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
            bool retVal = false;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.RemoveBlockUser.ToString()))
            {
                //TODO fje
                if (user.Logged)
                {
                    if (requestEmail != unblockEmail)
                    {
                        User blocked = user.Blocked.Single(i => i.Email == unblockEmail);
                        if (blocked != null)
                        {
                            List<User> lista = DeserializeUsers();
                            User blokator = new User(null, null, DateTime.Now, null, null, Roles.User, null);
                            User blokirani = new User(null, null, DateTime.Now, null, null, Roles.User, null);

                            foreach (User u in lista)
                            {
                                if (u.Email == requestEmail)
                                {
                                    blokator = u;

                                }
                                if (u.Email == unblockEmail)
                                {
                                    blokirani = u;
                                }
                            }

                            if (blokirani.Email != null)      // ako je pronasao blokiranog u tom fajlu tj zna da postoji i ima ga
                            {
                                blokator.Blocked.Remove(blokirani);
                                SerializeUsers(lista); // dodaj u fajl promene
                                retVal = true;

                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("You can not unblock yourself!");
                    }
                }
            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }
            return retVal;
        }

        public bool RemoveBlockUserFromRoom(string unblockEmail, string roomName)
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
            bool retval = false;

            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.RemoveBlockRoom.ToString()))
            {
                //TODO fje
                if (user.Logged)
                {
                    if (user.Email != unblockEmail)
                    {
                        Room room = ServiceModel.Instance.RoomList.Single(r => r.Theme == roomName);
                        User u = ServiceModel.Instance.LoggedIn.Single(i => i.Email == unblockEmail);
                        room.Blocked.Remove(u);
                        SerializeRoom(room);
                        retval = true;
                    }
                    else
                    {
                        Console.WriteLine("You can not unblock yourself retard!");
                    }
                }
                else
                {
                    Console.WriteLine("User {0} is already logged out!", user.Name);
                }
            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }

            return retval;
        } // DONE

        public int ResetPassword(string email)
        {
            //User user = Thread.CurrentPrincipal as User;
            //User user = userOnSession;
            /// audit both successfull and failed authorization checks
            // if (user.IsInRole(Permissions.ResetPassword.ToString()))
            // {
            //TODO fje

            // if (user.Logged)
            // {

            List<User> lista;
            lista = DeserializeUsers();
            if (lista.Any(x => x.Email.Equals(email)))
            {
                User user = lista.Single(x => x.Email.Equals(email));
                string pass = Guid.NewGuid().ToString().Substring(0, 30);
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
                    mm.Body = "New password : " + pass;
                    mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                    client.Send(mm);
                    lista.Single(x => x.Email.Equals(email)).Password = user.Password;
                    SerializeUsers(lista);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not end email\n\n" + e.ToString());
                }
                return 0;
            }
            else
            {
                return -1;
            }



              //  }
              //  else
               // {
               //     return -2; // neulogovan je
              //  }

           // }
           // else
           // {
                //TODO greske
           // }

            //return -1;
        }      // DONE

        public bool SendVerificationKey(string key)
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
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
                        user.Verify = true;
                        ServiceModel.Instance.LoggedIn.Add(user);
                        ServiceModel.Instance.GroupChat.Logged.Add(user);

                        List<User> lista = new List<User>();
                        lista= DeserializeUsers();
                        foreach(User u in lista)
                        {
                            if(u.Email==user.Email)
                            {
                                u.Verify = true;
                                u.SecureCode = user.SecureCode;
                            }

                        }

                        SerializeUsers(lista);

                        NotifyAll();
                        
                    }

                }

            }
            else
            {
                //TODO greske
            }

            return ok;

        }   //DONE

        public string Sha256encrypt(string phrase)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            System.Security.Cryptography.SHA256Managed sha256hasher = new System.Security.Cryptography.SHA256Managed();
            byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(phrase));
            return Convert.ToBase64String(hashedDataBytes);
        }  

        public bool SendPrivateMessage(string sendEmail, string reciveEmail, string message)  // DONE
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
            bool ok = false;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.SendPrivateMessage.ToString()))
            {
                if (user.Logged)
                {
                    Message m = new Message(message, sendEmail);
                    PrivateChat privateChat = ServiceModel.Instance.PrivateChatList.Single(i => (i.User1 == sendEmail && i.User2 == reciveEmail) || (i.User2 == sendEmail && i.User1 == reciveEmail));
                    if (privateChat == null)
                    {
                        PrivateChat newChat = new PrivateChat(sendEmail, reciveEmail);
                        newChat.Messages.Add(m);
                        ServiceModel.Instance.PrivateChatList.Add(newChat);

                        SerializePrivateChat(newChat);

                        ok = true;
                    }
                    else
                    {
                        privateChat.Messages.Add(m);

                        SerializePrivateChat(privateChat);

                        ok = true;
                    }
                }
                else
                {
                    Console.WriteLine("User {0} is already logged out!", user.Name);
                }

            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }

            return ok;
        }

        public bool SendGroupMessage(string userName, string message)
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
            bool ok = false;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.SendGroupMessage.ToString()))
            {
                if (user.Logged)
                {
                    Message m = new Message(message, userName);
                    ServiceModel.Instance.GroupChat.AllMessages.Add(m);
                    SerializeGroupChat(ServiceModel.Instance.GroupChat);   // serijal
                    NotifyAll();
                    ok = true;
                }
                else
                {
                    Console.WriteLine("User {0} is already logged out!", user.Name);
                }

            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }

            return ok;
        }    // DONE

        public bool SendRoomMessage(string userName, string roomName, string message)
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
            bool ok = false;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.SendRoomMessage.ToString()))
            {
                if (user.Logged)
                {
                    Message m = new Message(message, userName);
                    Room room = ServiceModel.Instance.RoomList.Single(r => r.Theme == roomName);
                    if(room != null)
                    {
                        room.AllMessages.Add(m);
                        SerializeRoom(room);
                        ok = true;
                    }
                    else
                    {
                        Console.WriteLine("Room {0} dose not exist", roomName);
                    }
                    
                }
                else
                {
                    Console.WriteLine("User {0} is already logged out!", user.Name);
                }

            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }

            return ok;
        }  // DONE

        public GroupChat GetGroupChat()
        {
            return ServiceModel.Instance.GroupChat;
        }   //vraca grupni chat sa singletona

        public Room GetPrivateRoom(string roomName)
        {
            Room room = ServiceModel.Instance.RoomList.Single(r => r.Theme == roomName);
            return room;
        } 

        public bool CloseRoom(string roomName)
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
            bool ok = false;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.CloseRoom.ToString()))
            {
                if (user.Logged)
                {
                    Room room = ServiceModel.Instance.RoomList.Single(r => r.Theme == roomName);
                    if(room != null)
                    {
                        ok = true;
                        CloseupRoom(room);   // Obrisi taj fajl kompletno
                        ServiceModel.Instance.RoomList.Remove(room);
                    }
                    else
                    {
                        Console.WriteLine("Room {0} dose not exist", roomName);
                    }
                }
                else
                {
                    Console.WriteLine("User {0} is already logged out!", user.Name);
                }

            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }

            return ok;
        }     // DONE

        public void KeepConnection()
        {
            // do nothing
        }   //odrzi konekciju u slucaju greske

        public void Subscribe(string email)
        {
            if (!ServiceModel.Instance.Clients.ContainsKey(email))
            {
                IChatServiceCallback callback = OperationContext.Current.GetCallbackChannel<IChatServiceCallback>();

                if (ServiceModel.Instance.LoggedIn.Any(i => i.Email.Equals(email)) == true)
                {
                    lock (ServiceModel.Instance.Clients)
                    {
                        ServiceModel.Instance.Clients.Add(ServiceModel.Instance.LoggedIn.Single(i => i.Email.Equals(email)).Email, callback);
                        userOnSession = ServiceModel.Instance.LoggedIn.Single(i => i.Email.Equals(email));
                    }
                }

            }

        }   //subscrubuje se user na grupni chat
    }
}
