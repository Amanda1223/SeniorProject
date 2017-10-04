/*
 * Name : Amanda Steidl
 * Course : Senior Project
 * Professor : Victor Miller
 * Project : Senior Project-Final
 * Due Date : October 4, 2017
 * Current File : Login.xaml.cs
 * Description : Login window details and functionality. The design is located within Login.xaml
 */

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

namespace ChatWindow
{
    /// <summary>
    /// Interaction logic for LogIn.xaml
    /// </summary>
    public partial class LogIn : Window
    {
        /// <summary>
        /// SemiTransOverlay : maintain a reference to the parent window to use function calls,
        ///     as well as custom events.
        /// </summary>
        private SemiTransOverlay m_parentWin = null;
        
        /// <summary>
        /// Custom events to gain the username for the semi-transparent overlay parent window.
        /// </summary>
        public event EventHandler<List<String>> RaiseCustomEvent;

        /// <summary>
        /// Login constructor, sets up the parent window as well as other components attached to the window
        /// at the start.
        /// </summary>
        /// <param name="a_parent"> Window : the parent window being passed in at creation. [SemiTransOverlay]</param>
        public LogIn(Window a_parent)
        {
            // must have the semi-transparent overlay as parent for each window created.
            m_parentWin = a_parent as SemiTransOverlay;
            this.Owner = m_parentWin;
            InitializeComponent();
            
            // settings for the window that may be changed in the future.
            txtPassword.IsReadOnly = true;
            txtPassword.IsEnabled = false;
            txtPassword.Focusable = false;

            // size of the window based upon the parent's dimensions (screen dimensions)
            this.Left = m_parentWin.Left + (m_parentWin.Width - this.ActualWidth) / 2;
            this.Top = m_parentWin.Top + (m_parentWin.Height - this.ActualHeight) / 2;
            this.Show();
        }

        /// <summary>
        /// Initial focus of the chatbox, the text normally eill say Username
        ///  however, when the textbox is focused it should be cleared..
        /// </summary>
        /// <param name="a_sender">login window is the sender.</param>
        /// <param name="e"> Unused in this function</param>
        private void txtUserName_GotFocus(object a_sender, RoutedEventArgs e)
        {
            if (txtUserName.Text == "Username")
            {
                txtUserName.Clear();
            }
            txtUserName.SelectionStart = txtUserName.Text.Length;
        }

        /// <summary>
        /// Change username to lower before being compared and sent to other windows.
        /// </summary>
        /// <param name="a_sender"> TextBox : username's textbox</param>
        /// <param name="e"> Unused in this function</param>
        private void txtUserName_LostFocus(object a_sender, RoutedEventArgs e)
        {
            txtUserName.Text = txtUserName.Text.ToLower();
        }

        /// <summary>
        ///  On a keydown event this will begin. Looking for the enter press to proc
        ///      the load in event.
        /// </summary>
        /// <param name="a_sender"> TextBox : username text box</param>
        /// <param name="e"> keyboard keys: "Key" entered to the keyboard</param>
        private void txtPassword_KeyDown(object a_sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                loginAttempt();
            }
        }

        /// <summary>
        /// When clicking on the image, this event will occur. Then there will be
        ///     a call to the login attempt.
        /// </summary>
        /// <param name="a_sender">Image : login button image. </param>
        /// <param name="e"> Unused in this function</param>
        private void btnLogin_MouseDown(object a_sender, MouseButtonEventArgs e)
        {
            loginAttempt();
        }

        /// <summary>
        ///  Function to send back the user information to the parent window.
        /// </summary>
        private void loginAttempt()
        {
            List<String> userInfo = new List<String>();
            userInfo.Add(txtUserName.Text.ToLower());

            // Custom event goes back to the event handler within SemiTransOverlay
            RaiseCustomEvent(this, userInfo);
            this.Close();
        }
    }
}
