using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;




namespace ConsoleApplication1
{

  // We're just going to print out all the visible windows' process ID and bounds.
  class Program
  {

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


    public struct Size
    {
      int width;
      int height;
    }

    //------------------------test--------------------------

    //==========================================================

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


      int width = Math.Abs(bounds.Right - bounds.Left);
      int height = Math.Abs(bounds.Top - bounds.Bottom);
      int x = bounds.Left;
      int y = bounds.Top;

      int[] rectArray = new int[4];
      rectArray[0] = x;
      rectArray[1] = y;
      rectArray[2] = width;
      rectArray[3] = height;

      //foreach (var i in rectArray)
      //{
      //  Console.Write( i + " ," );
      //}

     

      // Not sure whu we're getting 0 size windows that are "visible". But we'll ignore them.
      if (bounds.Bottom == 0 && bounds.Top == 0 && bounds.Right == 0 && bounds.Left == 0) return true;

          Console.WriteLine("typeOfHandle:" + handle.GetType());
          Console.WriteLine("processid: "+ processid + " (" + bounds.Bottom + "," + bounds.Top + "," + bounds.Left + "," + bounds.Right + ")");
          Console.WriteLine("width: " + width + "\n" + "height: " + height + "\n" + "handle: " + handle);
          Console.WriteLine("x position of upper-left corner: " + bounds.Left);
          Console.WriteLine("y position of upper-left corner: " + bounds.Top);

          Graphics g = Graphics.FromHdc(handle);
          Pen myPen = new Pen(System.Drawing.Color.Yellow, 20);
          Rectangle myRectangle = new Rectangle(0, 0, width, height);
          g.DrawRectangle(myPen, myRectangle);


            return true;

   

        }

        static void Main()
    {
      // Set the function we want to run on each window.
      EnumDelegate delEnumfunc = new EnumDelegate(EnumWindowsProcId);
      // Run it on each window
      bool bSuccessful = EnumDesktopWindows(IntPtr.Zero, delEnumfunc, IntPtr.Zero);

    

      // Pause
      Console.ReadLine();
    }
  }
}
