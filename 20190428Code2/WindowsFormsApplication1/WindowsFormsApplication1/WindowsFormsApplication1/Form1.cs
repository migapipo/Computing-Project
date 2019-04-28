using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {



        public Form1()
        {
            InitializeComponent();
            this.TransparencyKey = (BackColor);
            this.WindowState = FormWindowState.Maximized;
        }

        
        public void uxDraw_Click(object sender, EventArgs e)
        {


            //using (WindowGraphics wg = new WindowGraphics(this))
            //{
            //    wg.Graphics.DrawLine(Pens.Blue, 0, 0, 100, 100);
            //    //.....

            //    // or if you have to call many drawing functions, here is the way to reduce
            //    // your typing. This is what I always do...
            //    Graphics g = wg.Graphics;
            //    g.DrawString("I am on the title bar!", new Font("Tahoma", 10, FontStyle.Bold), Brushes.Black, 0, 4);
            //    //g.FillEllipse(Brushes.Black, this.Width - 40, this.Height - 40, 80, 80);
            //   // g.DrawRectangle(new Pen(Color.Yellow, 10), this.DisplayRectangle);
            //    // .... other drawing commands...
            //}


            //Draw a filled rectangle inside the form 
            //System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
            //System.Drawing.Graphics formGraphics = this.CreateGraphics();
            //formGraphics.FillRectangle(myBrush, new Rectangle(50, 50, 200, 300));
            //myBrush.Dispose();
            //formGraphics.Dispose();


            //int xPosition = Program.B;
            //int yPosition = Rect.Top;
            //int Width = Math.Abs(Rect.Left - Rect.Right);
            //int Height = Math.Abs(Rect.Top - Rect.Bottom);


            //Draw rectangle inside the Form
            System.Drawing.Graphics graphicsObj;
            graphicsObj = this.CreateGraphics();
            Pen myPen = new Pen(System.Drawing.Color.Red, 10);
            Rectangle myRectangle = new Rectangle(0, 15, 250, 200);
            Rectangle myRectangle1 = new Rectangle(1000, 150, 600, 800);
            graphicsObj.DrawRectangle(myPen, myRectangle);
            graphicsObj.DrawRectangle(myPen, myRectangle1);
            //DrawString((bounds.Left).ToString, new Font("Tahoma", 10, FontStyle.Bold), Brushes.Black, 0, 4);
            graphicsObj.DrawString("Hello World", new Font("Tahoma", 30, FontStyle.Bold), Brushes.Blue, 5, 5);

            //Draw form border (Form = Canvas)
            graphicsObj.DrawRectangle(new Pen(Color.Yellow, 20), this.DisplayRectangle);

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







            // Draw border here...   (CANNOT SHOW? WHY ?)

            IntPtr handle = GetWindowDC(hWnd);
            Graphics g = Graphics.FromHwnd(handle);
            Pen myPen = new Pen(System.Drawing.Color.Blue, 10);
            Rectangle myRectangle = new Rectangle(bounds.Left, bounds.Top, 200, 200);
            g.DrawRectangle(myPen, myRectangle);
           









            // Not sure whu we're getting 0 size windows that are "visible". But we'll ignore them.
            if (bounds.Bottom == 0 && bounds.Top == 0 && bounds.Right == 0 && bounds.Left == 0) return true;

            Console.WriteLine(processid + " (" + bounds.Bottom + "," + bounds.Top + "," + bounds.Left + "," + bounds.Right + ")");

            return true;
        }




    }

}

