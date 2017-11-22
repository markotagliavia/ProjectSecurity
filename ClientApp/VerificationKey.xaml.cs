using SecurityManager;
using System;
using System.Text;
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
        private EncryptDecrypt aesCommander = new EncryptDecrypt();

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
            byte[] vkInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, verificationKey);
            string vkHash = Sha256encrypt(verificationKey);
            //send data to server
            bool i = proxy.SendVerificationKey(vkInBytes,vkHash);
            if(i == true)
            {
                //key is ok
                 var s = new GroupChat(proxy, email);
                 s.Show();
                 this.Close();
            }
            else
            {
                MessageBox.Show("Key is not valid! Please provide correct one. Check your email.");
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
