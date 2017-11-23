using ForumModels;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for PrivateRoom.xaml
    /// </summary>
    public partial class ThemeRoom : Window
    {
        private WCFClient proxy;
        private ForumModels.Room room;
        private PrivateChat pc;
        private string email;
        private string theme;
        private ObservableCollection<string> logged;
        private ObservableCollection<Message> msg;
        private InstanceContext instanceContext;
        private EncryptDecrypt aesCommander = new EncryptDecrypt();
        private int i;

        public ThemeRoom(WCFClient proxy, string theme, string email,int i)
        {
            DataContext = this;
            this.proxy = proxy;
            this.i = i;
            this.email = email;
            this.theme = theme;
            logged = new ObservableCollection<string>();
            msg = new ObservableCollection<Message>();
            InitializeComponent();

            if (i == 1)
            {
                label.Content = theme;
                blockUserButton.IsEnabled = false;
                removeUserButton.IsEnabled = false;
            }
            else
            {
                this.Title = "Private chat";
                label.Content = "Private chat";
                blockUserButton.IsEnabled = false;
                removeUserButton.Visibility = Visibility.Hidden;
                closeRoomButton.Visibility = Visibility.Hidden;
                leaveRoomButton.Content = "Leave private chat";
               
            }
            
            loggedAsLabel1.Content = $"You are logged as {email}";           
        }

        public ObservableCollection<string> Logged
        {
            get
            {
                return logged;
            }
            set { logged = value; }
        }

        public ObservableCollection<Message> Msg
        {
            get
            {
                return msg;
            }
            set { msg = value; }
        }

        /// <summary>
        /// sending message to theme room
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enterMsgButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] emailInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, this.email);
            string emailHash = Sha256encrypt(this.email);
            byte[] msgInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, entryMessageTextbox.Text);
            string msgHash = Sha256encrypt(entryMessageTextbox.Text);

            if (i == 1)
            {
                byte[] roomInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, room.Theme);
                string roomHash = Sha256encrypt(room.Theme);
                if (!room.Blocked.Any(x => x.Email.Equals(email)))
                {
                    if (!String.IsNullOrWhiteSpace(entryMessageTextbox.Text))
                    {
                        if (room.Logged.Single(x => x.Email.Equals(email)).Logged)
                        {
                            if (room.Logged.Single(x => x.Email.Equals(email)) != null)
                            {
                                proxy.SendRoomMessage(emailInBytes, roomInBytes, msgInBytes, emailHash, roomHash, msgHash);
                                entryMessageTextbox.Clear();
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("You are banned from theme room chat and cannot write messages!");
                }
            }
            else
            {
                byte[] receiveMail;
                string receiveHash;
                if (pc.User1.Equals(this.email))
                {
                    receiveMail = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, pc.User2);
                    receiveHash = Sha256encrypt(pc.User2); 

                }
                else if (pc.User2.Equals(this.email))
                {
                    receiveMail = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, pc.User1);
                    receiveHash = Sha256encrypt(pc.User1);
                }
                else
                {
                    MessageBox.Show("You are admin and cannot send messages.");
                    return;
                }
                byte[] guidInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, this.theme);
                string guidHash = Sha256encrypt(this.theme);
                if (!String.IsNullOrWhiteSpace(entryMessageTextbox.Text))
                {
                    
                    proxy.SendPrivateMessage(emailInBytes, receiveMail,msgInBytes, emailHash, receiveHash,msgHash);
                    entryMessageTextbox.Clear();
                     
                }
                else
                {
                    return;
                }
            }
            
        }

        /// <summary>
        /// Leave theme room
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void leaveRoomButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// block another user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void blockUserButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] emailRequestInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, this.email);
            string emailRequestHash = Sha256encrypt(this.email);
            byte[] userToBlockInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, loggedUsersListBox.SelectedItem.ToString());
            string userToBlockHash = Sha256encrypt(loggedUsersListBox.SelectedItem.ToString());
            //if (i == 1)
           // {
                if (loggedUsersListBox.SelectedIndex != -1)
                {
                    if (!email.Equals(loggedUsersListBox.SelectedItem.ToString()))
                    {
                        if (((Button)sender).Content.Equals("Block user"))
                        {
                            proxy.BlockUser(emailRequestInBytes,userToBlockInBytes,emailRequestHash, userToBlockHash);
                        }
                        else
                        {
                            proxy.RemoveBlockUser(emailRequestInBytes, userToBlockInBytes, emailRequestHash, userToBlockHash);
                        }
                    }
                }
          /*  }
            else
            {
                byte[] roomInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, pc.Uid.ToString());
                string roomHash = Sha256encrypt(pc.Uid.ToString());
                if (loggedUsersListBox.SelectedIndex != -1)
                {
                    if (!email.Equals(loggedUsersListBox.SelectedItem.ToString()))
                    {
                        if (((Button)sender).Content.Equals("Block"))
                        {
                            proxy.BlockUser(emailRequestInBytes, userToBlockInBytes, emailRequestHash, userToBlockHash);
                        }
                        else
                        {
                            proxy.RemoveBlockUser(emailRequestInBytes, userToBlockInBytes, emailRequestHash, userToBlockHash);
                        }
                    }
                }
            }*/
            
        }

        /// <summary>
        /// Close theme room
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeRoomButton_Click(object sender, RoutedEventArgs e)
        {
      
            byte[] roomInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, room.Theme);
            string roomHash = Sha256encrypt(room.Theme);
            if (room.Logged.Any(x => x.Email.Equals(email)))
            {
                if (room.Logged.Single(x => x.Email.Equals(email)).Role == Roles.Admin)
                {
                    proxy.CloseRoom(roomInBytes, roomHash);
                }
            }
           
           
            this.Close();
        }

        private void ChatServiceCallback_ThemeNotified(object sender, ThemeRoomEventArgs e)
        {
            if (e.Room == null)
            {
                MessageBox.Show("You are blocked from rooms");
                this.Close();
            }
            else
            {
                room = e.Room;
                Logged.Clear();
                foreach (User u in room.Logged)
                {
                    Logged.Add(u.Email);
                }

                Msg.Clear();
                foreach (Message m in room.AllMessages)
                {
                    Msg.Add(m);
                }
                allMessagesScrollViewer.ScrollToBottom();

                if (room.Logged.Single(x => x.Email.Equals(email)).Role == Roles.Admin)
                {
                    removeUserButton.Visibility = Visibility.Visible;
                    closeRoomButton.Visibility = Visibility.Visible;
                    closeRoomButton.IsEnabled = true;
                }
                else
                {
                    removeUserButton.Visibility = Visibility.Hidden;
                    closeRoomButton.Visibility = Visibility.Hidden;
                }

                if (room.Code.Equals(new Guid("00000000-0000-0000-0000-000000000000")))
                {
                    this.Close();
                }

            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChatServiceCallback chatServiceCallback = new ChatServiceCallback();
            chatServiceCallback.ThemeNotified += ChatServiceCallback_ThemeNotified;
            chatServiceCallback.PrivateChatNotified += ChatServiceCallback_PrivateChatNotified;
            instanceContext = new InstanceContext(chatServiceCallback);

            EndpointAddress adr = this.proxy.Address;
            NetTcpBinding tcp = this.proxy.Binding;
            Guid guid = this.proxy.Guid;
            GenerateAesKey aes = this.proxy.Aes;
            this.proxy.InstanceContext = instanceContext;
            this.proxy.Abort();
            this.proxy = new WCFClient(instanceContext, tcp, adr);
            this.proxy.Guid = guid;
            this.proxy.Aes = aes;
            this.proxy.SessionKey(guid);
            Logged.Clear();
            Msg.Clear();
            if (i == 1)
            {
                byte[] emailInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, this.email);
                string emailHash = Sha256encrypt(this.email);
                byte[] roomInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, this.theme);
                string roomHash = Sha256encrypt(this.theme);
                try
                {
                    this.proxy.SubscribeUserTheme(emailInBytes, roomInBytes, emailHash, roomHash);
                }
                catch
                {
                    // TODO: Handle exception.
                }
                this.room = this.proxy.GetThemeRoom(roomInBytes, emailInBytes, roomHash, emailHash);
                this.proxy.LogInTheme(roomInBytes, emailInBytes, roomHash, emailHash);
                foreach (User u in this.room.Logged)
                {
                    Logged.Add(u.Email);
                }


                foreach (Message m in this.room.AllMessages)
                {
                    Msg.Add(m);
                }
                allMessagesScrollViewer.ScrollToBottom();

                if (this.room.Logged.Single(x => x.Email.Equals(email)).Role == Roles.Admin)
                {
                    removeUserButton.Visibility = Visibility.Visible;
                    closeRoomButton.Visibility = Visibility.Visible;
                    closeRoomButton.IsEnabled = true;
                }
                else
                {
                    removeUserButton.Visibility = Visibility.Hidden;
                    closeRoomButton.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                byte[] emailInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, this.email);
                string emailHash = Sha256encrypt(this.email);
                byte[] guidInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, this.theme);
                string guidHash = Sha256encrypt(this.theme);
                try
                {
                    this.proxy.SubscribeUserChat(emailInBytes,guidInBytes,emailHash,guidHash);
                }
                catch
                {
                    // TODO: Handle exception.
                }
             
                

                this.pc = this.proxy.GetPrivateChat(guidInBytes,guidHash);
                this.proxy.LogInPrivateChat(emailInBytes, emailHash, guidInBytes, guidHash);

                if (this.pc != null)
                {
                    if (this.pc.User2logged)
                    {
                        Logged.Add(this.pc.User2);
                    }


                    if (this.pc.User1logged)
                    {
                        Logged.Add(this.pc.User1);
                    }


                    foreach (Message m in this.pc.Messages)
                    {
                        Msg.Add(m);
                    }
                    allMessagesScrollViewer.ScrollToBottom();
                }
                else
                {
                    MessageBox.Show("You are blocked from this user and cannot chat with him!");
                    this.Close();
                }

            }


            Timer timer = new Timer(300000);
            timer.Elapsed +=
            (
                (object o, ElapsedEventArgs args) =>
                {
                    try
                    {
                        if (this.proxy.State == CommunicationState.Faulted)
                        {
                            NetTcpBinding tb = this.proxy.Binding;
                            EndpointAddress ad = this.proxy.Address;
                            this.proxy.Abort();
                            this.proxy = new WCFClient(instanceContext, tb, ad);
                        }

                        this.proxy.KeepConnection();
                    }
                    catch
                    {
                        // TODO: Handle exception.
                    }
                }
            );
        }

        private void ChatServiceCallback_PrivateChatNotified(object sender, PrivateChatEventArgs e)
        {
            if (e.PC == null)
            {
                this.Close();
            }
            else
            {
                pc = e.PC;
                Logged.Clear();
                if (pc.User1logged)
                {
                    Logged.Add(pc.User1);
                }
                if (pc.User2logged)
                {
                    Logged.Add(pc.User2);
                }

                Msg.Clear();
                foreach (Message m in pc.Messages)
                {
                    Msg.Add(m);
                }
                allMessagesScrollViewer.ScrollToBottom();

            }
        }

        /// <summary>
        /// On enter, send Message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                enterMsgButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        /// <summary>
        /// Leave room
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (i == 1)
            {
                byte[] emailInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, this.email);
                string emailHash = Sha256encrypt(this.email);
                byte[] roomInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, room.Theme);
                string roomHash = Sha256encrypt(room.Theme);
                proxy.LeaveRoom(roomInBytes, roomHash);
                proxy.UnsubscribeUserTheme(emailInBytes, roomInBytes, emailHash, roomHash);
                var s = new GroupChat(proxy, email);
                s.Show();
            }
            else
            {
                byte[] emailInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, this.email);
                string emailHash = Sha256encrypt(this.email);
                byte[] guidInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, pc.Uid.ToString());
                string guidHash = Sha256encrypt(pc.Uid.ToString());                
                this.proxy.LeavePrivateChat(guidInBytes, guidHash);
                this.proxy.UnsubscribeUserChat(emailInBytes, guidInBytes, emailHash, guidHash);
                var s = new GroupChat(proxy, email);
                s.Show();
            }
            
        }

        /// <summary>
        /// Ban user from theme room if you are admin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeUserButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] emailInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, loggedUsersListBox.SelectedItem.ToString());
            string emailHash = Sha256encrypt(loggedUsersListBox.SelectedItem.ToString());
            byte[] roomInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, room.Theme);
            string roomHash = Sha256encrypt(room.Theme);
            if (room.Logged.Any(x => x.Email.Equals(email)))
            {
                if (room.Logged.Single(x => x.Email.Equals(email)).Role == Roles.Admin)
                {
                    if (loggedUsersListBox.SelectedIndex != -1)
                    {
                        if (!email.Equals(loggedUsersListBox.SelectedItem.ToString()))
                        {
                            if (((Button)sender).Content.Equals("Ban user from chat"))
                            {
                                proxy.BlockUserFromRoom(emailInBytes,roomInBytes,emailHash,roomHash);
                            }
                            else
                            {
                                proxy.RemoveBlockUserFromRoom(emailInBytes, roomInBytes, emailHash, roomHash);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Convert input string to his hashed value using SHA256 alghoritm
        /// </summary>
        /// <param name="phrase">input string</param>
        /// <returns>Hashed value of input string</returns>
        public string Sha256encrypt(string phrase)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            System.Security.Cryptography.SHA256Managed sha256hasher = new System.Security.Cryptography.SHA256Managed();
            byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(phrase));
            return Convert.ToBase64String(hashedDataBytes);
        }

        public delegate void ThemeRoomEventHandler(object sender, ThemeRoomEventArgs e);
        public delegate void PrivateChatEventHandler(object sender, PrivateChatEventArgs e);

        public class ThemeRoomEventArgs : EventArgs
        {
            private Room room;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="message">Message from server.</param>
            public ThemeRoomEventArgs(Room room)
            {
                this.room = room;
            }

            /// <summary>
            /// Gets the message.
            /// </summary>
            public Room Room { get { return room; } }
        }

        public class PrivateChatEventArgs : EventArgs
        {
            private PrivateChat pc;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="message">Message from server.</param>
            public PrivateChatEventArgs(PrivateChat pc)
            {
                this.pc = pc;
            }

            /// <summary>
            /// Gets the message.
            /// </summary>
            public PrivateChat PC { get { return pc; } }
        }

        /// <summary>
        /// selection in list of logged users changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loggedUsersListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (loggedUsersListBox.SelectedIndex == -1)
            {
                blockUserButton.IsEnabled = false;
                removeUserButton.IsEnabled = false;
                //closeRoomButton.IsEnabled = false;
            }
            else
            {
                if (loggedUsersListBox.SelectedItem.ToString().Equals(email))
                {
                    blockUserButton.IsEnabled = false;
                    removeUserButton.IsEnabled = false;
                    //closeRoomButton.IsEnabled = false;
                }
                else
                {
                    if (i == 1)
                    {
                        if (room.Blocked.Any(x => x.Email.Equals(loggedUsersListBox.SelectedItem.ToString())))
                        {
                            removeUserButton.Content = "Unban user";
                        }
                        else
                        {
                            removeUserButton.Content = "Ban user from chat";
                        }

                        if (room.Logged.Any(x => x.Email.Equals(email)))
                        {
                            if (room.Logged.Single(x => x.Email.Equals(email)).Blocked.Any(x => x.Email.Equals(loggedUsersListBox.SelectedItem.ToString())))
                            {
                                blockUserButton.Content = "Unblock";
                                //closeRoomButton.IsEnabled = false;
                            }
                            else
                            {
                                blockUserButton.Content = "Block";
                            }
                        }
                        blockUserButton.IsEnabled = true;
                        removeUserButton.IsEnabled = true;
                        //closeRoomButton.IsEnabled = true;
                    }
                    else
                    {
                        if (pc.User1.Equals(email) && email.Equals(loggedUsersListBox.SelectedItem.ToString()))
                        {
                            blockUserButton.IsEnabled = false;
                        }
                        else if (pc.User2.Equals(email) && email.Equals(loggedUsersListBox.SelectedItem.ToString()))
                        {
                            blockUserButton.IsEnabled = false;
                        }
                        else
                        {
                            blockUserButton.IsEnabled = true;
                        }
                    }
                }
            }
        }
    }
}
