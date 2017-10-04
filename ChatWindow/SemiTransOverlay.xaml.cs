/*
 * Name : Amanda Steidl
 * Course : Senior Project
 * Professor : Victor Miller
 * Project : Senior Project-Final
 * Due Date : October 4, 2017
 * Current File : SemiTransOverlay.xaml.cs
 * Description : Communication with the client windows and the server is located in this function.
 *      Request/Responses are within this document.
 */

using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChatWindow
{

    /// <summary>
    /// Interaction logic for semiTransOverlay.xaml
    /// </summary>
    public partial class SemiTransOverlay : Window
    {

        /// <summary>
        /// Class containing username and password indices for use throughout the window.
        /// </summary>
        private class IndexOfConnectionInfo
        {
            public int username = 0;
            public int password = 1;
        }

        /// <summary>
        /// Contains session information based on what state the program is in:
        ///     (1) connected to global chat
        ///     (2) if the window is focused
        ///     (3) if the window is toggled
        ///     (4) the clients connected to the session
        ///     (5) current client's username
        ///     etc.
        /// </summary>
        private class SessionStatus
        {
            public bool isVisible = true;
            public bool isWinFocus;
            public bool isGlobalOpen;
            public GlobalChat globalChatbox = null;
            public List<string> globalList = new List<string>();
            public string currentUser = null;
        }

        /// <summary>
        /// IntServices : the style of services allowed by the m_serverConnection
        /// </summary>
        public static IntServices m_serverConnection;

        /// <summary>
        /// DuplexChannelFactory[IntServices] : allowing the client functionality of sending messages/connecting |
        ///     attaching the client to the services provided by the server<paramref name="m_serverConnection"/>.
        /// </summary>
        private static DuplexChannelFactory<IntServices> m_channelFactory;

        /// <summary>
        /// List[string] : list of the clients connected to the server. Initialized through first successful connection to the server.
        /// </summary>
        List<string> m_connected = new List<string>();

        /// <summary>
        /// SessionStatus : containing variables to maintain the current states / information about the session.
        /// </summary>
        SessionStatus m_sessionStatus;

        /// <summary>
        /// Dictionary[string, DirectMessage] : containing each direct message[DirectMessage (window)] to the recipient[string].
        /// </summary>
        Dictionary<string, DirectMessage> m_chatWindows = new Dictionary<string, DirectMessage>();

        /// <summary>
        /// Constructor for the semi-transparent overlay window:
        ///     (1) Involves starting an instance of the Login window
        ///     (2) Connecting to the server.
        ///     (3) Initializing the session status variables.
        /// </summary>
        public SemiTransOverlay()
        {
            m_channelFactory = new DuplexChannelFactory<IntServices>(new ImpClient(), "clientEP");
            m_serverConnection = m_channelFactory.CreateChannel();
            InitializeComponent();

            // Initialize session variables to the appropriate state
            m_sessionStatus = new SessionStatus();
            m_sessionStatus.isWinFocus = true;
            this.Show();
            openLogin();
        }

        /******************* START : Login Handling *******************/
        /// <summary>
        /// Creates a new login window for the client to attempt to login.
        /// Custom Event is raised to handle clicking the login button to pass back information.
        /// </summary>
        private void openLogin()
        {
            LogIn childForm = new LogIn(this);

            // New custom event, declared as attemptConnection which will obtain a List of strings
            childForm.RaiseCustomEvent += new EventHandler<List<String>>(attemptConnection);
        }

        /// <summary>
        /// Custom event handling the task of when the user requests to login to the server.
        ///     (1) Checks the username
        ///     (2) Connects to the server on success
        /// </summary>
        /// <param name="a_sender">object : the login window itself</param>
        /// <param name="a_userInfo"> List[string] : the user information gained from the login window</param>
        public void attemptConnection(object a_sender, List<String> a_userInfo)
        {
            List<string> userInfo = a_userInfo;
            IndexOfConnectionInfo index = new IndexOfConnectionInfo();
            string name = userInfo[index.username];
            List<string> temp = new List<string>();

            //Determining if the user is able to login with the chosen username
            int loginStatus = m_serverConnection.login(userInfo[index.username], ref temp);
            m_sessionStatus.globalList = temp;
            if (loginStatus == 1)
            {
                m_sessionStatus.currentUser = name;
                openWelcome();
                return;
            }
            else if (loginStatus == 0)
            {
                MessageBox.Show("Duplicate Login, Pre-existing in server.");
                Console.WriteLine("Log in failure. Duplicate in system.");
                openLogin();
                return;
            }
            else
            {
                MessageBox.Show("Error connecting to server. Please try again later.");
                openLogin();
                return;
            }
        }
        /******************* END : Login Handling *******************/

        /******************* START : Global Chat *******************/
        /// <summary>
        /// Creates a new custome event for the Welcome window in order to
        ///     open the global chat automatically on the Welcome window's closure.
        /// </summary>
        private void openWelcome()
        {
            Welcome childWindow = new Welcome(this, m_sessionStatus.currentUser);
            childWindow.RaiseCustomEvent += new EventHandler<bool>(openGlobalChat);
        }

        /// <summary>
        /// Open the first instance of the global chat for the user.
        /// </summary>
        /// <param name="sender"> Unused in this function. </param>
        /// <param name="a_isSuccess">bool : if the window was closed</param>
        private void openGlobalChat(object sender, bool a_isSuccess)
        {
            if (a_isSuccess)
            {
                // set up a new instance of the global chat
                newInstGlobal();

                // connect the user to the server list, and send the update to each client
                // that is already connected.
                m_serverConnection.newClientConnection(m_sessionStatus.currentUser);
                setUpButtons();
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Sets up a new instance of the global chat connected to the parent overlay.
        /// Updates the session information
        /// </summary>
        private void newInstGlobal()
        {
            m_sessionStatus.globalChatbox = new GlobalChat(this, m_sessionStatus.currentUser, m_sessionStatus.globalList);
            m_sessionStatus.isGlobalOpen = true;

        }
        /******************* END : Global Chat *******************/

        /******************* START : Set Up Overlay *******************/
        /// <summary>
        /// Buttons that are bound to certain aspects of the screen will be set up here with the appropriate images
        ///     and functionality behind them.
        /// </summary>
        private void setUpButtons()
        {
            Image btnGlobalChat = new Image();
            Image btnDirectMsg = new Image();
            Image btnExit = new Image();

            // function for appropriately creating buttons
            btnSettings(btnExit, "/Resources/Exit.png", 10, 1);
            btnSettings(btnGlobalChat, "/Resources/InformationOverlay/NewGlobal.png", 10, 2);
            btnSettings(btnDirectMsg, "/Resources/InformationOverlay/NewChat.png", 10, 3);

            // Adding the buttons to the Grid layout on the semi-transparent overlay.
            ImgControls.Children.Add(btnGlobalChat);
            ImgControls.Children.Add(btnDirectMsg);
            ImgControls.Children.Add(btnExit);


            // Create the functionality
            btnGlobalChat.MouseDown += new MouseButtonEventHandler(btnGlobalChat_MouseDown);
           // btnDirectMsg.MouseDown += new MouseButtonEventHandler(btnDirectMsg_MouseDown);
            btnExit.MouseDown += new MouseButtonEventHandler(btnExit_MouseDown);
        }

        /// <summary>
        /// Sets the functionality and design of each "button" | really an image that is clickable.
        /// </summary>
        /// <param name="a_img">Image : the image that is being manipulated in the function</param>
        /// <param name="a_src">string : the file path in the resource folder to the appropriate image</param>
        /// <param name="a_buffer">int : used as the amount of space between the picture and the next</param>
        /// <param name="a_imgIndex">int : used as an index for each image to move to certain positioning on the screen</param>
        private void btnSettings(Image a_img, string a_src, int a_buffer, int a_imgIndex)
        {
            a_img.Stretch = Stretch.Fill;
            a_img.HorizontalAlignment = HorizontalAlignment.Left;
            a_img.VerticalAlignment = VerticalAlignment.Top;
            a_img.Height = 65;
            a_img.Width = 65;
            a_img.Margin = new Thickness(this.Width - (a_imgIndex * a_img.Width + a_buffer), this.Height - (a_img.Height + a_buffer), 0, 0);
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(a_src, UriKind.Relative);
            img.EndInit();
            a_img.Source = img;
        }

        /******************* END : Set Up Overlay *******************/

        /******************* START : Overlay Functionality *******************/
        /// <summary>
        /// toggle application useage, when the client uses the hotkeys it will either change the state of the program to "on" or "off"
        /// </summary>
        public void toggleApplication()
        {
            if (m_sessionStatus.isVisible)
            {
                this.Hide();

                // Windows that are "owned" by the semi-transparent overlay window will become hidden
                foreach (Window owned in this.OwnedWindows)
                {
                    owned.Hide();
                }
                m_sessionStatus.isVisible = false;
            }
            else
            {
                this.Show();

                // Windows that are "owned" by the semi-transparent overlay window will become visible
                foreach (Window owned in this.OwnedWindows)
                {
                    owned.Show();
                }
                m_sessionStatus.isVisible = true;
            }
        }

        /// <summary>
        /// When clicking on the disconnect/exit button use a callback function to the
        ///     server to disconnect the user.
        /// </summary>
        /// <param name="a_sender">Image : the clickable image representing the disconnect button</param>
        /// <param name="e">Unused in this function</param>
        private void btnExit_MouseDown(object a_sender, MouseButtonEventArgs e)
        {
            //Disconnect the user from the application and shut down the application
            disconnect();
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// When clicking on the global chat, it will toggle the chat being minimized / visible to the user,
        ///     if the user had closed the chat it will open the chat entirely.
        /// </summary>
        /// <param name="a_sender">Image : the clickable image representing the global chat button</param>
        /// <param name="e">Unused in this function.</param>
        void btnGlobalChat_MouseDown(object a_sender, MouseEventArgs e)
        {
            // Minimize global chat / toggle
            if (m_sessionStatus.isGlobalOpen == false)
            {
                //Open Global Chat back up
                newInstGlobal();
                return;
            }
            if (m_sessionStatus.globalChatbox.WindowState == WindowState.Minimized)
            {
                m_sessionStatus.globalChatbox.WindowState = WindowState.Normal;
            }
            else
            {
                m_sessionStatus.globalChatbox.WindowState = WindowState.Minimized;
            }
        }

        /// <summary>
        /// Changing the global instance in the session status variable(s)
        /// </summary>
        /// <param name="a_isOpen">bool : containing the value if the Global window is open or not.</param>
        public void globalStatus(bool a_isOpen)
        {
            m_sessionStatus.isGlobalOpen = a_isOpen;
            if (a_isOpen == false)
            {
                m_sessionStatus.globalChatbox = null;
            }
        }

        /******************* END : Overlay Functionality *******************/


        /******************* START : Direct Message Connection *******************/

        /// <summary>
        /// When requesting to send a direct message to a client, the user will open
        ///     a new window with the destination client.
        /// </summary>
        /// <param name="a_dest">string : the username of the destination of the message</param>
        public void openDM(string a_dest)
        {
            DirectMessage childWindow = new DirectMessage(this, a_dest, m_sessionStatus.currentUser);
            m_chatWindows.Add(a_dest, childWindow);
        }

        /// <summary>
        /// When receiving a message from a user, this will check if the chat window already exists in the
        ///     <paramref name="m_chatWindows"/> or if it has to create a new chat window.
        /// </summary>
        /// <param name="a_from">string : username of who the message is from</param>
        /// <param name="a_msg">string : the message that is being received</param>
        public void incomingMessage(string a_from, string a_msg)
        {
            //check if window is already opened.
            foreach (var chat in m_chatWindows)
            {
                if(a_from == chat.Key)
                {
                    //Display the message
                    m_chatWindows[a_from].displayToUsr(a_from + ": " + a_msg);
                    return;
                }
            }
            DirectMessage childWindow = new DirectMessage(this, a_from, m_sessionStatus.currentUser, a_msg);
            m_chatWindows.Add(a_from, childWindow);
        }

        /******************* END: Direct Message Connection *******************/


        /******************* START : Client Requests *******************/

        /// <summary>
        /// Requests the server to send a message to a specified destination username, from
        ///     the current client username.
        /// </summary>
        /// <param name="a_dest">string : the destination of where the message is going.</param>
        /// <param name="a_msg">string : the message of what the user wants to send.</param>
        /// 
        public void sendToDestination(string a_dest, string a_msg)
        {
            m_serverConnection.sendToUser(m_sessionStatus.currentUser, a_dest, a_msg);
        }

        /// <summary>
        /// Adding a new user to the connection list.
        /// </summary>
        /// <param name="a_newUser">string : the username of the connecting user.</param>
        public void addNewConnectionList(string a_newUser)
        {
            if (m_sessionStatus.isGlobalOpen)
            {
                m_sessionStatus.globalList.Add(a_newUser);
                m_sessionStatus.globalChatbox.updateJoin(a_newUser);
                m_sessionStatus.globalChatbox.displayToUsr("      " +a_newUser + " joined the conversation!");
            }

        }

        /// <summary>
        /// Sending the message to all clients connected to the server
        /// </summary>
        /// <param name="a_msg">string : message being sent to the global chat</param>
        public void sendAllConnection(string a_msg)
        {
            m_serverConnection.sendToAll(m_sessionStatus.currentUser, a_msg);
        }

        /// <summary>
        /// Updates the client's user list since someone has disconnected from the chat.
        /// </summary>
        /// <param name="a_disconnected">string : username that has disconnected from the server</param>
        public void removeFromConnected(string a_disconnected)
        {
            //communicate with the global chat window's listbox
            m_sessionStatus.globalList.Remove(a_disconnected);
            m_sessionStatus.globalChatbox.updateDisconnect(a_disconnected);
            m_sessionStatus.globalChatbox.displayToUsr("      " + a_disconnected + " has left the conversation.");
        }

        /************************** END: Client Requests *****************************/


        /************************** START: Server Responses *****************************/
        /// <summary>
        /// Receiving the message from the server to the global chat
        /// </summary>
        /// <param name="a_message">string : the message to display that was sent to the user</param>
        public void messagePass(string a_message)
        {
            if (m_sessionStatus.isGlobalOpen)
            {
                // TODO :: This text should probably have it's own formatting - BLACK
                m_sessionStatus.globalChatbox.displayToUsr(a_message);

            }
        }

        /// <summary>
        /// Requesting a disconnect from the server.
        /// </summary>
        public void disconnect()
        {
            m_serverConnection.disconnectionHandle(m_sessionStatus.currentUser);
        }

        /************************** END: Server Responses *****************************/


        /*
         *Please Note:
         *      The below code is completely functional, however it will not function with
         *      multiple instances of the ChatWindow running since the first instance would have
         *      already used the CTRL+Q hotkey. This will make conflicts with any instance created
         *      after it.
         */

        /************************** START: Hotkey Declaration *****************************/
        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey([In] IntPtr hWnd, [In] int id, [In] uint fsModifiers, [In] uint vk);
        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey([In] IntPtr hWnd, [In] int id);
        private HwndSource _source;
        private const int HOTKEY_ID = 9000;

        /// <summary>
        /// When the application starts up, this function will start up using the application as the
        /// argument "e"
        /// </summary>
        /// <param name="e">This full application</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            RegisterHotKey();
        }

        /// <summary>
        /// When the application closes, ensures that each hotkey bound will be removed.
        /// </summary>
        /// <param name="e">This full application</param>
        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey();
            base.OnClosed(e);
        }

        /// <summary>
        /// Registers the hotkey CTRL+Q through hexadecimal values through Windows messages.
        /// </summary>
        private void RegisterHotKey()
        {
            var helper = new WindowInteropHelper(this);

            //virtual key code "reminders" - these are the codes for Q and CTRL
            const uint VK_QKEY = 0x51;
            const uint MOD_CTRL = 0x0002;
            if (!RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_CTRL, VK_QKEY))
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Unregisters the hotkey ( on the closure of the application ).
        /// </summary>
        private void UnregisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
        }

        /// <summary>
        /// Windows hotkey "break" or notification. This will toggle the application if the hotkey is correct.
        /// </summary>
        /// <param name="hwnd">Windows stuff</param>
        /// <param name="msg">int : looking for the value to be successful["posted"]</param>
        /// <param name="wParam">IntPtr : the full hotkey ID</param>
        /// <param name="lParam">Windows stuff</param>
        /// <param name="handled">bool : to see if the hotkey was successfully read in and handled properly</param>
        /// <returns>IntPtr</returns>
        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // WM_HOTKEY is "posted" when the message is the registered hotkey press.
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            //hot key successfully pressed.
                            toggleApplication();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        /************************** END: Hotkey Declaration *****************************/
    }
}
