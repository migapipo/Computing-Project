using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
            //this.WindowState = FormWindowState.Maximized;
        }

        public void uxDraw_Click(object sender,EventArgs e)
        {


            using (WindowGraphics wg = new WindowGraphics(this))
            {
                wg.Graphics.DrawLine(Pens.Blue, 0, 0, 100, 100);
                //.....

                // or if you have to call many drawing functions, here is the way to reduce
                // your typing. This is what I always do...
                Graphics g = wg.Graphics;
                g.DrawString("I am on the title bar!", new Font("Tahoma", 10, FontStyle.Bold), Brushes.Black, 0, 4);
                //g.FillEllipse(Brushes.Black, this.Width - 40, this.Height - 40, 80, 80);
               // g.DrawRectangle(new Pen(Color.Yellow, 10), this.DisplayRectangle);
                // .... other drawing commands...
            }


            //Draw a filled rectangle inside the form 
            System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
            System.Drawing.Graphics formGraphics = this.CreateGraphics();
            formGraphics.FillRectangle(myBrush, new Rectangle(50, 50, 200, 300));
            myBrush.Dispose();
            formGraphics.Dispose();

            //Draw rectangle inside the form
            System.Drawing.Graphics graphicsObj;
            graphicsObj = this.CreateGraphics();
            Pen myPen = new Pen(System.Drawing.Color.Red, 5);
            Rectangle myRectangle = new Rectangle(20, 20, 250, 200);
            graphicsObj.DrawRectangle(myPen, myRectangle);

            //Draw form border
            graphicsObj.DrawRectangle(new Pen(Color.Green, 10), this.DisplayRectangle);
        }

     

    }
}
