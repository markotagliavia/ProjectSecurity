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
    /// Interaction logic for ChangeRole.xaml
    /// </summary>
    public partial class ChangeRole : Window
    {
        private WCFClient proxy;
        private string email;
        private ObservableCollection<User> admins;
        private ObservableCollection<User> users;
        private ObservableCollection<User> allusers;
        private InstanceContext instanceContext;
        private EncryptDecrypt aesCommander = new EncryptDecrypt();
        public ChangeRole(WCFClient proxy, string email)
        {
            this.DataContext = this;
            Admins = new ObservableCollection<User>();
            Users = new ObservableCollection<User>();
            AllUsers = new ObservableCollection<User>();
            this.proxy = proxy;
            proxy.Abort();
            this.email = email;
            InitializeComponent();
            AddADMIN.IsEnabled = false;
            AddUSER.IsEnabled = false;
        }
        public ObservableCollection<User> Admins
        {
            get { return admins; }
            set { admins = value; }
        }
        public ObservableCollection<User> Users
        {
            get { return users; }
            set { users = value; }
        }
        public ObservableCollection<User> AllUsers
        {
            get { return allusers; }
            set { allusers = value; }
        }
        private void ChatServiceCallback_AllUsersNotified(object sender, AllUsersNotifiedEventArgs e)
        {
            AllUsers = e.AllUsers;
            Admins.Clear();
            Users.Clear();
            foreach (User u in AllUsers)
            {
                if (u.Role == Roles.Admin)
                {
                    Admins.Add(u);
                }
                else if (u.Role == Roles.User)
                {
                    Users.Add(u);
                }
            }

        }

        private void LoadedChangeRoll(object sender, RoutedEventArgs e)
        {
            ChatServiceCallback chatServiceCallback = new ChatServiceCallback();
            chatServiceCallback.UserNotified += ChatServiceCallback_AllUsersNotified;
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
            allusers = this.proxy.GetAllUsers(emailInBytes,emailHash); //kad ovo sredis, radice
            foreach (User u in allusers)
            {
                if (u.Role == Roles.Admin)
                {
                    Admins.Add(u);
                }
                else if (u.Role == Roles.User)
                {
                    Users.Add(u);
                }
            }

            try
            {
                this.proxy.SubscribeAllUsers(emailInBytes, emailHash);
            }
            catch
            {
                // TODO: Handle exception.
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

        private void AddAdminClick(object sender, RoutedEventArgs e)
        {
            if (!UsersListBox.SelectedItem.ToString().Equals(email))
            {
                byte[] emailInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, UsersListBox.SelectedItem.ToString());
                string emailHash = Sha256encrypt(UsersListBox.SelectedItem.ToString());
                bool ret = this.proxy.AddAdmin(emailInBytes,emailHash);
                if (ret)
                {
                    Admins.Add((User)UsersListBox.SelectedItem);
                    Users.Remove((User)UsersListBox.SelectedItem);
                }
                else
                {
                    MessageBox.Show("Cannot move this user to admin");
                }
               
                
            }
            
            AddADMIN.IsEnabled = false;
            AddUSER.IsEnabled = false;

        }

        private void AddUserClick(object sender, RoutedEventArgs e)
        {
            if (!AdminsListBox.SelectedItem.ToString().Equals(email))
            {
                byte[] emailInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, AdminsListBox.SelectedItem.ToString());
                string emailHash = Sha256encrypt(AdminsListBox.SelectedItem.ToString());
                bool ret = this.proxy.DeleteAdmin(emailInBytes, emailHash);
                if (ret)
                {
                    Users.Add((User)AdminsListBox.SelectedItem);
                    Admins.Remove((User)AdminsListBox.SelectedItem);
                }
                else
                {
                    MessageBox.Show("Cannot move this user to users");
                }
                
                
            }
            
            AddADMIN.IsEnabled = false;
            AddUSER.IsEnabled = false;
        }

        private void loggedUsersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddADMIN.IsEnabled = true;
            AddUSER.IsEnabled = false;
        }

        private void AdminsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddADMIN.IsEnabled = false;
            AddUSER.IsEnabled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            byte[] emailInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, this.email);
            string emailHash = Sha256encrypt(this.email);
            this.proxy.UnsubscribeAllUsers(emailInBytes, emailHash);
            var gr = new GroupChat(this.proxy,this.email);
            gr.Show();
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
    public delegate void AllUsersNotifiedEventHandler(object sender, AllUsersNotifiedEventArgs e);

    public class AllUsersNotifiedEventArgs : EventArgs
    {
        private ObservableCollection<User> allusers;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Message from server.</param>
        public AllUsersNotifiedEventArgs(ObservableCollection<User> au)
        {
            this.allusers = au;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public ObservableCollection<User> AllUsers { get { return allusers; } }
    }
}

