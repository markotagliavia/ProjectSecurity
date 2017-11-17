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

        public GroupChat(WCFClient proxy)
        {
            InitializeComponent();
            this.proxy = proxy;
            groupChat = proxy.GetGroupChat();
            DataContext = this;
            populateMsg();
        }

        public ObservableCollection<User> Logged
        {
            get
            {
                return groupChat.Logged;
            }
        }

        private void populateMsg()
        {
            foreach (Message m in groupChat.AllMessages)
            {
                ChatBox.AppendText($"{m.User} wrote on {m.CreationTime.ToString()} :\n{m.Text}\n\n");
            }
        }
    }
}
