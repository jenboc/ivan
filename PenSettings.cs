using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ivan
{
    public partial class PenSettings : Form
    {
        PictureBox[] colorBoxes;
        Dictionary<string, LineCap> lineCaps;


        public PenSettings()
        {
            InitializeComponent();

            colorBoxes = new PictureBox[] { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5 };

            lineCaps = new Dictionary<string, LineCap>
            { 
                { "round", LineCap.Round },
                { "square", LineCap.Square },
                { "triangle", LineCap.Triangle }
            };
        }


        private bool checkBoxes(Color color)
        {
            foreach (PictureBox box in colorBoxes)
            {
                if (box.BackColor == color)
                {
                    return true;
                }
            }

            return false;
        }

        private Color swapColor(PictureBox box, Color color)
        {
            Color temp = box.BackColor;
            box.BackColor = color;
            return temp;
        }

        private void shiftBoxes(Color newColor)
        {
            Color oldColor = colorBoxes[0].BackColor;
            for (int i=0; i < colorBoxes.Length; i++)
            { 
                if (i == 0)
                {
                    oldColor = swapColor(colorBoxes[i], newColor);
                }
                else
                {
                    oldColor = swapColor(colorBoxes[i], oldColor);
                }
            }
        }

        private void changePenColor(object sender, EventArgs e)
        {
            PictureBox box = (PictureBox)sender;
            DrawingForm.pen.Color = box.BackColor;
        }

        private void changeShape(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            DrawingForm.penShape = button.Text.ToLower();
        }


        private void changePenThickness(object sender, EventArgs e) //When trackbar is moved
        {
            TrackBar tBar = (TrackBar)sender;
            DrawingForm.pen.Width = tBar.Value;
        }

        private void openColorWidget(object sender, EventArgs e)
        {
            ColorDialog cDialog = new ColorDialog();

            if (cDialog.ShowDialog() == DialogResult.OK)
            {
                if (cDialog.Color != null)
                {
                    DrawingForm.pen.Color = cDialog.Color;
                    shiftBoxes(cDialog.Color);
                }
            }
        }

        private void changeStartCap(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            string key = (string)((PictureBox)sender).Tag;
            
            switch (me.Button)
            {
                case MouseButtons.Left:
                    DrawingForm.pen.StartCap = lineCaps[key];
                    break;
                case MouseButtons.Right:
                    DrawingForm.pen.EndCap = lineCaps[key];
                    break;
            }           
            
        }
    }
}
