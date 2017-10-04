/*
 * Name : Amanda Steidl
 * Course : Senior Project
 * Professor : Victor Miller
 * Project : Senior Project-Final
 * Due Date : October 4, 2017
 * Current File : DirectMessage.xaml.cs
 * Description : DirectMessage details and functionality. The design is located within DirectMessage.xaml
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
    /// Interaction logic for DirectMessage.xaml
    /// </summary>
    public partial class DirectMessage : Window
    {

        /// <summary>
        /// The parent of this window.
        /// </summary>
        private SemiTransOverlay m_parentWin = null;

        /// <summary>
        /// The client's username
        /// </summary>
        private string m_username = null;

        /// <summary>
        /// The destination client's username
        /// </summary>
        private string m_chatBuddy = null;

        /// <summary>
        /// Direct Message constructor when they're receiving a message from another client.
        /// </summary>
        /// <param name="a_parent">Window : the parent window, semiTransOverlay</param>
        /// <param name="a_chatWith">string : the chat buddy's client username</param>
        /// <param name="a_currUser">string : the current client's username</param>
        /// <param name="a_newMsg">string : the message being received from the chat buddy</param>
        public DirectMessage(Window a_parent, string a_chatWith, string a_currUser, string a_newMsg)
        {
            m_parentWin = a_parent as SemiTransOverlay;
            this.Owner = m_parentWin;
            m_username = a_currUser;
            m_chatBuddy = a_chatWith;
            InitializeComponent();
            this.Title = a_chatWith + " Direct Message";
            displayToUsr(a_chatWith + ": " + a_newMsg);
            this.Show();
        }

        /// <summary>
        /// Direct Message constructor when the current client is sending a message to another client.
        /// </summary>
        /// <param name="a_parent">Window : the parent window, semiTransOverlay</param>
        /// <param name="a_chatWith">string : the chat message's destination / chat buddy</param>
        /// <param name="a_currUser">string : the current client's username</param>
        public DirectMessage(Window a_parent, string a_chatWith, string a_currUser)
        {
            m_parentWin = a_parent as SemiTransOverlay;
            this.Owner = m_parentWin;
            m_username = a_currUser;
            m_chatBuddy = a_chatWith;
            InitializeComponent();
            this.Title = a_chatWith + " Direct Message";
            this.Show();
        }

        /// <summary>
        /// Send a message to the other user
        /// </summary>
        private void send()
        {
            string trimmed = txtMsgToSend.Text.Trim();
            if (!string.IsNullOrEmpty(trimmed))
            {
                //Use the parent's functionality to send the message through the server.
                m_parentWin.sendToDestination(m_chatBuddy, txtMsgToSend.Text);
                displayToUsr(m_username + ": " + txtMsgToSend.Text);
            }
            txtMsgToSend.Clear();
        }

        /// <summary>
        /// Display a full message including username + message to the user
        /// </summary>
        /// <param name="a_msg">string : a message in the format of username + message</param>
        public void displayToUsr(string a_msg)
        {
            txtDisplay.AppendText(a_msg + "\n");
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
        /// When the mouse hovers over the clickable area the image changes
        /// </summary>
        /// <param name="a_imgSend">Image : the area that is being hovered over</param>
        /// <param name="e">Unused in this function</param>
        private void btnSend_MouseEnter(object a_imgSend, MouseEventArgs e)
        {
            btnSend.Source = new BitmapImage(new Uri(@"/Resources/Chatroom/SendOnClick.png", UriKind.Relative));
        }

        /// <summary>
        /// When the mouse leaves the clickable area the image changes back to the original
        /// </summary>
        /// <param name="a_imgSend">Image : the area that is being watched</param>
        /// <param name="e">Unused in this function</param>
        private void btnSend_MouseLeave(object a_imgSend, MouseEventArgs e)
        {
            btnSend.Source = new BitmapImage(new Uri(@"/Resources/Chatroom/Send.png", UriKind.Relative));
        }
        
        /// <summary>
        /// Send message on pressing enter while within the textbox
        /// </summary>
        /// <param name="a_txtBox"></param>
        /// <param name="a_keyPressed">KeyEventsArgs : looking at the key pressed on the keyboard</param>
        private void keyDown(object a_txtBox, KeyEventArgs a_keyPressed)
        {
            if (a_keyPressed.Key == Key.Return)
            {
                send();
            }
        }

        /// <summary>
        /// Makes sure to keep the scroll at the bottom to show the most recent messages
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
