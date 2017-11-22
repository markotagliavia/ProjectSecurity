using SecurityManager;
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
    /// Interaction logic for NewRoomWindow.xaml
    /// </summary>
    public partial class NewRoomWindow : Window
    {
        private WCFClient proxy;
        private string email;
        private EncryptDecrypt aesCommander = new EncryptDecrypt();

        public NewRoomWindow(WCFClient proxy, string email)
        {
            InitializeComponent();
            this.proxy = proxy;
            this.email = email;
        }

        /// <summary>
        /// When window is initialized, focus is on theme field
        /// </summary>
        /// <param name="sender">key field</param>
        /// <param name="e">event</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            themeTextBox.Focus();
        }

        /// <summary>
        /// We are sending entered verification key to server
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event</param>
        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            string theme = themeTextBox.Text;
            byte[] themeInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, theme);
            string themeHash = Sha256encrypt(theme);
            //send data to server
            proxy.CreateRoom(themeInBytes,themeHash);
            this.Close();
          /*  if (i == true)
            {
                //treba notificirati na grupnom cetu, videti sa TIjanom
                //TO DO
                this.Close();
            }
            else
            {
                MessageBox.Show("There is already this theme room on forum!");
            }*/
        }

        /// <summary>
        /// Cancel button is pressed
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event</param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
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
        /// <summary>
        /// Convert input string to his hashed value using SHA256 alghoritm
        /// </summary>
        /// <param name="phrase">input string</param>
        /// <returns>Hashed value of input string</returns>
        public string Sha256encrypt(string phrase)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            System.Security.Cryptography.SHA256Managed sha256hasher = new System.Security.Cryptography.SHA256Managed();
            byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(phrase));
            return Convert.ToBase64String(hashedDataBytes);
        }
    }
}
