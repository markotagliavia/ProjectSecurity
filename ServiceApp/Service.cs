
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
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceApp
{

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.PerSession)]
    public class Service : IService
    {
        private EncryptDecrypt aesCommander = new EncryptDecrypt();
        private User userOnSession;     //logged user
        private Guid sessionID;
        public static string AppRoot = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public Guid SessionID
        {
            get
            {
                return sessionID;
            }
            set
            {
                sessionID = value;
            }
        }

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
                if (lista != null)
                {
                    foreach (PrivateChat r in lista)
                    {
                        if (r.Uid == pc.Uid)
                        {
                            lista.Remove(r); // izbaci staru i ubaci nove parametre sobe
                        }
                    }
                }
                else
                {
                    lista = new List<PrivateChat>();
                }


                lista.Add(pc);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(s, lista);

            }
            catch (SerializationException e)
            {
                //  Console.WriteLine("Failed to serialize. Reason: " + e.Message);
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
                // Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
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
                if (lista != null)
                {
                    foreach (Room r in lista)
                    {
                        if (r.Code == room.Code)
                        {
                            lista.Remove(r); // izbaci staru i ubaci nove parametre sobe

                        }
                    }

                }
                else
                {
                    lista = new List<Room>();
                }

                lista.Add(room);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(s, lista);

            }
            catch (SerializationException e)
            {
                //   Console.WriteLine("Failed to serialize. Reason: " + e.Message);    
            }
            finally
            {
                s.Close();
            }
        }       //  serijalizacija fajla sa sobama

        public List<Room> DeserializeFileRooms()
        {
            List<Room> rooms = null;
            FileStream fs = new FileStream("Rooms.dat", FileMode.OpenOrCreate);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                rooms = (List<Room>)formatter.Deserialize(fs);

            }
            catch (SerializationException e)
            {
                // Console.WriteLine("Failed to deserialize. Reason: " + e.Message);        
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
                    //    Console.WriteLine("Failed to serialize. Reason: " + e.Message);                    
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
            if (ServiceModel.Instance.RoomList.Any(x => x.Theme.Equals(room.Theme)))
            {
                int index = ServiceModel.Instance.RoomList.IndexOf(ServiceModel.Instance.RoomList.Single(x => x.Theme.Equals(room.Theme)));
                ServiceModel.Instance.RoomList[index].Code = new Guid("00000000-0000-0000-0000-000000000000");
                index = -1;
                for (int i = 0; i < ServiceModel.Instance.GroupChat.ThemeRooms.Count; i++)
                {
                    if (ServiceModel.Instance.GroupChat.ThemeRooms[i].Equals(room.Theme)) index = i;
                }
                if (index != -1)
                {
                    ServiceModel.Instance.GroupChat.ThemeRooms.RemoveAt(index);
                }
            }
            else
            {
                return;
            }

            /* if (File.Exists(room.Code.ToString() + ".dat"))
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
             }*/
            NotifyViewforRoom(null);
        }

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
                //  Console.WriteLine("Failed to serialize. Reason: " + e.Message);               
            }
            finally
            {
                s.Close();
            }
        }

        public Room DeserializeRoom(Room room)   // DEserialize Room
        {
            Room pc1 = null;

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
                    // Console.WriteLine("Failed to serialize. Reason: " + e.Message);                 
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
                    bf.Serialize(s, lista);
                }
                catch (SerializationException e)
                {
                    //Console.WriteLine("Failed to serialize. Reason: " + e.Message);                   
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

        public bool AddAdmin(byte[] emailBytes, string emailHash)
        {
            string email = "";
            try
            {
                email = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, emailBytes));
                if (!emailHash.Equals(Sha256encrypt(email)))
                {
                    Audit.ModifiedMessageDanger();
                    return false;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return false; }

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

            if (retVal)
            {
                Audit.AddAdminSuccess(user.Email, email);
            }
            {
                Audit.AddAdminFailed(user.Email, email);
            }

            return retVal;
        }

        public bool DeleteAdmin(byte[] emailBytes, string emailHash)
        {
            string email = "";
            try
            {
                email = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, emailBytes));
                if (!emailHash.Equals(Sha256encrypt(email)))
                {
                    Audit.ModifiedMessageDanger();
                    return false;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return false; }
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

            if (retVal)
            {
                Audit.DeleteAdminSuccess(user.Email, email);
            }
            else
            {
                Audit.DeleteAdminfailed(user.Email, email);
            }

            return retVal;
        }

        public bool BlockGroupChat(byte[] blockEmailBytes, string blockEmailHash)
        {
            string blockEmail = "";
            try
            {
                blockEmail = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, blockEmailBytes));
                if (!blockEmailHash.Equals(Sha256encrypt(blockEmail)))
                {
                    Audit.ModifiedMessageDanger();
                    return false;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return false; }

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

            if (retVal)
            {
                Audit.BlockGroupChatSuccess(blockEmail);
            }
            {
                Audit.BlockGroupChatFailed(blockEmail);
            }

            return retVal;
        }

        public bool BlockUser(byte[] requestEmailBytes, byte[] blokEmailBytes, string requestEmailHash, string blockEmailHash)
        {
            string requestEmail = "";
            string blockEmail = "";
            try
            {
                requestEmail = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, requestEmailBytes));
                blockEmail = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, blokEmailBytes));
                if (!blockEmailHash.Equals(Sha256encrypt(blockEmail)) || !requestEmailHash.Equals(Sha256encrypt(requestEmail)))
                {
                    Audit.ModifiedMessageDanger();
                    return false;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return false; }

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

            if (retVal)
            {
                Audit.BlockUserSuccess(requestEmail, blockEmail);
            }
            {
                Audit.BlockUserFailed(requestEmail, blockEmail);
            }

            return retVal;
        }

        public bool BlockUserFromRoom(byte[] blokEmailBytes, byte[] roomNameBytes, string blockEmailHash, string roomNameHash)
        {
            string roomName = "";
            string blockEmail = "";
            try
            {
                roomName = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, roomNameBytes));
                blockEmail = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, blokEmailBytes));
                if (!blockEmailHash.Equals(Sha256encrypt(blockEmail)) || !roomNameHash.Equals(Sha256encrypt(roomName)))
                {
                    Audit.ModifiedMessageDanger();
                    return false;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return false; }

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

            if (retVal)
            {
                Audit.BlockUserFromRoomSuccess(blockEmail, roomName);
            }
            {
                Audit.BlockUserFromRoomFailed(blockEmail, roomName);
            }

            return retVal;
        }

        public bool ChangePassword(byte[] emailBytes, byte[] oldPasswordBytes, byte[] newPasswordBytes, string emailHash, string oldPassowrdHash, string newPasswordHash)
        {
            string email = "";
            string oldPassowrd = "";
            string newPassword = "";
            try
            {
                email = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, emailBytes));
                oldPassowrd = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, oldPasswordBytes));
                newPassword = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, newPasswordBytes));
                if (!emailHash.Equals(Sha256encrypt(email)) || !newPasswordHash.Equals(Sha256encrypt(newPassword)) || !oldPassowrdHash.Equals(Sha256encrypt(oldPassowrd)))
                {
                    Audit.ModifiedMessageDanger();
                    return false;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return false; }

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
                        if (ur.Email == email && ur.Password == oldPassowrd)
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

            if (retVal)
            {
                Audit.ChangePasswordSuccess(email);
            }
            {
                Audit.ChangePasswordFailed(email);
            }

            return retVal;
        }

        public PrivateChat CreatePrivateChat(byte[] firstEmailBytes, byte[] secondEmailBytes, string firstEmailHash, string secondEmailHash)
        {
            string firstEmail = "";
            string secondEmail = "";
            try
            {
                firstEmail = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, firstEmailBytes));
                secondEmail = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, secondEmailBytes));
                if (!firstEmailHash.Equals(Sha256encrypt(firstEmail)) || !secondEmailHash.Equals(Sha256encrypt(secondEmail)))
                {
                    Audit.ModifiedMessageDanger();
                    return null;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return null; }

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
                            Audit.CreatePrivateChatSuccess(firstEmail, secondEmail);
                            return pc;
                        }
                        else
                        {
                            Console.WriteLine("Chat already exists!");
                            Audit.CreatePrivateChatSuccess(firstEmail, secondEmail);
                            return ServiceModel.Instance.PrivateChatList.Single(i => (i.User1.Equals(user.Email) && i.User2.Equals(user2.Email)));
                        }
                    }
                    else
                    {
                        Console.WriteLine("User {0} is not logged in!", secondEmail);
                        Audit.CreatePrivateChatFailed(firstEmail, secondEmail);
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine("User {0} is not logged in!", user.Name);
                    Audit.CreatePrivateChatFailed(firstEmail, secondEmail);
                    return null; //nije logovan user
                }
            }
            else
            {
                //TODO greske
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }

            Audit.CreatePrivateChatFailed(firstEmail, secondEmail);
            return null;
        }

        public void CreateRoom(byte[] roomNameBytes, string roomNameHash)
        {
            string roomName = "";
            try
            {
                roomName = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, roomNameBytes));
                if (!roomNameHash.Equals(Sha256encrypt(roomName)))
                {
                    Audit.ModifiedMessageDanger();
                    return;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return; }

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

            if (retVal)
            {
                Audit.CreateRoomSuccess(roomName);
            }
            else
            {
                Audit.CreateRoomFailed(roomName);
            }

        }

        public int LogIn(byte[] cipherEmail, byte[] cipherPassword, string emailHash, string passwordHash)
        {
            List<User> lista = DeserializeUsers();
            string email = "";
            string password = "";
            try
            {
                email = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, cipherEmail));
                password = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, cipherPassword));
                if (!emailHash.Equals(Sha256encrypt(email)) || !passwordHash.Equals(Sha256encrypt(password)))
                {
                    Audit.ModifiedMessageDanger();
                    return -3;
                }
            }
            catch (Exception ex) { Audit.BadAesKey(); return -3; }

            if (lista == null)
            {
                Audit.LogInFailed(email);
                return -2;
            }

            if (lista.Any(p => p.Email == email) == false)
            {
                Audit.LogInFailed(email);
                return -2;
            }
            User u = lista.Single(i => i.Email == email);

            u.SecureCode = Guid.NewGuid();

            if (u != null)
            {
                if (string.Equals(password, u.Password))
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
                            Console.WriteLine("Could not send email\n\n" + e.ToString());
                        }
                        Audit.LogInFailed(email);
                        return 0;
                    }
                    else
                    {
                        if (ServiceModel.Instance.LoggedIn.Any(i => i.Email.Equals(u.Email)) == false)
                        {
                            ServiceModel.Instance.GroupChat = DeserializeGroupChat();
                            u.Logged = true;
                            Audit.LogInSuccess(email);
                            ServiceModel.Instance.LoggedIn.Add(u);
                            ServiceModel.Instance.GroupChat.Logged.Add(u);
                            userOnSession = u;

                            //popuni observable liste
                            SerializeGroupChat(ServiceModel.Instance.GroupChat);

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

                            NotifyAll();
                            return 1;
                        }
                        else
                        {
                            Audit.LogInFailed(email);
                            return -2; //neko je vec logovan sa tim nalogom
                        }

                    }

                }
                else
                {
                    Audit.LogInFailed(email);
                    return -1;
                }
            }
            else
            {
                Audit.LogInFailed(email);
                return -1;
            }


        }

        public bool LogOut(byte[] emailBytes, string emailHash)
        {
            string email = "";
            try
            {
                email = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, emailBytes));
                if (!emailHash.Equals(Sha256encrypt(email)))
                {
                    Audit.ModifiedMessageDanger();
                    return false;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return false; }

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
                            Audit.LogOutSuccess(email);
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
            Audit.LogOutFailed(email);
            return false;
        }

        public bool Registration(byte[] nameBytes, byte[] snameBytes, byte[] dateBytes, byte[] genderBytes, byte[] emailBytes, byte[] passwordBytes, string nameHash, string snameHash, string dateHash, string genderHash, string emailHash, string passwordHash)
        {
            string name = "";
            string sname = "";
            string gender = "";
            string email = "";
            DateTime date = new DateTime();
            string password = "";
            try
            {
                name = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, nameBytes));
                sname = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, snameBytes));
                gender = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, genderBytes));
                email = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, emailBytes));
                password = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, passwordBytes));
                try { date = DateTime.Parse(Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, dateBytes))); }
                catch (Exception e) { date = DateTime.Now; }

                if (!emailHash.Equals(Sha256encrypt(email)) || !nameHash.Equals(Sha256encrypt(name)) || !snameHash.Equals(Sha256encrypt(sname)) || !genderHash.Equals(Sha256encrypt(gender)) || !passwordHash.Equals(Sha256encrypt(password)))
                {
                    Audit.ModifiedMessageDanger();
                    return false;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return false; }

            bool exists = false, registrationSuccess = false;
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
                    registrationSuccess = true;
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
                        registrationSuccess = true;
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

            if (registrationSuccess)
            {
                Audit.RegistrationSuccess(email);
            }
            else
            {
                Audit.RegistrationFailed(email);
            }

            return exists;
        }   //DONE

        public bool RemoveBlockGroupChat(byte[] unblockEmailBytes, string unblockEmailHash)
        {
            string unblockEmail = "";
            try
            {
                unblockEmail = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, unblockEmailBytes));
                if (!unblockEmailHash.Equals(Sha256encrypt(unblockEmail)))
                {
                    Audit.ModifiedMessageDanger();
                    return false;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return false; }

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

            if (retVal)
            {
                Audit.RemoveBlockGroupChatSuccess(unblockEmail);
            }
            else
            {
                Audit.RemoveBlockGroupChatFailed(unblockEmail);
            }

            return retVal;
        }

        public bool RemoveBlockUser(byte[] requestEmailBytes, byte[] unblockEmailBytes, string requestEmailHash, string unblockEmailHash)
        {
            string requestEmail = "";
            string unblockEmail = "";
            try
            {
                unblockEmail = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, unblockEmailBytes));
                requestEmail = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, requestEmailBytes));
                if (!unblockEmailHash.Equals(Sha256encrypt(unblockEmail)) || !requestEmailHash.Equals(Sha256encrypt(requestEmail)))
                {
                    Audit.ModifiedMessageDanger();
                    return false;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return false; }

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

            if (retVal)
            {
                Audit.RemoveBlockUserSuccess(requestEmail, unblockEmail);
            }
            else
            {
                Audit.RemoveBlockUserFailed(requestEmail, unblockEmail);
            }

            return retVal;
        }

        public bool RemoveBlockUserFromRoom(byte[] unblockEmailBytes, byte[] roomNameBytes, string unblockEmailHash, string roomNameHash)
        {
            string unblockEmail = "";
            string roomName = "";
            try
            {
                unblockEmail = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, unblockEmailBytes));
                roomName = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, roomNameBytes));
                if (!unblockEmailHash.Equals(Sha256encrypt(unblockEmail)) || !roomNameHash.Equals(Sha256encrypt(roomName)))
                {
                    Audit.ModifiedMessageDanger();
                    return false;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return false; }

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

            if (retval)
            {
                Audit.RemoveBlockUserFromRoomSuccess(unblockEmail, roomName);
            }
            else
            {
                Audit.RemoveBlockUserFromRoomFailed(unblockEmail, roomName);
            }

            return retval;
        }

        public int ResetPassword(byte[] emailBytes, string emailHash)
        {
            string email = "";
            try
            {
                email = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, emailBytes));
                if (!emailHash.Equals(Sha256encrypt(email)))
                {
                    Audit.ModifiedMessageDanger();
                    return -1;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return -1; }

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
                    Audit.ResetPasswordFailed(email);
                    return -1;
                }
                Audit.ResetPasswordSuccess(email);
                return 0;
            }
            else
            {
                Audit.ResetPasswordFailed(email);
                return -1;
            }

        }

        public bool SendVerificationKey(byte[] keyBytes, string keyHash)
        {
            string key = "";
            try
            {
                key = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, keyBytes));
                if (!keyHash.Equals(Sha256encrypt(key)))
                {
                    Audit.ModifiedMessageDanger();
                    return false;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return false; }

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
                        lista = DeserializeUsers();
                        foreach (User u in lista)
                        {
                            if (u.Email == user.Email)
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
                Console.WriteLine("User {0} don't have permission!", user.Name);
            }

            if (ok)
            {
                Audit.SendVerificationKeySuccess();
            }
            else
            {
                Audit.SendVerificationKeyFailed();
            }

            return ok;

        }

        public string Sha256encrypt(string phrase)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            System.Security.Cryptography.SHA256Managed sha256hasher = new System.Security.Cryptography.SHA256Managed();
            byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(phrase));
            return Convert.ToBase64String(hashedDataBytes);
        }    //DONE

        public void SendPrivateMessage(byte[] sendEmailBytes, byte[] reciveEmailBytes, byte[] messageBytes, string sendEmailHash, string reciveEmailHash, string messageHash)
        {
            string sendEmail = "";
            string reciveEmail = "";
            string message = "";
            try
            {
                sendEmail = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, sendEmailBytes));
                reciveEmail = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, reciveEmailBytes));
                message = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, messageBytes));
                if (!sendEmailHash.Equals(Sha256encrypt(sendEmail)) || !reciveEmailHash.Equals(Sha256encrypt(reciveEmail)) || !messageHash.Equals(Sha256encrypt(message)))
                {
                    Audit.ModifiedMessageDanger();
                    return;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return; }

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

            if (ok)
            {
                Audit.SendPrivateMessageSuccess(sendEmail, reciveEmail);
            }
            else
            {
                Audit.SendPrivateMessageFailed(sendEmail, reciveEmail);
            }
        }

        public void SendGroupMessage(byte[] userNameBytes, byte[] messageBytes, string userNameHash, string messageHash)
        {
            string userName = "";
            string message = "";
            try
            {
                userName = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, userNameBytes));
                message = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, messageBytes));
                if (!userNameHash.Equals(Sha256encrypt(userName)) || !messageHash.Equals(Sha256encrypt(message)))
                {
                    Audit.ModifiedMessageDanger();
                    return;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return; }

            User user = userOnSession;
            bool sendSuccess = false;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.SendGroupMessage.ToString()))
            {
                if (user.Logged)
                {
                    Message m = new Message(message, userName);

                    ServiceModel.Instance.GroupChat = DeserializeGroupChat(); //deseral

                    ServiceModel.Instance.GroupChat.AllMessages.Add(m);
                    sendSuccess = true;
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

            if (sendSuccess)
            {
                Audit.SendGroupMessageSuccess(userName);
            }
            else
            {
                Audit.SendGroupMessageFailed(userName);
            }

        }

        public void SendRoomMessage(byte[] userNameBytes, byte[] roomNameBytes, byte[] messageBytes, string userNameHash, string roomNameHash, string messageHash)
        {
            string userName = "";
            string roomName = "";
            string message = "";
            try
            {
                userName = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, userNameBytes));
                roomName = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, roomNameBytes));
                message = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, messageBytes));
                if (!userNameHash.Equals(Sha256encrypt(userName)) || !roomNameHash.Equals(Sha256encrypt(roomName)) || !messageHash.Equals(Sha256encrypt(message)))
                {
                    Audit.ModifiedMessageDanger();
                    return;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return; }

            User user = userOnSession;
            bool sendSuccess = false;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.SendRoomMessage.ToString()))
            {
                if (user.Logged)
                {
                    Message m = new Message(message, userName);
                    Room room = ServiceModel.Instance.RoomList.Single(r => r.Theme == roomName);
                    if (room != null)
                    {
                        ServiceModel.Instance.RoomList.Single(r => r.Theme == roomName).AllMessages.Add(m);
                        sendSuccess = true;
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

            if (sendSuccess)
            {
                Audit.SendRoomMessageSuccess(userName, roomName);
            }
            else
            {
                Audit.SendRoomMessageFailed(userName, roomName);
            }

        }


        //-----------------------------------------------------------------------------------------------------------------

        //-----------------------------------------------------------------------------------------------------------------

        public GroupChat GetGroupChat()
        {
            if (userOnSession != null)
            {
                if (ServiceModel.Instance.LoggedIn.Any(i => i.Email.Equals(userOnSession.Email)))
                {
                    if (userOnSession.IsInRole(Permissions.GetGroupChat.ToString()))
                    {
                        return DeserializeGroupChat();
                    }
                }

            }

            return null;
        }

        public Room GetThemeRoom(byte[] roomNameBytes, byte[] emailBytes, string roomNameHash, string emailHash)
        {
            string roomName = "";
            string email = "";
            try
            {
                roomName = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, roomNameBytes));
                email = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, emailBytes));
                if (!roomNameHash.Equals(Sha256encrypt(roomName)) || !emailHash.Equals(Sha256encrypt(email)))
                {
                    Audit.ModifiedMessageDanger();
                    return null;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return null; }

            Room room = ServiceModel.Instance.RoomList.Single(r => r.Theme == roomName);
            if (ServiceModel.Instance.LoggedIn.Any(i => i.Email.Equals(email)))
            {
                userOnSession = ServiceModel.Instance.LoggedIn.Single(i => i.Email.Equals(email));
                if (room.Logged.Any(i => i.Email.Equals(email)) == false)
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

        public bool CloseRoom(byte[] roomNameBytes, string roomNameHash)
        {
            string roomName = "";
            try
            {
                roomName = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, roomNameBytes));
                if (!roomNameHash.Equals(Sha256encrypt(roomName)))
                {
                    Audit.ModifiedMessageDanger();
                    return false;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return false; }

            User user = userOnSession;
            bool ok = false;
            /// audit both successfull and failed authorization checks
            if (user.IsInRole(Permissions.CloseRoom.ToString()))
            {
                if (user.Logged)
                {
                    Room room = ServiceModel.Instance.RoomList.Single(r => r.Theme == roomName);
                    if (room != null)
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

            if (ok)
            {
                Audit.CloseRoomSuccess(roomName);
            }
            else
            {
                Audit.CloseRoomFailed(roomName);
            }

            return ok;
        }

        public void KeepConnection()
        {
            // do nothing
        }   //odrzi konekciju u slucaju greske

        public void Subscribe(byte[] emailBytes, string emailHash)
        {
            string email = "";
            try
            {
                email = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, emailBytes));
                if (!emailHash.Equals(Sha256encrypt(email)))
                {
                    Audit.ModifiedMessageDanger();
                    return;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return; }

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

        }

        public void SubscribeAllUsers(byte[] emailBytes, string emailHash)
        {
            string email = "";
            try
            {
                email = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, emailBytes));
                if (!emailHash.Equals(Sha256encrypt(email)))
                {
                    Audit.ModifiedMessageDanger();
                    return;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return; }

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
        }

        public ObservableCollection<User> GetAllUsers(byte[] emailBytes, string emailHash)
        {
            string email = "";
            try
            {
                email = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, emailBytes));
                if (!emailHash.Equals(Sha256encrypt(email)))
                {
                    Audit.ModifiedMessageDanger();
                    return null;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return null; }

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

            if (userOnSession.IsInRole(Permissions.GetAllUsers.ToString()))
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
        }

        public void Unsubscribe(byte[] emailBytes, string emailHash)
        {
            string email = "";
            try
            {
                email = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, emailBytes));
                if (!emailHash.Equals(Sha256encrypt(email)))
                {
                    Audit.ModifiedMessageDanger();
                    return;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return; }

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

        public PrivateChat GetPrivateChat(byte[] codeByte, string codeHash)
        {
            string code1 = "";
            try
            {
                code1 = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, codeByte));
                if (!codeHash.Equals(Sha256encrypt(code1)))
                {
                    Audit.ModifiedMessageDanger();
                    return null;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return null; }

            Guid code = new Guid(code1);

            if (userOnSession.Logged)
            {
                if (ServiceModel.Instance.LoggedIn.Any(i => i.Email.Equals(userOnSession.Email)))
                {
                    if (userOnSession.IsInRole(Permissions.GetPrivateChat.ToString()))
                    {
                        PrivateChat pc = DeserializePrivateChat(code);
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

        public void SubscribeUserTheme(byte[] emailBytes, byte[] themeBytes, string emailHash, string themeHash)
        {
            string email = "";
            string theme = "";
            try
            {
                email = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, emailBytes));
                theme = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, themeBytes));
                if (!emailHash.Equals(Sha256encrypt(email)) || !themeHash.Equals(Sha256encrypt(theme)))
                {
                    Audit.ModifiedMessageDanger();
                    return;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return; }

            if (!ServiceModel.Instance.ClientsForThemeRoom.Any(i => i.Key.Equals(theme)))
            {
                Dictionary<string, IChatServiceCallback> pom = new Dictionary<string, IChatServiceCallback>();

                lock (ServiceModel.Instance.ClientsForThemeRoom)
                {
                    ServiceModel.Instance.ClientsForThemeRoom.Add(theme, pom);
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

        public void UnsubscribeUserTheme(byte[] emailBytes, byte[] themeBytes, string emailHash, string themeHash)
        {
            string email = "";
            string theme = "";
            try
            {
                email = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, emailBytes));
                theme = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, themeBytes));
                if (!emailHash.Equals(Sha256encrypt(email)) || !themeHash.Equals(Sha256encrypt(theme)))
                {
                    Audit.ModifiedMessageDanger();
                    return;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return; }

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

        public void UnsubscribeAllUsers(byte[] emailBytes, string emailHash)
        {
            string email = "";
            try
            {
                email = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, emailBytes));
                if (!emailHash.Equals(Sha256encrypt(email)))
                {
                    Audit.ModifiedMessageDanger();
                    return;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return; }

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

        public void LeaveRoom(byte[] themeBytes, string themeHash)
        {
            string theme = "";
            try
            {
                theme = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, themeBytes));
                if (!themeHash.Equals(Sha256encrypt(theme)))
                {
                    Audit.ModifiedMessageDanger();
                    return;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return; }

            if (userOnSession.IsInRole(Permissions.SendGroupMessage.ToString()))
            {
                if (userOnSession.Logged)
                {
                    if (ServiceModel.Instance.RoomList.Any(i => i.Theme.Equals(theme)))
                    {

                        if (ServiceModel.Instance.RoomList.Single(i => i.Theme.Equals(theme)).Logged.Any(i => i.Email.Equals(userOnSession.Email)))
                        {
                            int index = ServiceModel.Instance.RoomList.Single(i => i.Theme.Equals(theme)).Logged.IndexOf(ServiceModel.Instance.RoomList.Single(i => i.Theme.Equals(theme)).Logged.Single(i => i.Email.Equals(userOnSession.Email)));
                            ServiceModel.Instance.RoomList.Single(i => i.Theme.Equals(theme)).Logged.RemoveAt(index);
                            SerializeRoom(ServiceModel.Instance.RoomList.Single(i => i.Theme.Equals(theme)));
                            NotifyViewforRoom(ServiceModel.Instance.RoomList.Single(i => i.Theme.Equals(theme)));
                        }

                    }

                }
                else
                {
                    Console.WriteLine("User {0} is already logged out!", userOnSession.Name);
                }

            }
        }

        public void LeavePrivateChat(byte[] codeByte, string codeHash)
        {
            string code1 = "";
            try
            {
                code1 = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, codeByte));
                if (!codeHash.Equals(Sha256encrypt(code1)))
                {
                    Audit.ModifiedMessageDanger();
                    return;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return; }

            Guid code = new Guid(code1);

            if (userOnSession.IsInRole(Permissions.SendGroupMessage.ToString()))
            {
                if (userOnSession.Logged)
                {
                    if (ServiceModel.Instance.PrivateChatList.Any(i => i.Uid.Equals(code)))
                    {
                        PrivateChat pc = ServiceModel.Instance.PrivateChatList.Single(i => i.Uid.Equals(code));
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

        public void LogInTheme(byte[] themeBytes, byte[] emailBytes, string themeHash, string emailHash)
        {
            string email = "";
            string theme = "";
            try
            {
                email = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, emailBytes));
                theme = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, themeBytes));
                if (!emailHash.Equals(Sha256encrypt(email)) || !themeHash.Equals(Sha256encrypt(theme)))
                {
                    Audit.ModifiedMessageDanger();
                    return;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return; }

            Room room = ServiceModel.Instance.RoomList.Single(r => r.Theme == theme);

            if (ServiceModel.Instance.LoggedIn.Any(i => i.Email.Equals(email)))
            {
                NotifyViewforRoom(room);
            }
        }

        public void LogInPrivateChat(byte[] codeByte, string codeHash)
        {
            string code1 = "";
            try
            {
                code1 = Encoding.ASCII.GetString(aesCommander.Decrypt(ServiceModel.Instance.RSA.SessionKeys[SessionID].SymmetricKey, codeByte));
                if (!codeHash.Equals(Sha256encrypt(code1)))
                {
                    Audit.ModifiedMessageDanger();
                    return;
                }
            }
            catch (Exception e) { Audit.BadAesKey(); return; }

            Guid code = new Guid(code1);

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


        /// <summary>
        /// If aes key is decrypted with rsa private key, everything is ok, i onther case, nots
        /// </summary>
        /// <param name="crypted"></param>
        /// <returns></returns>
        public bool SendSessionKey(byte[] crypted)
        {
            if (ServiceModel.Instance.RSA.SetSymmetricKey(this.SessionID, crypted))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// this function provide public RSA key to Client
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        RSAParameters IService.GetPublicKey(Guid code)
        {
            this.SessionID = code;
            RSAParameters r = ServiceModel.Instance.RSA.Generate(code);
            return r;
        }

        public void SessionKey(Guid code)
        {
            SessionID = code;
        }
    }
}
