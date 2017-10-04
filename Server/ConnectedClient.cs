using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectedClient
    {
        /// <summary>
        /// connection from the client to the services. Will be accessed to interact with other clients.
        /// </summary>
        public IntClient connection;

        /// <summary>
        /// get and set the username for the client being handled.
        /// </summary>
        public string username { get; set; }
    }
}
