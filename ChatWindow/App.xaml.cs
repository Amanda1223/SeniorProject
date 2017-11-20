/*
 * Name : Amanda Steidl
 * Course : Senior Project
 * Professor : Victor Miller
 * Project : Senior Project-Final
 * Due Date : October 4, 2017
 * Current File : App.xaml.cs
 * Description : The start of the project, tells the project what to initiate first, then closes the application upon shutdown.
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace ChatWindow
{
    /// <summary>
    /// Function definitions mentioned in App.xaml
    ///     Such as start / close
    /// </summary>
    public partial class App : Application
    {

        SemiTransOverlay m_startWindow = null;

        /// <summary>
        /// When the application starts it will open the initial window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Create the startup window
            m_startWindow = new SemiTransOverlay();
        }

        /// <summary>
        /// When the application ends/closes the user will be disconnected and the application
        ///     will be shut down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            //Redundant code.m_startWindow.disconnect();
            m_startWindow.Close();
        }

       // TODO: MongoDB Connection
    }
}
