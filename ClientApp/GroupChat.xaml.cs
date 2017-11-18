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
using Forum;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GroupChat : Window
    {
        private WCFClient proxy;
        public Forum.GroupChat groupChat;
        private string email;

        public GroupChat(WCFClient proxy, string email)
        {
            InitializeComponent();
            this.proxy = proxy;
            this.email = email;
            groupChat = proxy.GetGroupChat();
            DataContext = this;
            if (groupChat.Logged.Single(x => x.Email.Equals(email)).Role == Roles.Admin)
            {
                removeUserButton.Visibility = Visibility.Visible;
                changePrivsMenuItem.Visibility = Visibility.Visible;
            }
        }

        public ObservableCollection<User> Logged
        {
            get
            {
                return groupChat.Logged;
            }
        }

        public ObservableCollection<Message> Msg
        {
            get
            {
                return groupChat.AllMessages;
            }
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
            // TO DO
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
    }
}
