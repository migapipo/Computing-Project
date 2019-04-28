using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());


            // Set the function we want to run on each window.
            EnumDelegate delEnumfunc = new EnumDelegate(EnumWindowsProcId);
            // Run it on each window
            bool bSuccessful = EnumDesktopWindows(IntPtr.Zero, delEnumfunc, IntPtr.Zero);


           

            

        }

        // We're just going to print out all the visible windows' process ID and bounds.

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


        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr hWnd);

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
          
            IntPtr handle = GetWindowDC(hWnd);


            // Not sure whu we're getting 0 size windows that are "visible". But we'll ignore them.
            if (bounds.Bottom == 0 && bounds.Top == 0 && bounds.Right == 0 && bounds.Left == 0) return true;

            Console.WriteLine(processid + " (" + bounds.Bottom + "," + bounds.Top + "," + bounds.Left + "," + bounds.Right + ")");

            return true;
        }

    }
}
