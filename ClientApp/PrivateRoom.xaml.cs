using Forum;
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
    public partial class PrivateRoom : Window
    {
        private WCFClient proxy;
        public Forum.Room room;

        public PrivateRoom(WCFClient proxy)
        {
            this.proxy = proxy;
            this.room = this.proxy.GetPrivateRoom(room.Theme);
            InitializeComponent();
            label.Content = room.Theme;
            DataContext = this;
            populateMsg();
        }

        public ObservableCollection<User> Logged 
        {
            get
            {
                return room.Logged;
            }
        }

        private void populateMsg()
        {
            foreach (Message m in room.AllMessages)
            {
                ChatBox.AppendText($"{m.User} wrote on {m.CreationTime.ToString()} :\n{m.Text}\n\n");
            }
        }
    }
}
