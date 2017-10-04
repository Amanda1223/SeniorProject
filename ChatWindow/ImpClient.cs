/*
 * Name : Amanda Steidl
 * Course : Senior Project
 * Professor : Victor Miller
 * Project : Senior Project-Final
 * Due Date : October 4, 2017
 * Current File : ImpClient.cs
 * Description : The implementation of the client interfaces.
 */

using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatWindow
{
    /// <summary>
    /// Class containing the interfaces to the client.
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ImpClient : IntClient
    {
        /// <summary>
        /// The message goes from the server to the client end. This will send a message to the windows.
        /// </summary>
        /// <param name="a_message">string : message being forwarded</param>
        /// <param name="a_user">string : the user sending the message</param>
        public void frwdMsg(string a_message, string a_user)
        {

            // Display message to the text box, format the message
            ((SemiTransOverlay)Application.Current.MainWindow).messagePass(a_user + " : " + a_message);
        }

        /// <summary>
        /// When receiving a message from another user, this function will send the message
        ///     to the appropriate window.
        /// </summary>
        /// <param name="a_msg">string : message to be displayed</param>
        /// <param name="a_source">string : the client's username of who the message is from</param>
        public void whisperMsg(string a_msg, string a_source)
        {
            ((SemiTransOverlay)Application.Current.MainWindow).incomingMessage(a_source, a_msg);
            return;
        }

        /// <summary>
        /// Adding a new user to the client-side list. Information supplied from the server.
        /// </summary>
        /// <param name="a_connected">string : the new client's username who has connected</param>
        public void addUserToList(string a_connected)
        {
            ((SemiTransOverlay)Application.Current.MainWindow).addNewConnectionList(a_connected);
        }

        /// <summary>
        /// Removing a client who has disconnected from the list of usernames
        /// </summary>
        /// <param name="a_disconnected">string : the client's username who has disconnected.</param>
        public void removeUserFromList(string a_disconnected)
        {
            ((SemiTransOverlay)Application.Current.MainWindow).removeFromConnected(a_disconnected);
        }
    }
}