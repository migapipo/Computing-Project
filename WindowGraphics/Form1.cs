using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace UnicornSoftwares
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void uxDraw_Click( object sender, EventArgs e )
		{
			using ( WindowGraphics wg = new WindowGraphics( this ) )
			{
				//.....
				wg.Graphics.DrawLine( Pens.Blue, 0, 0, 100, 100 );
				//.....

				// or if you have to call many drawing functions, here is the way to reduce
				// your typing. This is what I always do...
				Graphics g = wg.Graphics;
				g.DrawString( "I am on the title bar!", new Font( "Tahoma", 10, FontStyle.Bold ), Brushes.Black, 0, 4 );
				g.FillEllipse( Brushes.Black, this.Width - 40, this.Height - 40, 80, 80 );
				// .... other drawing commands...
			}
		}
	}
}
