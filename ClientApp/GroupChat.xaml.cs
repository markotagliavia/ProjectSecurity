using SecurityManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ForumModels;
using System.ServiceModel;
using System.Timers;

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
        private ObservableCollection<User> logged;
        private ObservableCollection<Message> msg;

        private InstanceContext instanceContext;

        public GroupChat(WCFClient proxy, string email)
        {
            this.DataContext = this;
            logged = new ObservableCollection<User>();
            msg = new ObservableCollection<Message>();
            this.proxy = proxy;
            proxy.Abort();
            this.email = email;
            InitializeComponent();
        }

        private void ChatServiceCallback_ClientNotified(object sender, ClientNotifiedEventArgs e)
        {
            groupChat = e.GroupChat;
            foreach (User u in groupChat.Logged)
            {
                if (Logged.Any(x => x.Email.Equals(u.Email)) == false)
                {
                    Logged.Add(u);
                }
            }

            foreach (Message m in groupChat.AllMessages)
            {
                if (Msg.Any(x => x.Code.Equals(m.Code)) == false)
                {
                    Msg.Add(m);
                }
            }
            //Logged = groupChat.Logged;
            //Msg = groupChat.AllMessages;

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

        /// <summary>
        /// Sending entered message to server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enterMsgButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(entryMessageTextbox.Text))
            {
                if (groupChat.Logged.Single(x => x.Email.Equals(email)).Logged)
                {
                    if (groupChat.Logged.Single(x => x.Email.Equals(email)) != null)
                    {
                        proxy.SendGroupMessage(email, entryMessageTextbox.Text);
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

        /// <summary>
        /// Log out from Forum
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logOutButton_Click(object sender, RoutedEventArgs e)
        {
            proxy.LogOut(email);
            var s = new MainWindow();
            s.Show();
            this.Close();
        }

        /// <summary>
        /// Blocking selected user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void blockUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (loggedUsersListBox.SelectedIndex != -1)
            {
                if (!email.Equals(loggedUsersListBox.SelectedItem.ToString()))
                {
                    proxy.BlockUser(email, loggedUsersListBox.SelectedItem.ToString());
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
            if (loggedUsersListBox.SelectedIndex != -1)
            {
                if (!email.Equals(loggedUsersListBox.SelectedItem.ToString()))
                {
                    proxy.BlockGroupChat(loggedUsersListBox.SelectedItem.ToString());
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
            // TO DO
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pswChangeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //TO DO
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
                if (Logged.Single(x => x.Email.Equals(email)).Role.Equals(Roles.Admin))
                {
                    var s = new ChangeRoll(this.proxy, email);
                    s.Show();
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
            proxy.LogOut(email);
            var s = new MainWindow();
            s.Show();
            this.Close();
        }

        /// <summary>
        /// Selection on logged users changes and event is raised
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loggedUsersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TO DO
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChatServiceCallback chatServiceCallback = new ChatServiceCallback();
            chatServiceCallback.ClientNotified += ChatServiceCallback_ClientNotified;

            instanceContext = new InstanceContext(chatServiceCallback);

            //this.proxy.InstanceContext = instanceContext;

            EndpointAddress adr = this.proxy.Address;
            NetTcpBinding tcp = this.proxy.Binding;
            this.proxy.InstanceContext = instanceContext;
            this.proxy.Abort();
            
            this.proxy = new WCFClient(instanceContext, tcp, adr);
            groupChat = this.proxy.GetGroupChat();
            foreach (User u in groupChat.Logged)
            {
                if (Logged.Any(x => x.Email.Equals(u.Email)) == false)
                {
                    Logged.Add(u);
                }
            }

            foreach (Message m in groupChat.AllMessages)
            {
                if (Msg.Any(x => x.Code.Equals(m.Code)) == false)
                {
                    Msg.Add(m);
                }
            }
            if (groupChat.Logged.Single(x => x.Email.Equals(email)).Role == Roles.Admin)
            {
                removeUserButton.Visibility = Visibility.Visible;
                changePrivsMenuItem.Visibility = Visibility.Visible;
            }



            try
            {
                this.proxy.Subscribe(email);
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
    }
    

    public class ChatServiceCallback : IChatServiceCallback
    {
        public event ClientNotifiedEventHandler ClientNotified;


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
