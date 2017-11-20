using ForumModels;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
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
            loggedAsLabel.Content = $"You are logged as {email}";
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
            //TO DO
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
            //TO DO
        }

        /// <summary>
        /// Close theme room
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeRoomButton_Click(object sender, RoutedEventArgs e)
        {
            //TO DO check admin rigths
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (room.Logged.Single(x => x.Email.Equals(email)).Role == Roles.Admin)
            {
                removeUserButton.Visibility = Visibility.Visible;
                closeRoomButton.Visibility = Visibility.Visible;
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
            //TO DO kreirati na servisu ovu fju
            //proxy.LeaveThemeRoom(room.Theme);
        }

        /// <summary>
        /// Ban user from theme room if you are admin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeUserButton_Click(object sender, RoutedEventArgs e)
        {
            //TO DO check admin rights
        }
    }
}
