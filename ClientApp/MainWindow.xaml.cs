﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WCFClient proxy;


        public MainWindow()
        {
            InitializeComponent();
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/ServiceApp";

            using (proxy = new WCFClient(binding, new EndpointAddress(new Uri(address))))
            {
               

           
            }

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

            if (user.Equals("admin") && pass.Equals("admin"))
            {
                SystemSounds.Asterisk.Play();

                proxy.LogIn(user,pass,"code");// OVDE TREBA KOD



                //Send data to server
                var s = new GroupChat(proxy);    //Forum starting if data is ok
                SystemSounds.Asterisk.Play();
                s.Show();
                this.Close();
            }
            else
            {
                if (user.Equals("admin") && !pass.Equals("admin"))      //Delete later, precoded user and pass
                {
                    SystemSounds.Hand.Play();
                    MessageBox.Show("Wrong password!");
                    return;
                }

                if (String.IsNullOrWhiteSpace(user) || String.IsNullOrWhiteSpace(pass))
                {
                    SystemSounds.Hand.Play();
                    MessageBox.Show("Please fill all fields!");
                    return;
                }

                //Send input data to server
                var s = new GroupChat(proxy);    //Forum starting if data is ok
                SystemSounds.Asterisk.Play();
                s.Show();
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
    }
}
