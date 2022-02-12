using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace jenboc_paint
{
    public partial class jenboc_paint : Form
    {
        Graphics graphics;
        int x = -1;
        int y = -1;
        bool mouseDown = false;
        Pen pen;

        public jenboc_paint()
        {
            InitializeComponent();
            
            graphics = graphicsPanel.CreateGraphics();
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            pen = new Pen(Color.Black, 5);
            pen.StartCap = pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            PictureBox pBox = (PictureBox)sender;
        }

        private void graphicsPanel_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            x = e.X;
            y = e.Y;
        }

        private void graphicsPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown && (x != -1 && y != -1))
            {
                graphics.DrawLine(pen, new Point(x, y), e.Location);
                x = e.X;
                y = e.Y;
            }
        }

        private void graphicsPanel_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            x = -1;
            y = -1;
        }
    }
}
