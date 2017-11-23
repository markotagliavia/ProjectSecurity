using SecurityManager;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ForumModels;
using System.ServiceModel;
using System.Timers;
using static ClientApp.ThemeRoom;
using System.Text;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GroupChat : Window
    {
        private WCFClient proxy;
        public ForumModels.GroupChat groupChat;
        private string email;
        private int i = 0;
        private ObservableCollection<User> logged;
        private ObservableCollection<Message> msg;
        private ObservableCollection<string> themeRooms;
        private ObservableCollection<string> privateRooms;
        private EncryptDecrypt aesCommander = new EncryptDecrypt();
        Room roompom;
        PrivateChat pcpom;

        private InstanceContext instanceContext;

        public GroupChat(WCFClient proxy, string email)
        {
            this.DataContext = this;
            logged = new ObservableCollection<User>();
            msg = new ObservableCollection<Message>();
            themeRooms = new ObservableCollection<string>();
            privateRooms = new ObservableCollection<string>();
            this.proxy = proxy;
            proxy.Abort();
            this.email = email;
            InitializeComponent();
            blockUserButton.IsEnabled = false;
            removeUserButton.IsEnabled = false;
            loggedAsLabel.Content = $"You are logged as {email}";
        }

        private void ChatServiceCallback_ClientNotified(object sender, ClientNotifiedEventArgs e)
        {
            groupChat = e.GroupChat;
            Logged.Clear();
            foreach (User u in groupChat.Logged)
            {
                Logged.Add(u);
            }

            foreach (Message m in groupChat.AllMessages)
            {
                if (Msg.Any(x => x.Code.Equals(m.Code)) == false)
                {
                    Msg.Add(m);
                }
            }
            allMessagesScrollViewer.ScrollToBottom();

            roomsMenuItem.Items.Clear();
            foreach (string s in groupChat.ThemeRooms)
            {
                MenuItem mi = new MenuItem();
                mi.Header = s;
                mi.Click += new RoutedEventHandler(menuItemRoomsClick);
                roomsMenuItem.Items.Add(mi);
            }

            privateChatsMenuItem.Items.Clear();
            foreach (string s in groupChat.PrivateChatsNames)
            {
                MenuItem mi = new MenuItem();
                mi.Header = s;
                mi.Click += new RoutedEventHandler(menuItemPrivateRoomsClick);
                privateChatsMenuItem.Items.Add(mi);
            }

            if (groupChat.Logged.Single(x => x.Email.Equals(email)).Role == Roles.Admin)
            {
                removeUserButton.Visibility = Visibility.Visible;
                changePrivsMenuItem.Visibility = Visibility.Visible;
                privateChatsMenuItem.Visibility = Visibility.Visible;
            }
            else
            {
                removeUserButton.Visibility = Visibility.Hidden;
                changePrivsMenuItem.Visibility = Visibility.Hidden;
                privateChatsMenuItem.Visibility = Visibility.Hidden;
            }
        }

        protected void menuItemRoomsClick(object sender, EventArgs e)
        {
            if (Logged.Any(x => x.Email.Equals(email)))
            {
                if (Logged.Single(x => x.Email.Equals(email)).Logged)
                {
                    Console.WriteLine(((MenuItem)sender).Header.ToString());
                    byte[] emailInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, this.email);
                    string emailHash = Sha256encrypt(this.email);
                    byte[] roomInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, ((MenuItem)sender).Header.ToString());
                    string roomHash = Sha256encrypt(((MenuItem)sender).Header.ToString());
                    Room r = proxy.GetThemeRoom(roomInBytes,emailInBytes,roomHash, emailHash);
                    if (r != null)
                    {
                        if (!r.Blocked.Any(x => x.Email.Equals(email)))
                        {
                            this.i = 2;
                            roompom = r;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("You are removed from this room!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("You are removed from this room!");
                    }
                }
            }
        }

        protected void menuItemPrivateRoomsClick(object sender, EventArgs e)
        {
            if (Logged.Any(x => x.Email.Equals(email)))
            {
                if (Logged.Single(x => x.Email.Equals(email)).Logged)
                {
                    Console.WriteLine(((MenuItem)sender).Header.ToString());
                    byte[] roomInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, ((MenuItem)sender).Header.ToString());
                    string roomHash = Sha256encrypt(((MenuItem)sender).Header.ToString());
                    PrivateChat pc = proxy.GetPrivateChat(roomInBytes, roomHash);
                    if (pc != null)
                    {
                        //proveri da li imaju jedan drugog u blokiranim?
                            this.i = 3;
                            pcpom = pc;
                            this.Close();
                    }
                }
            }
        }

        public ObservableCollection<User> Logged
        {
            get{ return logged; }
            set { logged = value; }
        }

        public ObservableCollection<Message> Msg
        {
            get { return msg; }
            set { msg = value; }
        }

        public ObservableCollection<string> ThemeRooms
        {
            get { return themeRooms; }
            set { themeRooms = value; }
        }

        public ObservableCollection<string> PrivateRooms
        {
            get { return privateRooms; }
            set { privateRooms = value; }
        }

        /// <summary>
        /// Sending entered message to server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enterMsgButton_Click(object sender, RoutedEventArgs e)
        {
            if (!groupChat.Blocked.Any(x => x.Email.Equals(email)))
            {
                if (!String.IsNullOrWhiteSpace(entryMessageTextbox.Text))
                {
                    if (groupChat.Logged.Single(x => x.Email.Equals(email)).Logged)
                    {
                        if (groupChat.Logged.Single(x => x.Email.Equals(email)) != null)
                        {
                            byte[] emailInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, this.email);
                            string emailHash = Sha256encrypt(this.email);
                            byte[] msgInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, entryMessageTextbox.Text);
                            string msgHash = Sha256encrypt(entryMessageTextbox.Text);
                            proxy.SendGroupMessage(emailInBytes,msgInBytes,emailHash,msgHash );
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
        /// Log out from Forum
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logOutButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] emailInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, this.email);
            string emailHash = Sha256encrypt(this.email);
            //proxy.LogOut(emailInBytes,emailHash);
            //proxy.Unsubscribe(emailInBytes, emailHash);
            this.Close();
        }

        /// <summary>
        /// Blocking selected user
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
                    if (((Button)sender).Content.Equals("Block user"))
                    {
                        proxy.BlockUser(emailInBytes,userInBytes,emailHash,userHash);
                    }
                    else
                    {
                        proxy.RemoveBlockUser(emailInBytes, userInBytes, emailHash, userHash);
                    }
                }
            }
        }

        /// <summary>
        /// Ban user from group chat if you are admin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeUserButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] userInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, loggedUsersListBox.SelectedItem.ToString());
            string userHash = Sha256encrypt(loggedUsersListBox.SelectedItem.ToString());
            if (loggedUsersListBox.SelectedIndex != -1)
            {
                if (!email.Equals(loggedUsersListBox.SelectedItem.ToString()))
                {
                    if (((Button)sender).Content.Equals("Ban user from chat"))
                    {
                        proxy.BlockGroupChat(userInBytes,userHash);
                    }
                    else
                    {
                        proxy.RemoveBlockGroupChat(userInBytes,userHash);
                    }
                }
            }
        }

        /// <summary>
        /// Create new room
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newRoomMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Logged.Any(x => x.Email.Equals(email)))
            {
                if (Logged.Single(x => x.Email.Equals(email)).Logged)
                {
                    if (Logged.Single(x => x.Email.Equals(email)).Role.Equals(Roles.Admin))
                    {
                        var s = new NewRoomWindow(this.proxy, email);
                        s.Show();
                    }
                    else
                    {
                        MessageBox.Show("Forbbidden! Only admins can create new rooms!");
                        Audit.AuthorizationFailed(email, "New room", "Authorization failed");
                    }
                }
            }
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pswChangeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Logged.Any(x => x.Email.Equals(email)))
            {
                if (Logged.Single(x => x.Email.Equals(email)).Logged)
                {
                    var s = new ChangePasswordWindow(this.proxy, email);
                    s.Show();
                }               
            }
        }

        /// <summary>
        /// Change privilegies if you are admin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changePrivsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Logged.Any(x => x.Email.Equals(email)))
            {
                if (Logged.Single(x => x.Email.Equals(email)).Logged)
                {
                    if (Logged.Single(x => x.Email.Equals(email)).Role.Equals(Roles.Admin))
                    {
                        this.i = 1;
                        
                        this.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Log out from menu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logOutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Selection on logged users changes and event is raised
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loggedUsersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (loggedUsersListBox.SelectedIndex == -1)
            {
                blockUserButton.IsEnabled = false;
                removeUserButton.IsEnabled = false;
                openPrivateChatButton.IsEnabled = false;
            }
            else
            {
                if (loggedUsersListBox.SelectedItem.ToString().Equals(email))
                {
                    blockUserButton.IsEnabled = false;
                    removeUserButton.IsEnabled = false;
                    openPrivateChatButton.IsEnabled = false;
                }
                else
                {
                    if (groupChat.Blocked.Any(x => x.Email.Equals(loggedUsersListBox.SelectedItem.ToString())))
                    {
                        removeUserButton.Content = "Unban user";
                    }
                    else
                    {
                        removeUserButton.Content = "Ban user from chat";
                    }

                    if (groupChat.Logged.Any(x => x.Email.Equals(email)))
                    {
                        if (groupChat.Logged.Single(x => x.Email.Equals(email)).Blocked.Any(x => x.Email.Equals(loggedUsersListBox.SelectedItem.ToString())))
                        {
                            blockUserButton.Content = "Unblock";
                            openPrivateChatButton.IsEnabled = false;
                        }
                        else
                        {
                            blockUserButton.Content = "Block";
                        }
                    }
                    blockUserButton.IsEnabled = true;
                    removeUserButton.IsEnabled = true;
                    openPrivateChatButton.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Setup for group chat windows on load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChatServiceCallback chatServiceCallback = new ChatServiceCallback();
            chatServiceCallback.ClientNotified += ChatServiceCallback_ClientNotified;

            instanceContext = new InstanceContext(chatServiceCallback);

            //this.proxy.InstanceContext = instanceContext;

            EndpointAddress adr = this.proxy.Address;
            NetTcpBinding tcp = this.proxy.Binding;
            Guid guid = this.proxy.Guid;
            GenerateAesKey aes = this.proxy.Aes;
            tcp.MaxConnections = 500;
              tcp.OpenTimeout = new TimeSpan(0, 10, 0);
              tcp.CloseTimeout = new TimeSpan(0, 10, 0);
              tcp.SendTimeout = new TimeSpan(0, 1, 0);
              tcp.ReceiveTimeout = new TimeSpan(0, 10, 0);
            this.proxy.InstanceContext = instanceContext;
            this.proxy.Abort();
            
            this.proxy = new WCFClient(instanceContext, tcp, adr);
            this.proxy.Guid = guid;
            this.proxy.Aes = aes;
            this.proxy.SessionKey(guid);
            byte[] emailInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, this.email);
            string emailHash = Sha256encrypt(this.email);
            try
            {
                this.proxy.Subscribe(emailInBytes,emailHash);
            }
            catch
            {
                // TODO: Handle exception.
            }

            groupChat = this.proxy.GetGroupChat();
            Logged.Clear();
            foreach (User u in groupChat.Logged)
            {
                Logged.Add(u);
            }

            foreach (Message m in groupChat.AllMessages)
            {
                if (Msg.Any(x => x.Code.Equals(m.Code)) == false)
                {
                    Msg.Add(m);
                }
            }
            allMessagesScrollViewer.ScrollToBottom();

            roomsMenuItem.Items.Clear();
            foreach (string s in groupChat.ThemeRooms)
            {
                MenuItem mi = new MenuItem();
                mi.Header = s;
                mi.Click += new RoutedEventHandler(menuItemRoomsClick);
                roomsMenuItem.Items.Add(mi);
            }

            privateChatsMenuItem.Items.Clear();
            foreach (string s in groupChat.PrivateChatsNames)
            {
                MenuItem mi = new MenuItem();
                mi.Header = s;
                mi.Click += new RoutedEventHandler(menuItemPrivateRoomsClick);
                privateChatsMenuItem.Items.Add(mi);
            }

            if (groupChat.Logged.Single(x => x.Email.Equals(email)).Role == Roles.Admin)
            {
                removeUserButton.Visibility = Visibility.Visible;
                changePrivsMenuItem.Visibility = Visibility.Visible;
                privateChatsMenuItem.Visibility = Visibility.Visible;
            }


            Timer timer = new Timer(100);
            timer.Elapsed +=
            (
                (object o, ElapsedEventArgs args) =>
                {
                    try
                    {
                        if (this.proxy.State == CommunicationState.Faulted)
                        {
                            NetTcpBinding tb = this.proxy.Binding;
                            tb.MaxConnections = 500;
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
                 if(enterMsgButton.IsEnabled) enterMsgButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        /// <summary>
        /// on close, logout user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            byte[] emailInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, this.email);
            string emailHash = Sha256encrypt(this.email);
            if (i == 0)
            {
                proxy.LogOut(emailInBytes, emailHash);
                proxy.Unsubscribe(emailInBytes, emailHash);
                var s = new MainWindow();
                s.Show();
                //unsubscribe
            }
            else if (i == 1)
            {
                proxy.Unsubscribe(emailInBytes, emailHash);
                var s = new ChangeRole(this.proxy, email);
                s.Show();
            }
            else if (i == 2)
            {
                proxy.Unsubscribe(emailInBytes, emailHash);
                var window = new ThemeRoom(this.proxy, this.roompom.Theme, this.email, 1);
                window.Show();
            }
            else if (i == 3)
            {
                proxy.Unsubscribe(emailInBytes, emailHash);
                var window = new ThemeRoom(this.proxy, pcpom.Uid.ToString(), this.email, 2);
                window.Show();
            }
            
        }

        /// <summary>
        /// Opens private chat with selected user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openPrivateChatButton_Click(object sender, RoutedEventArgs e)
        {
            string userToChat = loggedUsersListBox.SelectedItem.ToString();
            if (!this.email.Equals(userToChat))
            {
                byte[] firstInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, this.email);
                string firstHAsh = Sha256encrypt(this.email);
                byte[] secondInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, userToChat);
                string secondHAsh = Sha256encrypt(userToChat);
                pcpom = this.proxy.CreatePrivateChat(firstInBytes,secondInBytes,firstHAsh,secondHAsh);
                this.i = 3;
                this.Close();
            }
            else
            {
                MessageBox.Show("Cannot talk to itself.");
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

       
    }


    public class ChatServiceCallback : IChatServiceCallback
    {
        public event ClientNotifiedEventHandler ClientNotified;
        public event AllUsersNotifiedEventHandler UserNotified;
        public event ThemeRoomEventHandler ThemeNotified;
        public event PrivateChatEventHandler PrivateChatNotified;

        /// <summary>
        /// Notifies the client of the message by raising an event.
        /// </summary>
        /// <param name="message">Message from the server.</param>

        void IChatServiceCallback.HandleGroupChat(ForumModels.GroupChat gr)
        {
            if (ClientNotified != null)
            {
                ClientNotified(this, new ClientNotifiedEventArgs(gr));
            }
        }
        void IChatServiceCallback.AllUsers(ObservableCollection<User> users)
        {
            if (UserNotified != null)
            {
                UserNotified(this, new AllUsersNotifiedEventArgs(users));
            }
        }

        void IChatServiceCallback.GetRoom(Room room)
        {
            if (ThemeNotified != null)
            {
                ThemeNotified(this, new ThemeRoomEventArgs(room));
            }
        }

        void IChatServiceCallback.GetPrivateChat(PrivateChat pc)
        {
            if (PrivateChatNotified != null)
            {
                PrivateChatNotified(this, new PrivateChatEventArgs(pc));
            }
        }
    }

    public delegate void ClientNotifiedEventHandler(object sender, ClientNotifiedEventArgs e);

    public class ClientNotifiedEventArgs : EventArgs
    {
        private ForumModels.GroupChat groupChat;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Message from server.</param>
        public ClientNotifiedEventArgs(ForumModels.GroupChat gr)
        {
            this.groupChat = gr;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public ForumModels.GroupChat GroupChat { get { return groupChat; } }
    }
}
