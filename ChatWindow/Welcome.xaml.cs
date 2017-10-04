/*
 * Name : Amanda Steidl
 * Course : Senior Project
 * Professor : Victor Miller
 * Project : Senior Project-Final
 * Due Date : October 4, 2017
 * Current File : Welcome.xaml.cs
 * Description : Welcome window's details and functionality. The design is located within Window.xaml
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
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome : Window
    {
        /// <summary>
        /// The custom event to send a notification back to the parent window
        ///     so the overlay can take care of another action.
        /// </summary>
        public event EventHandler<bool> RaiseCustomEvent;

        /// <summary>
        /// The parent window to the current window being used [welcome window]
        /// </summary>
        private SemiTransOverlay m_parentWin = null;

        /// <summary>
        /// Welcome window constructor, simply sets the screen to welcome the current client to the application
        /// </summary>
        /// <param name="a_parent">Window : the window that this belongs to</param>
        /// <param name="a_username">string : the username of the current client</param>
        public Welcome(Window a_parent, string a_username)
        {
            m_parentWin = a_parent as SemiTransOverlay;
            this.Owner = m_parentWin;
            InitializeComponent();
            lblUserWelcome.Content = a_username;
            this.Show();
        }

        /// <summary>
        /// When the welcome window closes, sets off another event.
        /// </summary>
        /// <param name="a_btn">object : a button that had been clicked</param>
        /// <param name="e"></param>
        private void btnCloseWelcome_Click(object a_btn, RoutedEventArgs e)
        {
            welcomeClosed();
        }

        /// <summary>
        /// When the welcome window is closed the custom event is raised to let the 
        ///     semi-transparent overlay know that it shoulud begin it's next event.
        /// </summary>
        private void welcomeClosed()
        {
            RaiseCustomEvent(this, true);
            if (this.IsVisible)
            {
                this.Close();
            }
        }
    }
}