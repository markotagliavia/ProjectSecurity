﻿using System;
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
    /// Interaction logic for ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        private WCFClient proxy;
        private string email;

        public ChangePasswordWindow(WCFClient proxy, string email)
        {
            InitializeComponent();
            this.proxy = proxy;
            this.email = email;
        }

        /// <summary>
        /// submit new password and send data to server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changePassButton_Click(object sender, RoutedEventArgs e)
        {
            string oldPass = oldPassTextBox.Password;
            string newPass = newPassTextBox.Password;
            if (newPass.Length < 5)
            {
                MessageBox.Show("Too short new password!");
            }
            else if (newPass.Contains(" ") || newPass.Contains("\t"))
            {
                MessageBox.Show("Password should not contain white spaces!");
            }
            else if (String.IsNullOrWhiteSpace(newPass))
            {
                MessageBox.Show("You must fill this field!");
            }
            else if (newPass.Length > 30)
            {
                MessageBox.Show("Combination from 5 to 30 alphanumerics or/and special characters");
            }
            else
            {
                bool retVal = proxy.ChangePassword(email, oldPass, newPass);
                if (retVal == false)
                {
                    MessageBox.Show("Something went wrong! Password is not changed.");
                }
                else
                {
                    MessageBox.Show("Password is succesfully changed.");
                }
            }
            
        }

        /// <summary>
        /// Close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// When window is initialized, focus is on old pass field
        /// </summary>
        /// <param name="sender">email field</param>
        /// <param name="e">event</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            oldPassTextBox.Focus();
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
                changePassButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
    }
}