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
    public partial class PenSettings : Form
    {
        public PenSettings()
        {
            InitializeComponent();
        }

        private void changePenColor(object sender, EventArgs e)
        {
            PictureBox box = (PictureBox)sender;
            DrawingForm.pen.Color = box.BackColor;
        }


        private void changePenThickness(object sender, EventArgs e) //When trackbar is moved
        {
            TrackBar tBar = (TrackBar)sender;
            DrawingForm.pen.Width = tBar.Value;
        }
    }
}
