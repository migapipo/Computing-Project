using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

namespace WindowsFormsApplication1
{
    public class WindowGraphics : IDisposable
    {
        private IntPtr hrgn, hdc, hWnd;
        private Graphics graphics;
        private Rect rect;
        private static readonly bool is9xMe;

        /// <summary>
        /// Static constructor that checks the windows version
        /// </summary>
        static WindowGraphics()
        {
            uint ver = GetVersion();
            if (ver < 0x80000000) // What I am doing?!! See the Windows SDK Documentation
                is9xMe = false;
            else
                is9xMe = true;
        }

        /// <summary>
        /// Creates a new instance of the WindowGraphics class.
        /// </summary>
        /// <param name="wnd">The window to create the Graphics for.</param>
        public WindowGraphics(IWin32Window wnd)
        {
            hWnd = wnd.Handle;
        }

        /// <summary>
        /// Creates the graphics. This functin is automatically called when
        /// you use the Graphics property for the first time.
        /// </summary>
        public void CreateGraphics()
        {
            if (graphics != null)
                DestroyGraphics();

            hdc = GetDC(IntPtr.Zero); // get DC for the entire screen

            hrgn = GetVisibleRgn(hWnd); // obtain visible clipping region for the window
            SelectClipRgn(hdc, hrgn); // clip the DC with the region

            if (is9xMe)
                OffsetClipRgn(hdc, rect.left, rect.top);

            rect = new Rect();
            GetWindowRect(hWnd, rect);

            // move the origin from upper left corner of the screen,
            // to the upper left corner of the form
            SetWindowOrgEx(hdc, -rect.left, -rect.top, IntPtr.Zero);

            graphics = Graphics.FromHdc(hdc);
        }

        public void DestroyGraphics()
        {
            if (graphics != null)
            {
                graphics.Dispose();
                graphics = null;
            }

            if (hdc != IntPtr.Zero)
            {
                ReleaseDC(IntPtr.Zero, hdc);
                hdc = IntPtr.Zero;
            }
            if (hrgn != IntPtr.Zero)
            {
                DeleteObject(hrgn);
                hrgn = IntPtr.Zero;
            }
        }

        public Graphics Graphics
        {
            get
            {
                if (graphics == null)
                    CreateGraphics();
                return graphics;
            }
        }

        /// <summary>
        /// Gets the visible clipping region for the window.
        /// </summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <returns>Handle to the clipping region.</returns>
        private IntPtr GetVisibleRgn(IntPtr hWnd)
        {
            IntPtr hrgn, hdc;
            hrgn = CreateRectRgn(0, 0, 0, 0);
            hdc = GetWindowDC(hWnd);
            int res = GetRandomRgn(hdc, hrgn, 4); // the value of SYSRGN is 4. Refer to Windows SDK Documentation.
            ReleaseDC(hWnd, hdc);
            return hrgn;
        }

        [StructLayout(LayoutKind.Sequential)]
        class Rect
        {
            public int left, top, right, bottom;
        }

        [DllImport("user32")]
        private static extern int GetWindowRect(IntPtr hwnd, [Out]Rect lpRect);
        [DllImport("user32")]
        private static extern IntPtr GetWindowDC(IntPtr hwnd);
        [DllImport("user32")]
        private static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("user32")]
        private static extern IntPtr ReleaseDC(IntPtr hwnd, IntPtr hdc);
        [DllImport("gdi32")]
        private static extern int GetRandomRgn(IntPtr hdc, IntPtr hrgn, int iNum);
        [DllImport("gdi32")]
        private static extern IntPtr CreateRectRgn(int X1, int Y1, int X2, int Y2);
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr hObject);
        [DllImport("gdi32")]
        private static extern int SelectClipRgn(IntPtr hdc, IntPtr hRgn);
        [DllImport("kernel32")]
        private static extern uint GetVersion();
        [DllImport("gdi32")]
        private static extern int OffsetClipRgn(IntPtr hdc, int x, int y);
        [DllImport("gdi32")]
        public static extern int SetWindowOrgEx(IntPtr hdc, int nX, int nY, IntPtr lpPoint);

        public void Dispose()
        {
            DestroyGraphics();
        }
    }
}