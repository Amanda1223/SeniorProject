/*
 * Name : Amanda Steidl
 * Course : Senior Project
 * Professor : Victor Miller
 * Project : Senior Project-Final
 * Due Date : October 4, 2017
 * Current File : ImpServices.cs
 * Description : Implementation of the services provided to the user, server side.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Collections.Concurrent;
using Interfaces;


namespace Server
{
    /// <summary>
    /// Implementation for the operation contracts located in the Inter
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class ImpServices : IntServices
    {
        
        /// <summary>
        /// ConcurrentDictionary [string, ConnectedClient] : where the dictionary is populated with [string]usernames and their [connectedClient]connection to the server.
        /// </summary>
        public ConcurrentDictionary<string, ConnectedClient> m_connClients = new ConcurrentDictionary<string, ConnectedClient>();
        
        // Conccurency will protect conflicts/race conditions since the server is multi threaded.

        /// <summary>
        /// If the client picks a unique username, a connection will be made to the server.
        /// </summary>
        /// <param name="a_username"> string : the username requested by the client</param>
        /// <param name="a_userList"> List-strings : usernames to be a permanent copy for the
        /// client after a connection is made, passed by reference.</param>
        /// <returns>
        /// return 0 upon duplicate  username existing, does not fully connect.
        /// return -1 upon error connecting to the server.
        /// return 1 upon success.
        /// </returns>
        public int login(string a_username, ref List<string> a_userList)
        {

            // Make sure the user name does not already exist
            foreach (var client in m_connClients)
            {
                
                if(client.Key.ToLower() == a_username.ToLower())
                {
                    Console.WriteLine("Duplicate user information attempted.");
                    return 0;
                }
            }

            // Create a new client chat user, with their user name & their connection
            var establishedUserConnection = OperationContext.Current.GetCallbackChannel<IntClient>();
            ConnectedClient newClient = new ConnectedClient();
            newClient.connection = establishedUserConnection;

            // Checking username again for uniqueness (race conditions).
            newClient.username = a_username;
            Console.WriteLine("[ " +DateTime.Now.ToString("h:mm:ss tt") + " ]  " + a_username + " has connected to the server.");
            if (!m_connClients.TryAdd(a_username, newClient)) {
                Console.WriteLine("Failed to connected to server.");
                return -1;
            }

            // Initial list of users for each new client that connects.
            foreach (var client in m_connClients)
            {
                a_userList.Add(client.Key);
            }
            return 1;
        }

        /*************************** START: Message Sending ***************************/
        /// <summary>
        /// Sends a message to all currently connected clients, excluding the sender.
        /// </summary>
        /// <param name="a_username">string : user sending the message to the client list</param>
        /// <param name="a_msg">string : message to send to the client list.</param>
        public void sendToAll(string a_username, string a_msg)
        { 

            // Output the string with time, username, message to the server's console.
            Console.WriteLine("[ " + DateTime.Now.ToString("h:mm:ss tt") + " ]  " + a_username + " SENT < " + a_msg + " > TO ALL");

            // Pass the message to each client excluding the sender.
            foreach (var client in m_connClients)
            {
                if (client.Key.ToLower() != a_username.ToLower())
                {
                    client.Value.connection.frwdMsg(a_msg, a_username);
                }
            }
        }

        /// <summary>
        /// Sends a message from one client to another. Destination client needs to be connected to the server.
        /// If the destination client is offline or does not exist, the user is unable to send messages.
        /// </summary>
        /// <param name="a_source">string : the client requesting to send a message.</param>
        /// <param name="a_dest">string : the final destination of the message.</param>
        /// <param name="a_msg">string : the message to send to the <paramref name="a_dest"/> client.</param>
        public void sendToUser(string a_source, string a_dest, string a_msg)
        {
            ConnectedClient getValue;
            Console.WriteLine("[ " + DateTime.Now.ToString("h:mm:ss tt") + " ]  " + a_source + " SENT < " + a_msg + " > TO " + a_dest);

            // If the destination client exists, will be stored in getValue
            bool isValue = m_connClients.TryGetValue(a_dest, out getValue);
            if (isValue)
            {

                // Using the callback function to whisper the message to the correct destination client
                getValue.connection.whisperMsg(a_msg, a_source);
            }
        }
        /*************************** END: Message Sending ***************************/

        /*************************** START: Dis/Connection ***************************/
        /// <summary>
        /// When a different client connects to the server, the clients currently connected
        /// will receive an update to their client lists so they are aware of the change.
        /// </summary>
        /// <param name="a_connected">string : user that had connected.</param>
        public void newClientConnection(string a_connected)
        {
            if ((m_connClients).Count <= 1)
            {
                return;
            }

            // If something is wrong with the client's username connecting.
            if (a_connected == null || a_connected == "")
            {
                return;
            }
            foreach (var client in m_connClients)
            {

                // Pass the client to everyone else
                if (client.Key.ToLower() != a_connected.ToLower())
                {
                    client.Value.connection.addUserToList(a_connected);
                }
            }
        }

        /// <summary>
        /// Ensures the user disconnects properly while updating the disconnection
        /// to the other connected clients.
        /// </summary>
        /// <param name="a_disconnected">string : user that is requesting a disconnect</param>
        public void disconnectionHandle(string a_disconnected)
        {

            // if the user currently exists, attempt to remove
            ConnectedClient userInfo;
            if (m_connClients.TryGetValue(a_disconnected, out userInfo))
            {

                // if the client can be removed from the server list
                ConnectedClient tmp;
                if(m_connClients.TryRemove(a_disconnected, out tmp))
                {
                    Console.WriteLine(a_disconnected + " has left the server.");
                }
                else
                {
                    Console.WriteLine("Unknown Failure removing: " + a_disconnected + " from the server.");
                    return;
                }
            }else
            {
                Console.WriteLine("Error Disconnecting: " + a_disconnected + " from the server.");
                return;
            }

            // Callback action : update other clients
            foreach (var client in m_connClients)
            {

                // client is already removed, do not need to worry about checking for him i ntthe list
                client.Value.connection.removeUserFromList(a_disconnected);
            }
        }

        /*************************** END: Dis/Connection ***************************/
    }
}
