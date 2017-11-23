using SecurityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.IO;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Xml;
using System.Security.Cryptography;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WCFClient proxy;
        private EncryptDecrypt aesCommander = new EncryptDecrypt();

        public MainWindow()
        {
            string ip = "";
            string port = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load((System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"NetworkConfig.xml")).ToString());
                ip = doc.GetElementsByTagName("IPAddress")[0].InnerXml;
                port = doc.GetElementsByTagName("Port")[0].InnerXml;
            }
            catch (Exception ex)
            {
                MessageBox.Show("NetworkConfig file is missing!");
                this.Close();
            }
            NetTcpBinding binding = new NetTcpBinding();
            string address = $"net.tcp://{ip}:{port}/ServiceApp";
            InstanceContext instanceContext;
            ChatServiceCallback chatServiceCallback = new ChatServiceCallback();
            //chatServiceCallback.ClientNotified += ChatServiceCallback_ClientNotified;

            instanceContext = new InstanceContext(chatServiceCallback);
            bool exit = false;
            do
            {
                try
                {
                    this.proxy = new WCFClient(instanceContext, binding, new EndpointAddress(new Uri(address)));
                    exit = true;
                }
                catch (Exception e)
                {
                    var result = MessageBox.Show("Do you want to try to establish connection again?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                   
                    if (result == MessageBoxResult.Yes)
                    {
                        exit = false;
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        exit = true;
                    }
                }
            } while (exit == false);
            RSAParameters pom = new RSAParameters();
           
            do
            {
                try
                {
                    pom = this.proxy.GetPublicKey(this.proxy.Guid);
                    
                    exit = true;
                }
                catch (Exception e)
                {
                    var result = MessageBox.Show("Do you want to try to establish connection again?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        exit = false;
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        exit = true;
                    }
                }
                if (pom.Exponent == null && exit == true)
                {
                    var result = MessageBox.Show("Do you want to try to establish connection again?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        exit = false;
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        exit = true;
                    }
                }
            } while (pom.Exponent == null && exit == false);

            this.proxy.Guid = Guid.NewGuid();
            this.proxy.Aes = new GenerateAesKey(pom, 2048);

            if (this.proxy.SendSessionKey(this.proxy.Aes.AesEncryptedSessionKey))
            {
                InitializeComponent();
            }
            else
            {
                MessageBox.Show("Session is in danger. Possibly someone broke into it. We will shutdown connection to prevent this danger.");
                this.proxy.Close();
            }


            Console.ReadLine();
        }

        /// <summary>
        /// When window is initialized, focus is on email field
        /// </summary>
        /// <param name="sender">email field</param>
        /// <param name="e">event</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBox1.Focus();
        }

        /// <summary>
        /// When button Register is clicked, Register window pop up
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event</param>
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            var s = new NewUserWindow(proxy);
            this.Close();
            s.Show();
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
                button1.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        /// <summary>
        /// Login button is clicked and we should send data to server
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event</param>
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string user = textBox1.Text;
            string pass = passwordBox1.Password;
            String cryptedPass = Sha256encrypt(pass);

                if (String.IsNullOrWhiteSpace(user) || String.IsNullOrWhiteSpace(pass))
                {
                    SystemSounds.Hand.Play();
                    MessageBox.Show("Please fill all fields!");
                    return;
                }

            //treba izmeniti login da radi bez verifikacionog koda i da server vrati odgovor(int {-1 lose, 1 odma login, 0 unesi kod}) da li je potrebno da se unese
            byte[] userInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, user);
            byte[] passwordInBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, cryptedPass);
            string userHash = Sha256encrypt(user);
            string passwordHash = Sha256encrypt(cryptedPass);

            int i = proxy.LogIn(userInBytes, passwordInBytes, userHash, passwordHash);
            if (i == 0)
            {
                //prvi put se loguje i mora da se unese i kod
                var r = new VerificationKey(proxy, user);
                r.Show();
                this.Close();
            }
            else if (i == -1)
            {
                MessageBox.Show("Email and password are incorrect! Try again or reset your password.");
            }
            else if (i == 1)
            {
                //uspesan login
                //Send data to server
                var s1 = new GroupChat(proxy, user);    //Forum starting if data is ok
                SystemSounds.Asterisk.Play();
                s1.Show();
                this.Close();
            }
            else if (i == -3)
            {
                MessageBox.Show("Someone modified your messages, for your security, we will prevent further program execution.");
                this.Close();
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

        /// <summary>
        /// Reset password for enetered email
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetPassButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] email = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, textBox1.Text);
            string passwordHash = Sha256encrypt(textBox1.Text);
            if (!String.IsNullOrWhiteSpace(textBox1.Text))
            {
                int i = proxy.ResetPassword(email,passwordHash);
                if (i == -1)
                {
                    MessageBox.Show("Please provide correct email address.");
                }
            }
        }
    }
}
