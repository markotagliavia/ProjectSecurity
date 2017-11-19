using ForumModels;
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
using System.Windows.Shapes;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for PrivateRoom.xaml
    /// </summary>
    public partial class ThemeRoom : Window
    {
        private WCFClient proxy;
        public ForumModels.Room room;

        public ThemeRoom(WCFClient proxy)
        {
            this.proxy = proxy;
            this.room = this.proxy.GetPrivateRoom(room.Theme);
            InitializeComponent();
            label.Content = room.Theme;
            DataContext = this;
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



    }
}
