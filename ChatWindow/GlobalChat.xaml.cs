/*
 * Name : Amanda Steidl
 * Course : Senior Project
 * Professor : Victor Miller
 * Project : Senior Project-Final
 * Due Date : October 4, 2017
 * Current File : GlobalChat.xaml.cs
 * Description : Global chat's details and functionality. The design is located within GlobalChat.xaml
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace ChatWindow
{
    /// <summary>
    /// Interaction logic for GlobalChat.xaml
    /// </summary>
    public partial class GlobalChat : Window
    {

        /// <summary>
        /// The initializer of the parent window to this class
        /// </summary>
        private SemiTransOverlay m_parentWin = null;

        /// <summary>
        /// Initializer of the username
        /// </summary>
        private string m_username = null;

        /// <summary>
        /// Initializer of the observable collection that will be used as the client's username list
        ///     provided by the server/semi-transparent overlay
        /// </summary>
        private ObservableCollection<string> m_userList;

        /// <summary>
        /// Constructor for this window, sets up the list and parent window
        /// </summary>
        /// <param name="a_parent">Window : the parent window to this window, semiTransOverlay</param>
        /// <param name="a_user">string : the username of the current client</param>
        /// <param name="a_globalList">List[string] : all the user's that have been connected to the server</param>
        public GlobalChat(Window a_parent, string a_user, List<string> a_globalList)
        { 
            m_parentWin = a_parent as SemiTransOverlay;
            this.Owner = m_parentWin;
            m_username = a_user;
            InitializeComponent();
            setListValues(a_globalList);
            this.Show();
        }

        /// <summary>
        /// Updates a new user connecting by adding them to the <paramref name="m_userList"/>
        /// </summary>
        /// <param name="a_connected">string : the username of who had just connected</param>
        public void updateJoin(string a_connected)
        {
            if (a_connected == null)
            {
                return;
            }
            m_userList.Add(a_connected);
        }

        /// <summary>
        /// Updates the client by removing the user who had just left using <paramref name="m_userList"/>
        /// </summary>
        /// <param name="a_disconnected">string : the user who has disconnected.</param>
        public void updateDisconnect(string a_disconnected)
        {
            if(a_disconnected == null)
            {
                return;
            }
            m_userList.Remove(a_disconnected);
        }

        /// <summary>
        /// Sets the initial list of user's that are connected.
        /// </summary>
        /// <param name="a_connections">List[string] : the initial list of client username's connected to the server</param>
        public void setListValues(List<string> a_connections)
        {
            if (a_connections == null)
            {
                return;
            }

            // Initializes the collection used by the listbox
            m_userList = new ObservableCollection<string> (a_connections);
            lstNames.ItemsSource = m_userList;
            lstNames.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
        }

        /// <summary>
        /// Displays message to the user
        /// </summary>
        /// <param name="a_msg">string : the message to be displayed</param>
        public void displayToUsr(string a_msg)
        {
            txtDisplay.AppendText(a_msg + "\n");
        }


        /// <summary>
        /// When the mouse hovers over the clickable area the image changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_MouseEnter(object sender, MouseEventArgs e)
        {
            btnSend.Source = new BitmapImage(new Uri(@"/Resources/Chatroom/SendOnClick.png", UriKind.Relative));
        }

        /// <summary>
        /// When the mouse leaves the clickable area the image changes back to the original
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_MouseLeave(object sender, MouseEventArgs e)
        {
            btnSend.Source = new BitmapImage(new Uri(@"/Resources/Chatroom/Send.png", UriKind.Relative));
        }

        /// <summary>
        /// Send Message Command on Mouse Down on the Image
        /// </summary>
        /// <param name="a_imgSend">Image : when clicking on the image, send the information</param>
        /// <param name="e">Unused in this function</param>
        private void btnSend_Press(object a_imgSend, MouseButtonEventArgs e)
        {
            send();
        }

        /// <summary>
        /// Manages the sending of messages
        /// </summary>
        private void send()
        {
            string trimmed = txtMsgToSend.Text.Trim();
            if (!String.IsNullOrEmpty(trimmed))
            {
                m_parentWin.sendAllConnection(txtMsgToSend.Text);
                displayToUsr(m_username + ": " + txtMsgToSend.Text);
            }
            txtMsgToSend.Clear();
        }

        /// <summary>
        ///  When pressing the enter key to send a message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="a_keyPressed">KeyEventsArgs : looking at the key pressed on the keyboard</param>
        private void keyDown(object sender, KeyEventArgs a_keyPressed)
        {
            if (a_keyPressed.Key == Key.Return)
            {
                send();
            }
        }

        /// <summary>
        /// When the global chat is closed, it has to be known to the parentWin that it no longer
        ///     exists :: therefore, it can start a new instance the correct way through the G button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleGlobal(object sender, EventArgs e)
        {
            //upon window closed
            m_parentWin.globalStatus(false);
        }
        
        /// <summary>
        /// Open the direct message chat for the correct selected user, and allow the parent
        ///     to handle creation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openDirectMessage(object sender, RoutedEventArgs e)
        {
            if (lstNames.SelectedIndex == -1)
            {
                return;
            }
            string userDest = lstNames.SelectedItem.ToString();

            // parent window opening direct message window chat.
            m_parentWin.openDM(userDest);
        }

        /// <summary>
        /// Make sure to keep the scroll at the bottom to show the most recent recieved message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextChanged_ScrollHandle(object sender, TextChangedEventArgs e)
        {
            txtDisplay.ScrollToEnd();
            return;
        }
    }
}
