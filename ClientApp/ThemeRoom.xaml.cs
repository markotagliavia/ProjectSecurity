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
        private string email;
        private ObservableCollection<User> logged;
        private ObservableCollection<Message> msg;
        private InstanceContext instanceContext;
        private EncryptDecrypt aesCommander = new EncryptDecrypt();

        public ThemeRoom(WCFClient proxy, Room r, string email)
        {
            DataContext = this;
            this.proxy = proxy;
            this.room = r;
            this.email = email;
            logged = new ObservableCollection<User>();
            msg = new ObservableCollection<Message>();
            InitializeComponent();
            label.Content = room.Theme;
            blockUserButton.IsEnabled = false;
            removeUserButton.IsEnabled = false;
            loggedAsLabel1.Content = $"You are logged as {email}";           
        }

        public ObservableCollection<User> Logged
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
                            proxy.SendRoomMessage(emailInBytes,roomInBytes,msgInBytes,emailHash, roomHash, msgHash);
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
                MessageBox.Show("You are banned from group chat and cannot write messages!");
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
            byte[] emailInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, this.email);
            string emailHash = Sha256encrypt(this.email);
            byte[] userInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, loggedUsersListBox.SelectedItem.ToString());
            string userHash = Sha256encrypt(loggedUsersListBox.SelectedItem.ToString());
            if (loggedUsersListBox.SelectedIndex != -1)
            {
                if (!email.Equals(loggedUsersListBox.SelectedItem.ToString()))
                {
                    if (((Button)sender).Content.Equals("Block"))
                    {
                        proxy.BlockUser(emailInBytes, userInBytes,emailHash,userHash);
                    }
                    else
                    {
                        proxy.RemoveBlockUser(emailInBytes, userInBytes, emailHash, userHash);
                    }
                }
            }
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
                    proxy.CloseRoom(roomInBytes,roomHash);
                }
            }
        }

        private void ChatServiceCallback_ThemeNotified(object sender, ThemeRoomEventArgs e)
        {
            if (e.Room == null)
            {
                this.Close();
            }
            else
            {
                room = e.Room;
                Logged.Clear();
                foreach (User u in room.Logged)
                {
                    Logged.Add(u);
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
            this.proxy.SendSessionKey(guid);
            byte[] emailInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, this.email);
            string emailHash = Sha256encrypt(this.email);
            byte[] roomInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, room.Theme);
            string roomHash = Sha256encrypt(room.Theme);
            try
            {
                this.proxy.SubscribeUserTheme(emailInBytes,roomInBytes,emailHash, roomHash);
            }
            catch
            {
                // TODO: Handle exception.
            }
            
            this.room = this.proxy.GetThemeRoom(roomInBytes, emailInBytes, roomHash,emailHash);
            this.proxy.LogInTheme(roomInBytes, emailInBytes, roomHash, emailHash);

            Logged.Clear();
            foreach (User u in this.room.Logged)
            {
                Logged.Add(u);
            }

            Msg.Clear();
            foreach (Message m in this.room.AllMessages)
            {
                Msg.Add(m);
            }
            allMessagesScrollViewer.ScrollToBottom();

            if (this.room.Logged.Single(x => x.Email.Equals(email)).Role == Roles.Admin)
            {
                removeUserButton.Visibility = Visibility.Visible;
                closeRoomButton.Visibility = Visibility.Visible;
            }
            else
            {
                removeUserButton.Visibility = Visibility.Hidden;
                closeRoomButton.Visibility = Visibility.Hidden;
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
            byte[] emailInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, this.email);
            string emailHash = Sha256encrypt(this.email);
            byte[] roomInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, room.Theme);
            string roomHash = Sha256encrypt(room.Theme);
            proxy.LeaveRoom(roomInBytes,roomHash);
            proxy.UnsubscribeUserTheme(emailInBytes,roomInBytes,emailHash, roomHash);
            var s = new GroupChat(proxy, email);
            s.Show();
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

        /// <summary>
        /// selection in list of logged users changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loggedUsersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (loggedUsersListBox.SelectedIndex == -1)
            {
                blockUserButton.IsEnabled = false;
                removeUserButton.IsEnabled = false;
                closeRoomButton.IsEnabled = false;
            }
            else
            {
                if (loggedUsersListBox.SelectedItem.ToString().Equals(email))
                {
                    blockUserButton.IsEnabled = false;
                    removeUserButton.IsEnabled = false;
                    closeRoomButton.IsEnabled = false;
                }
                else
                {
                    if (room.Blocked.Any(x => x.Email.Equals(email)))
                    {
                        removeUserButton.Content = "Unban user";
                    }

                    if (room.Logged.Any(x => x.Email.Equals(email)))
                    {
                        if (room.Logged.Single(x => x.Email.Equals(email)).Blocked.Any(x => x.Email.Equals(loggedUsersListBox.SelectedItem.ToString())))
                        {
                            blockUserButton.Content = "Unblock";
                            closeRoomButton.IsEnabled = false;
                        }
                    }
                    blockUserButton.IsEnabled = true;
                    removeUserButton.IsEnabled = true;
                    closeRoomButton.IsEnabled = true;
                }
            }
        }
    }
}
