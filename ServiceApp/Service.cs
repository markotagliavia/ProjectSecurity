
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
        public static string AppRoot = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);


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

        public void NotifyViewforRoom(Room room)
        {
            Room r = DeserializeRoom(room);
            ThreadPool.QueueUserWorkItem
            (
                delegate
                {
                    lock (ServiceModel.Instance.ClientsForThemeRoom)
                    {
                        List<string> disconnectedClientGuids = new List<string>();

                        if (ServiceModel.Instance.ClientsForThemeRoom.Any(i => i.Key.Equals(r.Theme)))
                        {
                            foreach (KeyValuePair<string, IChatServiceCallback> client in ServiceModel.Instance.ClientsForThemeRoom[r.Theme])
                            {
                                try
                                {
                                    client.Value.GetRoom(r);
                                }
                                catch (Exception)
                                {
                                    disconnectedClientGuids.Add(client.Key);
                                }
                            }

                            foreach (string clientGuid in disconnectedClientGuids)
                            {
                                ServiceModel.Instance.ClientsForThemeRoom.Single(i => i.Key.Equals(r.Theme)).Value.Remove(clientGuid);
                            }
                        }

                       
                    }
                }
            );
        }

        private void NotifyViewforAdmins()
        {
            List<User> lista = DeserializeUsers();
            ObservableCollection<User> obs = new ObservableCollection<User>();
            foreach (User u in lista)
            {
                obs.Add(u);
            }
            ThreadPool.QueueUserWorkItem
            (
                delegate
                {
                    lock (ServiceModel.Instance.ClientsForViewAdmins)
                    {
                        List<string> disconnectedClientGuids = new List<string>();

                        foreach (KeyValuePair<string, IChatServiceCallback> client in ServiceModel.Instance.ClientsForViewAdmins)
                        {
                            try
                            {
                                client.Value.AllUsers(obs);
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
                            ServiceModel.Instance.ClientsForViewAdmins.Remove(clientGuid);
                        }
                    }
                }
            );
        }

        public void SerializeFileChats(PrivateChat pc)
        {
            List<PrivateChat> lista = DeserializeChats();
            Stream s = File.Open("Chats.dat", FileMode.Create);
            try
            {
                if(lista!=null)
                {
                    foreach (PrivateChat r in lista)
                    {
                        if (r.Uid == pc.Uid)
                        {
                            lista.Remove(r); // izbaci staru i ubaci nove parametre sobe

                        }
                    }
                }else
                {
                    lista = new List<PrivateChat>();
                }
                

                lista.Add(pc);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(s, lista);

            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);

            }
            finally
            {
                s.Close();
            }

        }     // serijalizacija fajla sa privatnim chatovima
     
        public List<PrivateChat> DeserializeChats()
        {
            List<PrivateChat> pc = null;
            FileStream fs = new FileStream("Chats.dat", FileMode.OpenOrCreate);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                pc = (List<PrivateChat>)formatter.Deserialize(fs);

            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);

            }
            finally
            {
                fs.Close();
            }

            return pc;

        }

        public void SerializeFileRooms(Room room)
        {
            List<Room> lista = DeserializeFileRooms();
            Stream s = File.Open("Rooms.dat", FileMode.Create);
            try
            {
                if(lista!=null)
                {
                    foreach (Room r in lista)
                    {
                        if (r.Code == room.Code)
                        {
                            lista.Remove(r); // izbaci staru i ubaci nove parametre sobe

                        }
                    }

                }else
                {
                    lista = new List<Room>();
                }
               
              
                    lista.Add(room);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(s, lista);        
           
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
    
            }
            finally
            {
                s.Close();
            }
        }       //  serijalizacija fajla sa sobama

        public List<Room> DeserializeFileRooms()
        {
            List<Room> rooms=null;
            FileStream fs = new FileStream("Rooms.dat", FileMode.OpenOrCreate);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                rooms = (List<Room>)formatter.Deserialize(fs);

            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
        
            }
            finally
            {
                fs.Close();
            }


            return rooms;
        }

        public void SerializeGroupChat(GroupChat gc)
        {
            lock (ServiceModel.Instance.LockGroupChat)
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
                    
                }
                finally
                {
                    s.Close();
                }
            }

        }   // serialize GroupChat

        public GroupChat DeserializeGroupChat()
        {
            lock (ServiceModel.Instance.LockGroupChat)
            {
                GroupChat gc = new GroupChat();

                FileStream fs = new FileStream("GroupChat.dat", FileMode.OpenOrCreate);

                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    gc = (GroupChat)formatter.Deserialize(fs);

                }
                catch (SerializationException e)
                {
                    //Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                }
                finally
                {
                    fs.Close();
                }
                return gc;
            }
        }        // deserialize GroupChat

        public void CloseupRoom(Room room)
        {
           // AppRoot = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

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
            //NotifyViewforRoom(null);
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
                
            }
            finally
            {
                s.Close();
            }


        }

        public Room DeserializeRoom(Room room)   // DEserialize Room
        {
            Room pc1=null;

            FileStream fs = new FileStream(room.Code.ToString() + ".dat", FileMode.OpenOrCreate);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                pc1 = (Room)formatter.Deserialize(fs);

            }
            catch (SerializationException e)
            {
               // Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                
            }
            finally
            {
                fs.Close();
            }

            return pc1;



        }

        public void SerializePrivateChat(PrivateChat pc)
        {
            lock (ServiceModel.Instance.LockPrivateChat)
            {
                Stream s = File.Open(pc.Uid.ToString() + ".dat", FileMode.Create);
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(s, pc);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                  
                }
                finally
                {
                    s.Close();
                }
            }
        }             // serialize PrivateChat datoteka

        public PrivateChat DeserializePrivateChat(Guid code)
        {
            lock (ServiceModel.Instance.LockPrivateChat)
            {
                PrivateChat pc1 = null;

                FileStream fs = new FileStream(code.ToString() + ".dat", FileMode.OpenOrCreate);

                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    pc1 = (PrivateChat)formatter.Deserialize(fs);

                }
                catch (SerializationException e)
                {
                   // Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                    
                }
                finally
                {
                    fs.Close();
                }

                return pc1;
            }
        }    // deserialize PrivateChat datoteka

        public void SerializeUsers(List<User> lista)
        {
            lock (ServiceModel.Instance.LockUsers)
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
                   
                }
                finally
                {
                    s.Close();
                }
            }
        }        // serialize user datoteka

        public List<User> DeserializeUsers()
        {
            lock (ServiceModel.Instance.LockUsers)
            {
                List<User> lista = new List<User>();

                FileStream fs = new FileStream("Users.dat", FileMode.OpenOrCreate);

                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    lista = (List<User>)formatter.Deserialize(fs);

                }
                catch (SerializationException e)
                {
                   // Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                    
                }
                finally
                {
                    fs.Close();
                }


                return lista;
            }
        }               // deserialize user datoteka

        //-----------------------------------------------------------------------------------------------------------------

        //-----------------------------------------------------------------------------------------------------------------

        //-----------------------------------------------------------------------------------------------------------------

        public bool AddAdmin(string email)
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
            bool retVal = false;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.AddAdmin.ToString()) && userOnSession.Logged && ServiceModel.Instance.LoggedIn.Any(i => i.Email.Equals(userOnSession.Email)))
            {
                if (user.Logged)
                {
                    List<User> lista = DeserializeUsers();      // deser user
                    foreach (User u in lista)
                    {
                        if (u.Email.Equals(email))
                        {
                            u.Role = Roles.Admin;
                            ServiceModel.Instance.GroupChat = DeserializeGroupChat();  //deserialize group
                            if (ServiceModel.Instance.LoggedIn.Any(i => i.Email.Equals(email)))
                            {
                                ServiceModel.Instance.LoggedIn.Single(i => i.Email.Equals(email)).Role = Roles.Admin;
                            }
                            if (ServiceModel.Instance.GroupChat.Logged.Any(i => i.Email.Equals(email)))
                            {
                                ServiceModel.Instance.GroupChat.Logged.Single(i => i.Email.Equals(email)).Role = Roles.Admin;
                            }
                            SerializeUsers(lista); // ser user
                            SerializeGroupChat(ServiceModel.Instance.GroupChat); // ser group
                            NotifyAll();
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
        }                 //DONE

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
                        User u = ServiceModel.Instance.LoggedIn.Single(i => i.Email == blockEmail);
                        if (ServiceModel.Instance.GroupChat.Blocked.Single(i => i.Email == blockEmail) == null)
                        {
                            ServiceModel.Instance.GroupChat = DeserializeGroupChat();// deser
                            ServiceModel.Instance.GroupChat.Blocked.Add(u);
                            SerializeGroupChat(ServiceModel.Instance.GroupChat); // ser
                            retVal = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("You can not block yourself!");
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

        public bool BlockUser(string requestEmail, string blockEmail)   
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

                            List<User> lista = DeserializeUsers();  // deser user
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
                                ServiceModel.Instance.GroupChat = DeserializeGroupChat(); //deser group chat
                                if (ServiceModel.Instance.GroupChat.Logged.Single(i => i.Email.Equals(user.Email)) != null)
                                {   // ovde blokira iz liste logovanih a ne mora da bude logovan da bi blokirao??
                                    ServiceModel.Instance.GroupChat.Logged.Single(i => i.Email.Equals(user.Email)).Blocked.Add(ServiceModel.Instance.GroupChat.Logged.Single(i => i.Email.Equals(blockEmail)));
                                    SerializeGroupChat(ServiceModel.Instance.GroupChat);  //ser group chat
                                }
                                SerializeUsers(lista); // dodaj u fajl ser user
                                retVal = true;

                            }

                        }
                    }
                    else
                    {
                        Console.WriteLine("You can not block yourself!");
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
        }  //Blokira iz liste logovanih a sta ako nije logovan sto mora?

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

                        SerializeFileRooms(room);  

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
        }   //DONE

        public bool ChangePassword(string email, string oldPassowrd, string newPassword)   
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
        } //DONE

        public PrivateChat CreatePrivateChat(string firstEmail, string secondEmail)
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
                            SerializeFileChats(pc);
                            ServiceModel.Instance.PrivateChatList.Add(pc);
                            return pc;
                        }
                        else
                        {
                            Console.WriteLine("Chat already exists!");
                            return ServiceModel.Instance.PrivateChatList.Single(i => (i.User1.Equals(user.Email) && i.User2.Equals(user2.Email)));
                        }
                    }
                    else
                    {
                        Console.WriteLine("User {0} is not logged in!", secondEmail);
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine("User {0} is not logged in!", user.Name);
                    return null; //nije logovan user
                }
            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }

            return null;
        }        //DONE

        public void CreateRoom(string roomName)
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
                        SerializeFileRooms(room); // dodaj u listu soba fajl
                        SerializeGroupChat(ServiceModel.Instance.GroupChat);
                        
                        retVal = true;
                        NotifyAll();
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
           
        }        // DONE

        public bool DeleteAdmin(string email)                 
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
            bool retVal = true;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.DeleteAdmin.ToString()))
            {
                
                if (user.Logged)
                {
                    if (ServiceModel.Instance.LoggedIn.Any(i => i.Email == email))
                    {
                        User u = ServiceModel.Instance.LoggedIn.Single(i => i.Email == email);
                        u.Role = Roles.User;
                    }
                    

                    List<User> lista = DeserializeUsers(); //deser user
                    if (lista.Any(i => i.Email == email))
                    {
                        foreach (User ur in lista)
                        {
                            if (ur.Email == email)
                            {
                                ur.Role = Roles.User;
                                SerializeUsers(lista); // upisi u fajl
                            }

                        }
                        NotifyAll();
                        retVal = true;
                    }
                    else
                    {
                        return false;
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
        } //DONE

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

                            //popuni observable liste
                            ServiceModel.Instance.GroupChat = DeserializeGroupChat();  //done

                            List<PrivateChat> pc = DeserializeChats();
                            if (pc != null)
                            {
                                foreach (PrivateChat item in pc)
                                {
                                    if (item.User1 == email || item.User2 == email)
                                    {
                                        ServiceModel.Instance.PrivateChatList.Add(item);
                                    }
                                }
                            }



                            List<Room> rooms = DeserializeFileRooms();
                            if (rooms != null)
                            {
                                foreach (Room item in rooms)
                                {
                                    bool blocked = false;
                                    foreach (User user in item.Blocked)
                                    {
                                        if (user.Email == email)
                                        {
                                            blocked = true;

                                        }
                                    }
                                    if (blocked != true)
                                    {
                                        ServiceModel.Instance.RoomList.Add(item);
                                    }
                                }


                            }


                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Could not end email\n\n" + e.ToString());
                        }
                        return 0;
                    }
                    else
                    {
                        if (ServiceModel.Instance.LoggedIn.Any(i => i.Email.Equals(u.Email))==false)
                        {
                            ServiceModel.Instance.GroupChat = DeserializeGroupChat();
                            u.Logged = true;
                            ServiceModel.Instance.LoggedIn.Add(u);
                            ServiceModel.Instance.GroupChat.Logged.Add(u);
                            userOnSession = u;

                            //popuni observable liste
                            SerializeGroupChat(ServiceModel.Instance.GroupChat);

                            List<PrivateChat> pc = DeserializeChats();
                            if(pc!=null)
                            {
                                foreach (PrivateChat item in pc)
                                {
                                    if (item.User1 == email || item.User2 == email)
                                    {
                                        ServiceModel.Instance.PrivateChatList.Add(item);
                                    }
                                }
                            }
                            


                            List<Room> rooms = DeserializeFileRooms();
                            if(rooms!=null)
                            {
                                foreach (Room item in rooms)
                                {
                                    bool blocked = false;
                                    foreach (User user in item.Blocked)
                                    {
                                        if (user.Email == email)
                                        {
                                            blocked = true;

                                        }
                                    }
                                    if (blocked != true)
                                    {
                                        ServiceModel.Instance.RoomList.Add(item);
                                    }
                                }
                            }

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


        }    // ne treba da se upisuje promena u fajl  valjda done???

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
                    if (ServiceModel.Instance.LoggedIn.Any(x => x.Email.Equals(email)))
                    {
                        int index = ServiceModel.Instance.LoggedIn.IndexOf(ServiceModel.Instance.LoggedIn.Single(x => x.Email.Equals(email)));
                        ServiceModel.Instance.LoggedIn.RemoveAt(index);
                        ServiceModel.Instance.GroupChat = DeserializeGroupChat(); //deser users
                        

                        if (ServiceModel.Instance.GroupChat.Logged.Any(x => x.Email.Equals(email)))
                        {
                            index = ServiceModel.Instance.GroupChat.Logged.IndexOf(ServiceModel.Instance.GroupChat.Logged.Single(x => x.Email.Equals(email)));
                            ServiceModel.Instance.GroupChat.Logged.RemoveAt(index);
                            lock (ServiceModel.Instance.Clients)
                            {
                                if (ServiceModel.Instance.Clients.ContainsKey(user.Email))
                                {
                                    ServiceModel.Instance.Clients.Remove(user.Email);
                                    SerializeGroupChat(ServiceModel.Instance.GroupChat);
                                    NotifyAll();
                                }
                            }
                            return true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("User {0} is not logged!", user.Name);
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

            NotifyViewforAdmins();

            return exists;
        }   //DONE

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
                        ServiceModel.Instance.GroupChat = DeserializeGroupChat();  // deser
                        //User u = ServiceModel.Instance.LoggedIn.Single(i => i.Email == unblockEmail);
                        int index = ServiceModel.Instance.GroupChat.Blocked.IndexOf(ServiceModel.Instance.GroupChat.Blocked.Single(x => x.Email.Equals(unblockEmail)));
                        if (index > -1)
                        {
                            ServiceModel.Instance.GroupChat.Blocked.RemoveAt(index);
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

        public bool RemoveBlockUser(string requestEmail, string unblockEmail)        
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

                                ServiceModel.Instance.GroupChat = DeserializeGroupChat(); //deser group chat
                                if (ServiceModel.Instance.GroupChat.Logged.Single(i => i.Email.Equals(user.Email)) != null)
                                {                                                                                                                                //Logged???? zasto logged valjda blocked               
                                    ServiceModel.Instance.GroupChat.Logged.Single(i => i.Email.Equals(user.Email)).Blocked.Remove(ServiceModel.Instance.GroupChat.Logged.Single(i => i.Email.Equals(unblockEmail)));
                                    SerializeGroupChat(ServiceModel.Instance.GroupChat);  //ser group chat
                                }


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
        }   // Ista stvar ko sa block user

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
                        SerializeFileRooms(room);
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
        }  //DONE

        public int ResetPassword(string email)
        {
            List<User> lista;
            lista = DeserializeUsers();  //deser user
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
                    SerializeUsers(lista);  // ser user
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
        }       //DONE

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
                        ServiceModel.Instance.GroupChat = DeserializeGroupChat();
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
                        SerializeGroupChat(ServiceModel.Instance.GroupChat);
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
        }    //DONE

        public bool SendPrivateMessage(string sendEmail, string reciveEmail, string message)  
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
                        SerializeFileChats(newChat);
                        ok = true;
                    }
                    else
                    {
                        privateChat.Messages.Add(m);

                        SerializePrivateChat(privateChat);
                        SerializeFileChats(privateChat);
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
        } //DONE

        public void SendGroupMessage(string userName, string message)
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.SendGroupMessage.ToString()))
            {
                if (user.Logged)
                {
                    Message m = new Message(message, userName);

                    ServiceModel.Instance.GroupChat = DeserializeGroupChat(); //deseral

                    ServiceModel.Instance.GroupChat.AllMessages.Add(m);
                    SerializeGroupChat(ServiceModel.Instance.GroupChat);   // serijal
                    NotifyAll();
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

        }    // DONE

        public void SendRoomMessage(string userName, string roomName, string message)
        {
            //User user = Thread.CurrentPrincipal as User;
            User user = userOnSession;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.SendRoomMessage.ToString()))
            {
                if (user.Logged)
                {
                    Message m = new Message(message, userName);
                    Room room = ServiceModel.Instance.RoomList.Single(r => r.Theme == roomName);
                    if(room != null)
                    {
                        ServiceModel.Instance.RoomList.Single(r => r.Theme == roomName).AllMessages.Add(m);
                        SerializeRoom(ServiceModel.Instance.RoomList.Single(r => r.Theme == roomName));
                        NotifyViewforRoom(ServiceModel.Instance.RoomList.Single(r => r.Theme == roomName));
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
            
           
        }   //DONE


        //-----------------------------------------------------------------------------------------------------------------

        //-----------------------------------------------------------------------------------------------------------------

        public GroupChat GetGroupChat()
        {
            //ServiceModel.Instance.GroupChat = DeserializeGroupChat();
            return DeserializeGroupChat();
        }   //vraca grupni chat sa singletona (ne treba)

        public Room GetThemeRoom(string roomName,string email)
        {
            Room room = ServiceModel.Instance.RoomList.Single(r => r.Theme == roomName);
            if (ServiceModel.Instance.LoggedIn.Any(i => i.Email.Equals(email)))
            {
                userOnSession = ServiceModel.Instance.LoggedIn.Single(i => i.Email.Equals(email));
                if (room.Logged.Any(i => i.Email.Equals(email))==false)
                {
                    ServiceModel.Instance.RoomList.Single(r => r.Theme == roomName).Logged.Add(userOnSession);
                }
                
                SerializeRoom(ServiceModel.Instance.RoomList.Single(r => r.Theme == roomName));
                return ServiceModel.Instance.RoomList.Single(r => r.Theme == roomName);
            }
            else
            {
                return null;
            }
           
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
                        ServiceModel.Instance.GroupChat = DeserializeGroupChat();
                        ServiceModel.Instance.RoomList.Remove(room);
                        ServiceModel.Instance.GroupChat.ThemeRooms.Remove(room.Theme);
                        SerializeGroupChat(ServiceModel.Instance.GroupChat);
                        CloseupRoom(room);   // Obrisi taj fajl kompletno
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
        }      //DONE

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

        public void SubscribeAllUsers(string email)
        {
            if (!ServiceModel.Instance.ClientsForViewAdmins.ContainsKey(email))
            {
                IChatServiceCallback callback = OperationContext.Current.GetCallbackChannel<IChatServiceCallback>();

                if (ServiceModel.Instance.LoggedIn.Any(i => i.Email.Equals(email)) == true)
                {
                    lock (ServiceModel.Instance.ClientsForViewAdmins)
                    {
                        ServiceModel.Instance.ClientsForViewAdmins.Add(ServiceModel.Instance.LoggedIn.Single(i => i.Email.Equals(email)).Email, callback);
                        userOnSession = ServiceModel.Instance.LoggedIn.Single(i => i.Email.Equals(email));
                    }
                }

            }
        }  //ne treba

        public ObservableCollection<User> GetAllUsers(string email)
        {
            List<User> lista = DeserializeUsers();
            ObservableCollection<User> obs = new ObservableCollection<User>();
            foreach (User u in lista)
            {
                obs.Add(u);
            }

            if (ServiceModel.Instance.LoggedIn.Any(i => i.Email.Equals(email)))
            {
                userOnSession = ServiceModel.Instance.LoggedIn.Single(i => i.Email.Equals(email));
            }
            else
            {
                return null;
            }

            if(userOnSession.IsInRole(Permissions.GetAllUsers.ToString()))
            {
                if (userOnSession.Logged)
                {
                    if (ServiceModel.Instance.LoggedIn.Any(i => i.Email.Equals(userOnSession.Email)))
                    {
                        return obs;
                    }
                }
            }

            return null;
        } //ne treba

        public void Unsubscribe(string email)
        {

            if (ServiceModel.Instance.Clients.ContainsKey(email))
            {
                
                if (ServiceModel.Instance.LoggedIn.Any(i => i.Email.Equals(email)) == true)
                {
                    lock (ServiceModel.Instance.Clients)
                    {
                        if (ServiceModel.Instance.Clients.ContainsKey(email))
                        {
                            ServiceModel.Instance.Clients.Remove(email);
                        }
                    }
                }

            }
        }

        public PrivateChat GetPrivateChat(Guid code)
        {
            if (userOnSession.Logged)
            {
                if (ServiceModel.Instance.LoggedIn.Any(i => i.Email.Equals(userOnSession.Email)))
                {
                    if (userOnSession.IsInRole(Permissions.GetPrivateChat.ToString()))
                    {
                        PrivateChat pc  = DeserializePrivateChat(code);
                        return pc;
                    }
                    else
                    {
                        return null;
                    }
                    
                }
                else
                {
                    return null;
                }
                
            }
            else
            {
                return null;
            }
            
        }

        private void NotifyViewforPC(Guid uid)
        {
            throw new NotImplementedException();
        }

        public void SubscribeUserTheme(string email, string theme)
        {
            if (!ServiceModel.Instance.ClientsForThemeRoom.Any(i => i.Key.Equals(theme)))
            {
                Dictionary<string, IChatServiceCallback> pom =  new Dictionary<string, IChatServiceCallback>();
        
                lock (ServiceModel.Instance.ClientsForThemeRoom)
                {
                    ServiceModel.Instance.ClientsForThemeRoom.Add(theme,pom);
                }
            }
            
            if (!ServiceModel.Instance.ClientsForThemeRoom[theme].ContainsKey(email))
            {
                
                IChatServiceCallback callback = OperationContext.Current.GetCallbackChannel<IChatServiceCallback>();

                if (ServiceModel.Instance.LoggedIn.Any(i => i.Email.Equals(email)) == true)
                {
                  
                    lock (ServiceModel.Instance.ClientsForThemeRoom)
                    {
                        ServiceModel.Instance.ClientsForThemeRoom[theme].Add(ServiceModel.Instance.LoggedIn.Single(i => i.Email.Equals(email)).Email, callback);
                        userOnSession = ServiceModel.Instance.LoggedIn.Single(i => i.Email.Equals(email));
                    }
                }

            }
        }

        public void UnsubscribeUserTheme(string email, string theme)
        {
            if (ServiceModel.Instance.ClientsForThemeRoom.ContainsKey(theme))
            {
                if (ServiceModel.Instance.LoggedIn.Any(i => i.Email.Equals(email)) == true)
                {
                    lock (ServiceModel.Instance.ClientsForThemeRoom)
                    {
                        if (ServiceModel.Instance.ClientsForThemeRoom.Single(i => i.Key.Equals(theme)).Value.ContainsKey(email))
                        {
                            ServiceModel.Instance.ClientsForThemeRoom.Single(i => i.Key.Equals(theme)).Value.Remove(email);
                        }
                    }
                }

            }
        }

        public void UnsubscribeAllUsers(string email)
        {
            if (ServiceModel.Instance.ClientsForViewAdmins.ContainsKey(email))
            {
                if (ServiceModel.Instance.LoggedIn.Any(i => i.Email.Equals(email)) == true)
                {
                    lock (ServiceModel.Instance.ClientsForViewAdmins)
                    {
                        if (ServiceModel.Instance.ClientsForViewAdmins.ContainsKey(email))
                        {
                            ServiceModel.Instance.ClientsForViewAdmins.Remove(email);
                        }
                    }
                }

            }
        }

        public void LeaveRoom(string theme)
        {
            if (userOnSession.IsInRole(Permissions.SendGroupMessage.ToString()))
            {
                if (userOnSession.Logged)
                {
                    if (ServiceModel.Instance.RoomList.Any(i => i.Theme.Equals(theme)))
                    {
                        Room room = ServiceModel.Instance.RoomList.Single(i => i.Theme.Equals(theme));
                        if (room.Logged.Any(i => i.Email.Equals(userOnSession.Email)))
                        {
                            int index = room.Logged.IndexOf(room.Logged.Single(i => i.Email.Equals(userOnSession.Email)));
                            room.Logged.RemoveAt(index);
                            SerializeRoom(room);
                            NotifyViewforRoom(room);
                        }

                    }

                }
                else
                {
                    Console.WriteLine("User {0} is already logged out!", userOnSession.Name);
                }

            }
        }

        public void LeavePrivateChat(Guid code)
        {
            if (userOnSession.IsInRole(Permissions.SendGroupMessage.ToString()))
            {
                if (userOnSession.Logged)
                {
                    if (ServiceModel.Instance.PrivateChatList.Any(i => i.Uid.Equals(code)))
                    {
                        PrivateChat pc= ServiceModel.Instance.PrivateChatList.Single(i => i.Uid.Equals(code));
                        if (pc.User1.Equals(userOnSession.Email))
                        {
                            pc.User1logged = false;
                            SerializePrivateChat(pc);
                            NotifyViewforPC(pc.Uid);
                        }
                        else if (pc.User2.Equals(userOnSession.Email))
                        {
                            pc.User2logged = false;
                            SerializePrivateChat(pc);
                            NotifyViewforPC(pc.Uid);
                        }

                    }

                }
                else
                {
                    Console.WriteLine("User {0} is already logged out!", userOnSession.Name);
                }

            }
        }

        public void LogInTheme(string theme,string email)
        {
            Room room = ServiceModel.Instance.RoomList.Single(r => r.Theme == theme);

            if (ServiceModel.Instance.LoggedIn.Any(i => i.Email.Equals(email)))
            {
                NotifyViewforRoom(room);               
            }         
        }

        public void LogInPrivateChat(Guid code)
        {
            if (userOnSession.Logged)
            {
                if (ServiceModel.Instance.LoggedIn.Any(i => i.Email.Equals(userOnSession.Email)))
                {
                    if (userOnSession.IsInRole(Permissions.GetPrivateChat.ToString()))
                    {
                        PrivateChat pc = DeserializePrivateChat(code);

                        if (pc.User1.Equals(userOnSession.Email))
                        {
                            pc.User1logged = true;
                        }
                        else if (pc.User1.Equals(userOnSession.Email))
                        {
                            pc.User2logged = true;
                        }
                        
                        NotifyViewforPC(pc.Uid);
                        SerializePrivateChat(pc);
                       
                    }
                    

                }
                

            }
            
        }
    }
}
