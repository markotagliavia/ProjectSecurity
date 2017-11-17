using System;
using System.Collections.Generic;
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
            DataContext = this;
        }
    }
}
