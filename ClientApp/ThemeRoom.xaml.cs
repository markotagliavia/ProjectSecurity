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
                return room.Logged;
            }
        }

        public ObservableCollection<Message> Msg
        {
            get
            {
                return room.AllMessages;
            }
        }

        /// <summary>
        /// sending message to theme room
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enterMsgButton_Click(object sender, RoutedEventArgs e)
        {
            if (!room.Blocked.Any(x => x.Email.Equals(email)))
            {
                if (!String.IsNullOrWhiteSpace(entryMessageTextbox.Text))
                {
                    if (room.Logged.Single(x => x.Email.Equals(email)).Logged)
                    {
                        if (room.Logged.Single(x => x.Email.Equals(email)) != null)
                        {
                            proxy.SendRoomMessage(email, room.Theme, entryMessageTextbox.Text);
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
            if (loggedUsersListBox.SelectedIndex != -1)
            {
                if (!email.Equals(loggedUsersListBox.SelectedItem.ToString()))
                {
                    if (((Button)sender).Content.Equals("Block"))
                    {
                        proxy.BlockUser(email, loggedUsersListBox.SelectedItem.ToString());
                    }
                    else
                    {
                        proxy.RemoveBlockUser(email, loggedUsersListBox.SelectedItem.ToString());
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
            if (room.Logged.Any(x => x.Email.Equals(email)))
            {
                if (room.Logged.Single(x => x.Email.Equals(email)).Role == Roles.Admin)
                {
                    proxy.CloseRoom(room.Theme);
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
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChatServiceCallback chatServiceCallback = new ChatServiceCallback();
            chatServiceCallback.ThemeNotified += ChatServiceCallback_ThemeNotified;
            instanceContext = new InstanceContext(chatServiceCallback);

            EndpointAddress adr = this.proxy.Address;
            NetTcpBinding tcp = this.proxy.Binding;
            this.proxy.InstanceContext = instanceContext;
            this.proxy.Abort();

            this.proxy = new WCFClient(instanceContext, tcp, adr);


            try
            {
                this.proxy.SubscribeUserTheme(email, room.Theme);
            }
            catch
            {
                // TODO: Handle exception.
            }

            room = this.proxy.GetThemeRoom(room.Theme);
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
            proxy.LeaveRoom(room.Theme);
            proxy.UnsubscribeUserTheme(email, room.Theme);
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
                                proxy.BlockUserFromRoom(loggedUsersListBox.SelectedItem.ToString(), room.Theme);
                            }
                            else
                            {
                                proxy.RemoveBlockUserFromRoom(loggedUsersListBox.SelectedItem.ToString(), room.Theme);
                            }
                        }
                    }
                }
            }
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
