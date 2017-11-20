/*
 * Name : Amanda Steidl
 * Course : Senior Project
 * Professor : Victor Miller
 * Project : Senior Project-Final
 * Due Date : October 4, 2017
 * Current File : ServerMain.cs
 * Description : Server start, using implementation of services for the client.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{

    /// <summary>
    /// Class containing the server start, and keeps the connection open.
    /// This class holds:
    /// implementation of all services available to the client.
    /// </summary>
    class ServerMain
    {
        public static ImpServices testServ;

        /// <summary>
        /// Opens the server connection and the instance of the implementation of services.
        /// </summary>
        /// <param name="args">(none) string of arguments that coukd be passed in at program start. None in this case.</param>
        static void Main(string[] args)
        {
            testServ = new ImpServices();
            using(ServiceHost newHost = new ServiceHost(testServ))
            {
                newHost.Open();
                Console.WriteLine("Server is Online.");
                Console.ReadLine();

                newHost.Close();
            }

        }
    }
}

