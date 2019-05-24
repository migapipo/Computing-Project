using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace ColorBorder
{



    // We're just going to print out all the visible windows' process ID and bounds.
    class Program
    {
        //************************20190518 NEW **************************************
        //Retrieves a handle to the foreground window (the window with which the user is currently working). 
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();


        //Retrieves a handle to the window that contains the specified point.
        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(System.Drawing.Point p);

        //Copies the text of the specified window's title bar (if it has one) into a buffer.
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr GetDesktopWindow();


        [DllImport("user32.dll")]
        static extern IntPtr GetDCEx(IntPtr hwnd, IntPtr hrgn, uint flags);


        //************************20190518 NEW END**************************************






        //new : Changes the size, position, and Z order of a child, pop-up, or top-level window. 
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y,
            int cX, int cY, uint uFlags);

        //new : Draws a border around the specified rectangle by using the specified brush. (The width and height of the border are always one logical unit.)
        [DllImport("user32.dll")]
        static extern uint FrameRect(IntPtr hWnd, ref RECT lprc, IntPtr hbr);

        // Used to enumerate over all windows.
        private delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        // This lets us get all the windows on the current desktop.
        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnumDesktopWindows(IntPtr hDesktop,
        EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

        // Get the PID for the window
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        // Get the bounds for the window 
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        // Window visible check
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        //new : creates a logical brush that has the specified solid color.
        [DllImport("gdi32.dll", EntryPoint = "CreateSolidBrush", SetLastError = true)]
        static extern IntPtr CreateSolidBrush(uint crColor);

        //new : retrieves a handle to a device context (DC) for the client area of a specified window or for the entire screen.
        [DllImport("user32.dll", EntryPoint = "GetDC")]
        static extern IntPtr GetDC(IntPtr hWnd);

        //new : retrieves the device context (DC) for the entire window, including title bar, menus, and scroll bars. 
        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hdc);

        [DllImport("user32.dll")]
        static extern IntPtr FromHdc(IntPtr hWnd);


        // HBRUSH hBRUSH = CreateHatchBrush(2, 1);
        //DeleteObject(hBrush);


        // To access the bounds. (There may be a less awkward way to do this...
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }



        /**
         * This is the function to apply to each window handle.
         */
        private static bool EnumWindowsProcId(IntPtr hWnd, int lParam)
        {
            // Ignore invisible windows
            if (!IsWindowVisible(hWnd)) return true;

            // Get the process ID for the current window
            uint processid = 0;
            GetWindowThreadProcessId(hWnd, out processid);

            //Add check for domain here...
            // Get the bounds for the window.
            RECT bounds;
            GetWindowRect(hWnd, out bounds);

            // Draw border here...
            // GetWindowDC retrieves the device context(DC) for the ENTIRE window.
            IntPtr handle = GetWindowDC(hWnd);

            int width = Math.Abs(bounds.Right - bounds.Left);
            int height = Math.Abs(bounds.Top - bounds.Bottom);
            int x = bounds.Left;
            int y = bounds.Top;


            // Not sure whu we're getting 0 size windows that are "visible". But we'll ignore them.
            if (bounds.Bottom == 0 && bounds.Top == 0 && bounds.Right == 0 && bounds.Left == 0) return true;


            string name = String.Empty;
            Process proc = System.Diagnostics.Process.GetProcessById((int)processid);
            if (proc != null)
            {
                name = proc.ProcessName;
            }
            //Console.WriteLine("Process Name is : " + name);

          
            Graphics g = Graphics.FromHdc(handle);

            if (handle != null)
            {
                Pen p = new Pen(System.Drawing.Color.Transparent, 20);


                using (StreamReader r = new StreamReader("file.json"))
                {
                    string json = r.ReadToEnd();
           
                    Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            
                    p.Color = Color.Red;

                    foreach (KeyValuePair<string, string> item in dict)
                    {
                        if (name == item.Key)
                        {
                           p.Color = Color.FromName(dict[name]);
                          //Console.WriteLine("COLOR IS " + dict[name]);
                        }
                    }

                    Rectangle myRectangle = new Rectangle(0, 0, width, height);
                    g.DrawRectangle(p, myRectangle);

                  
                }

                //g.ReleaseHdc();
                g.Dispose();    // Could possibly cause NullException --- CHECK IF I AM THE REASON TO CAUSE ISSUE
                p.Dispose();
                ReleaseDC(hWnd, handle);
            }

            return true;

        }





        public static void getClickWindowName(IntPtr hwnd)
        {

            IntPtr hdcDesktop = GetDCEx(GetDesktopWindow(), IntPtr.Zero, 1027);
            Graphics g1 = Graphics.FromHdc(hdcDesktop);

            Pen p2 = new Pen(System.Drawing.Color.Black, 70);
            Rectangle Rec2 = new Rectangle(0, 0, 2000, 400);

            // get of handle of current working window
            IntPtr handleOfWindow = GetForegroundWindow();
            // get title of the window
            const int count = 512; // Set length limit of the window title 
            var text = new StringBuilder(count);

            //Define String format -- center 
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            if (GetWindowText(handleOfWindow, text, count) > 0)
            {
                //MessageBox.Show(text.ToString());
                //Console.WriteLine("!!" + text.ToString());
                g1.DrawString("CURRENT ACTIVE WINDOW: " + text.ToString(), new Font("Tahoma", 15), Brushes.Black, Rec2, sf);
               
            }
           
            g1.Dispose();


        }

        public static void drawTitleBar() {


            IntPtr hdcDesktop = GetDCEx(GetDesktopWindow(), IntPtr.Zero, 1027);
            Graphics g = Graphics.FromHdc(hdcDesktop);
            

            Pen p1 = new Pen(System.Drawing.Color.Orange, 70);


            Rectangle Rec1 = new Rectangle(0, 0, 2000, 10);


            //g.DrawString("HELLO", new Font("Tahoma", 10), Brushes.Black, Rec1);
            g.DrawRectangle(p1, Rec1);
           
            //g.FillEllipse(Brushes.Red, 0, 0, 400, 400);
            p1.Dispose();
            g.Dispose();
        }
         

        public static void Main()
        {

            // Set the function we want to run on each window.
            EnumDelegate delEnumfunc = new EnumDelegate(EnumWindowsProcId);
            
            while (true)
            {
                
                // Run it on each window
                bool bSuccessful = EnumDesktopWindows(IntPtr.Zero, delEnumfunc, IntPtr.Zero);

                drawTitleBar(); 
                //Next step: Customise the titleBar to make the color change dynamically as the border color 

                getClickWindowName(IntPtr.Zero);

            }
         

            // Pause
            Console.ReadLine();

        }

    }
}
