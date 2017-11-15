using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for NewUserWindow.xaml
    /// </summary>
    public partial class NewUserWindow : Window
    {
        public NewUserWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When window is loaded, focus should be placed on e-mail textBox
        /// </summary>
        /// <param name="sender">window</param>
        /// <param name="e">event</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBox1.Focus();
        }

        /// <summary>
        /// Close button clicked, window is closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// When window is closing, LoginWindow should appear
        /// </summary>
        /// <param name="sender">window</param>
        /// <param name="e">event</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var s = new MainWindow();
            s.Show();
        }

        /// <summary>
        /// When focus from email textBox is lost, we should check validity of input and set border of textBox to red if input is incorrect,
        /// and to green if it is correct
        /// </summary>
        /// <param name="sender">textBox</param>
        /// <param name="e">event</param>
        private void textBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            var regexItem = new Regex(@"^[a-zA-Z0-9@.]+$");
            int countMonkey = textBox1.Text.Where(c => c == '@').Count();
            int countDot = textBox1.Text.Where(c => c == '.').Count();

            if (textBox1.Text.Length < 5)
            {
                textBox1.BorderBrush = new SolidColorBrush(Colors.Red);
                textBox1.ToolTip = "E-mail is too short!";
            }
            else if (countMonkey <= 0 || countMonkey > 1)
            {
                textBox1.BorderBrush = new SolidColorBrush(Colors.Red);
                textBox1.ToolTip = "E-mail should have one @ separator!";
            }
            else if (countDot <= 0 || countDot > 2)
            {
                textBox1.BorderBrush = new SolidColorBrush(Colors.Red);
                textBox1.ToolTip = "E-mail should have 1-2 comma separator!";
            }
            else if (textBox1.Text.Contains(" ") || textBox1.Text.Contains("\t"))
            {
                textBox1.BorderBrush = new SolidColorBrush(Colors.Red);
                textBox1.ToolTip = "E-mail should not contain white spaces!";
            }
            else if (String.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.BorderBrush = new SolidColorBrush(Colors.Red);
                textBox1.ToolTip = "You must fill this field!";
            }
            else if (!regexItem.IsMatch(textBox1.Text))
            {
                textBox1.BorderBrush = new SolidColorBrush(Colors.Red);
                textBox1.ToolTip = "E-mail can not contain special characters except @ separator!";
            }
            else
            {
                textBox1.BorderBrush = new SolidColorBrush(Colors.Green);
                textBox1.ToolTip = "Combination of 5 to 30 alphanumerics charactes with @ separator";
            }
        }

        /// <summary>
        /// When focus from password textBox is lost, we should check validity of input and set border of textBox to red if input is incorrect,
        /// and to green if it is correct
        /// </summary>
        /// <param name="sender">textBox</param>
        /// <param name="e">event</param>
        private void passwordBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (passwordBox1.Password.Length < 5)
            {
                passwordBox1.BorderBrush = new SolidColorBrush(Colors.Red);
                passwordBox1.ToolTip = "Password should have from 5 to 30 alphanumerics or/and special characters!";
            }
            else if (passwordBox1.Password.Contains(" ") || passwordBox1.Password.Contains("\t"))
            {
                passwordBox1.BorderBrush = new SolidColorBrush(Colors.Red);
                passwordBox1.ToolTip = "Password should not contain white spaces!";
            }
            else if (String.IsNullOrWhiteSpace(passwordBox1.Password))
            {
                passwordBox1.BorderBrush = new SolidColorBrush(Colors.Red);
                passwordBox1.ToolTip = "You must fill this field!";
            }
            else
            {
                passwordBox1.BorderBrush = new SolidColorBrush(Colors.Green);
                passwordBox1.ToolTip = "Combination from 5 to 30 alphanumerics or/and special characters";
            }
        }

        /// <summary>
        /// When focus from Repeat password textBox is lost, we should check validity of input and set border of textBox to red if input is incorrect,
        /// and to green if it is correct
        /// </summary>
        /// <param name="sender">textBox</param>
        /// <param name="e">event</param>
        private void passwordBox2_LostFocus(object sender, RoutedEventArgs e)
        {
            if (passwordBox2.Password.Length < 5)
            {
                passwordBox2.BorderBrush = new SolidColorBrush(Colors.Red);
                passwordBox2.ToolTip = "Password should have from 5 to 30 alphanumerics or/and special characters!";
            }
            else if (passwordBox2.Password.Contains(" ") || passwordBox2.Password.Contains("\t"))
            {
                passwordBox2.BorderBrush = new SolidColorBrush(Colors.Red);
                passwordBox2.ToolTip = "Password should not contain white spaces!";
            }
            else if (String.IsNullOrWhiteSpace(passwordBox2.Password))
            {
                passwordBox2.BorderBrush = new SolidColorBrush(Colors.Red);
                passwordBox2.ToolTip = "You must fill this field!";
            }
            else
            {
                passwordBox2.BorderBrush = new SolidColorBrush(Colors.Green);
                passwordBox2.ToolTip = "Combination from 5 to 30 alphanumerics or/and special characters";
            }
        }

        /// <summary>
        /// Button for registration clicked and this function is called
        /// Proccess input and send data to server or show error
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event</param>
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            string user = textBox1.Text;
            string pass = passwordBox1.Password;
            string repass = passwordBox2.Password;

            passwordBox1.Focus();
            passwordBox2.Focus();
            button2.Focus();

            var regexItem = new Regex(@"^[a-zA-Z0-9@.]+$");
            int countMonkey = textBox1.Text.Where(c => c == '@').Count();
            int countDot = textBox1.Text.Where(c => c == '.').Count();

            if (pass.Equals(repass) && !user.Equals("admin") && user.Length >= 5 && pass.Length >= 5 &&
                !user.Contains(" ") && !user.Contains("\t") && regexItem.IsMatch(user) &&
                !pass.Contains(" ") && !pass.Contains("\t") &&
                !repass.Contains(" ") && !repass.Contains("\t"))
            {
                //Valid data. Send it to server
                              
                this.Close();
            }
            else if (!pass.Equals(repass))
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("Passwords doesn't match!", "Error");
            }
            else if (user.Equals("admin"))  //precoded username, wil be deleted later
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("Forbidden username!", "Error");
            }
            else if (countMonkey <= 0 || countMonkey > 1)
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("E-mail should have one @ separator!");
            }
            else if (countDot <= 0 || countDot > 2)
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("E-mail should have 1-2 comma separator!");
            }
            else if (user.Length < 5)
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("Email seems like it is not valid", "Error");
            }
            else if (pass.Length < 5)
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("Too short password", "Error");
            }
            else if (user.Contains(" ") || user.Contains("\t"))
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("E-mail can not contain white spaces!", "Error");
            }
            else if (pass.Contains(" ") || pass.Contains("\t") ||
                repass.Contains(" ") || repass.Contains("\t"))
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("Password can not contain white spaces", "Error");
            }
            else if (!regexItem.IsMatch(user))
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("E-mail can not contain special characters", "Error");
            }
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
                button2.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
    }
}
