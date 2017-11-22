using ClientApp;
using SecurityManager;
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


namespace ClientApp
{
   
    /// <summary>
    /// Interaction logic for NewUserWindow.xaml
    /// </summary>
    public partial class NewUserWindow : Window
    {
        WCFClient proxy;
        private EncryptDecrypt aesCommander = new EncryptDecrypt(); 

        public NewUserWindow(WCFClient proxy)
        {
            InitializeComponent();
            this.proxy = proxy;
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
            string name = nameTextbox.Text;
            string lastName = lastNameTextbox.Text;
            string gender = ((bool)radioButtonMale.IsChecked) ? "Male" : "Female";
            DateTime birthDate = DateTime.Now;
            if (!String.IsNullOrWhiteSpace(dateTextBox.Text))
            {
                try
                {
                    if (DateTime.Parse(dateTextBox.Text) != null)
                    {
                        birthDate = DateTime.Parse(dateTextBox.Text);
                    }
                }
                catch (Exception ex) { }
            }

            passwordBox1.Focus();
            passwordBox2.Focus();
            nameTextbox.Focus();
            lastNameTextbox.Focus();
            dateTextBox.Focus();
            button2.Focus();


            var regexItem = new Regex(@"^[a-zA-Z0-9@.]+$");
            int countMonkey = textBox1.Text.Where(c => c == '@').Count();
            int countDot = textBox1.Text.Where(c => c == '.').Count();

            if (pass.Equals(repass) && user.Length >= 5 && pass.Length >= 5 &&
                !user.Contains(" ") && !user.Contains("\t") && regexItem.IsMatch(user) &&
                !pass.Contains(" ") && !pass.Contains("\t") &&
                !repass.Contains(" ") && !repass.Contains("\t") && name.Length >= 2 && lastName.Length >= 2 && !String.IsNullOrWhiteSpace(dateTextBox.Text))
            {
                //Valid data. Send it to server
                //kriptovati sifru
                byte[] nameBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, name);
                byte[] lastNameBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, lastName);
                byte[] birthDateBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, birthDate.ToString());
                byte[] genderBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, gender);
                byte[] userBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, user);
                byte[] passBytes = aesCommander.EncryptData(this.proxy.Aes.MySessionkey, Sha256encrypt(pass));

                proxy.Registration(nameBytes, lastNameBytes, birthDateBytes, genderBytes, userBytes, passBytes, Sha256encrypt(name), Sha256encrypt(lastName), Sha256encrypt(birthDate.ToString()), Sha256encrypt(gender), Sha256encrypt(user), Sha256encrypt(Sha256encrypt(pass)));

                this.Close();
            }
            else if (!pass.Equals(repass))
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("Passwords doesn't match!", "Error");
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
            else if (!regexItem.IsMatch(name))
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("Name can not contain special characters", "Error");
            }
            else if (!regexItem.IsMatch(lastName))
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("Last name can not contain special characters", "Error");
            }
            else if (String.IsNullOrWhiteSpace(name))
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("Name can not contain special characters", "Error");
            }
            else if (String.IsNullOrWhiteSpace(lastName))
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("Last name can not contain special characters", "Error");
            }
            else if (lastName.Length < 2)
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("Too short Last name", "Error");
            }
            else if (name.Length < 2)
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("Too short name", "Error");
            }
            else if (String.IsNullOrWhiteSpace(dateTextBox.Text))
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("Date is not valid", "Error");
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

        /// <summary>
        /// When focus on name is lost, textBox should change color border
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nameTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            var regexItem = new Regex(@"^[a-zA-Z0-9@.]+$");

            if (nameTextbox.Text.Contains("\t"))
            {
                nameTextbox.BorderBrush = new SolidColorBrush(Colors.Red);
                nameTextbox.ToolTip = "E-mail should not contain tabs!";
            }
            else if (String.IsNullOrWhiteSpace(nameTextbox.Text))
            {
                nameTextbox.BorderBrush = new SolidColorBrush(Colors.Red);
                nameTextbox.ToolTip = "You must fill this field!";
            }
            else if (!regexItem.IsMatch(nameTextbox.Text))
            {
                nameTextbox.BorderBrush = new SolidColorBrush(Colors.Red);
                nameTextbox.ToolTip = "Name can not contain special characters!";
            }
            else
            {
                nameTextbox.BorderBrush = new SolidColorBrush(Colors.Green);
                nameTextbox.ToolTip = "Combination of 2 to 30 alphabetic charactes ";
            }
        }

        /// <summary>
        /// When focus on last name is lost, textBox should change color border
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lastNameTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            var regexItem = new Regex(@"^[a-zA-Z0-9@.]+$");

            if (lastNameTextbox.Text.Contains("\t"))
            {
                lastNameTextbox.BorderBrush = new SolidColorBrush(Colors.Red);
                lastNameTextbox.ToolTip = "Last name should not contain tabs!";
            }
            else if (String.IsNullOrWhiteSpace(lastNameTextbox.Text))
            {
                lastNameTextbox.BorderBrush = new SolidColorBrush(Colors.Red);
                lastNameTextbox.ToolTip = "You must fill this field!";
            }
            else if (!regexItem.IsMatch(nameTextbox.Text))
            {
                lastNameTextbox.BorderBrush = new SolidColorBrush(Colors.Red);
                lastNameTextbox.ToolTip = "Last name can not contain special characters!";
            }
            else
            {
                lastNameTextbox.BorderBrush = new SolidColorBrush(Colors.Green);
                lastNameTextbox.ToolTip = "Combination of 2 to 30 alphabetic charactes ";
            }
        }

        /// <summary>
        /// When focus on date is lost, textBox should change color border
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var regexItem = new Regex(@"^[a-zA-Z0-9@.]+$");

            if (dateTextBox.Text.Contains("\t"))
            {
                dateTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                dateTextBox.ToolTip = "Date should not contain tabs!";
            }
            else if (String.IsNullOrWhiteSpace(dateTextBox.Text))
            {
                dateTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                dateTextBox.ToolTip = "You must fill this field!";
            }
            else if (!regexItem.IsMatch(dateTextBox.Text))
            {
                dateTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                dateTextBox.ToolTip = "Date can not contain special characters!";
            }
            else
            {
                dateTextBox.BorderBrush = new SolidColorBrush(Colors.Green);
                dateTextBox.ToolTip = "XX.XX.XXXX.";
            }
        }

        public string Sha256encrypt(string phrase)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            System.Security.Cryptography.SHA256Managed sha256hasher = new System.Security.Cryptography.SHA256Managed();
            byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(phrase));
            return Convert.ToBase64String(hashedDataBytes);
        }
    }
}
