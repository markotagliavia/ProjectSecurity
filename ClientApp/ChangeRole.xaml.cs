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
        public ChangeRole()
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
            Admins = new ObservableCollection<User>(); //ne mora bas sve da se optimizuje :D 
            Users = new ObservableCollection<User>();
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

        }

        private void LoadedChangeRoll(object sender, RoutedEventArgs e)
        {
            ChatServiceCallback chatServiceCallback = new ChatServiceCallback();
            //chatServiceCallback.UserNotified += ChatServiceCallback_AllUsersNotified;
            instanceContext = new InstanceContext(chatServiceCallback);

            EndpointAddress adr = this.proxy.Address;
            NetTcpBinding tcp = this.proxy.Binding;
            this.proxy.InstanceContext = instanceContext;
            this.proxy.Abort();

            this.proxy = new WCFClient(instanceContext, tcp, adr);
            //allusers = this.proxy.GetAllUsers();
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
                //this.proxy.SubscribeAllUsers(email);
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
            if (!loggedUsersListBox.SelectedItem.ToString().Equals(email))
            {
                this.proxy.AddAdmin(loggedUsersListBox.SelectedItem.ToString());
                Admins.Add((User)loggedUsersListBox.SelectedItem);
                Users.Remove((User)loggedUsersListBox.SelectedItem);
                
            }
            loggedUsersListBox.SelectedItems.Clear();
            AddADMIN.IsEnabled = false;
            AddUSER.IsEnabled = false;

        }

        private void AddUserClick(object sender, RoutedEventArgs e)
        {
            if (!AdminsListBox.SelectedItem.ToString().Equals(email))
            {
                this.proxy.DeleteAdmin(AdminsListBox.SelectedItem.ToString());
                Users.Add((User)AdminsListBox.SelectedItem);
                Admins.Remove((User)AdminsListBox.SelectedItem);
                
            }
            AdminsListBox.SelectedItems.Clear();
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

