using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace ClientApp
{
    /// <summary>
    /// Interaction logic for VerificationKey.xaml
    /// </summary>
    public partial class VerificationKey : Window
    {
        private WCFClient proxy;
        private string email;

        public VerificationKey(WCFClient proxy, string email)
        {
            InitializeComponent();
            this.proxy = proxy;
            this.email = email;
        }

        /// <summary>
        /// When window is initialized, focus is on key field
        /// </summary>
        /// <param name="sender">key field</param>
        /// <param name="e">event</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            verificationKeyTextBox.Focus();
        }

        /// <summary>
        /// We are sending entered verification key to server
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event</param>
        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            string verificationKey = verificationKeyTextBox.Text;

            //send data to server
            bool i = proxy.SendVerificationKey(verificationKey);
            if(i == true)
            {
                //key is ok
            //  var s = new GroupChat(proxy, email);
            //  s.Show();
            //  this.Close();
            }
            else
            {
            //  //key is not ok, try again
            }
        }

        /// <summary>
        /// Cancel button is pressed, back to authentication page
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event</param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            var s = new MainWindow();
            s.Show();
            this.Close();
        }

        /// <summary>
        /// When enter is pressed we should raise event for submit data
        /// </summary>
        /// <param name="sender">key</param>
        /// <param name="e">event</param>
        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                submitButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
    }
}
