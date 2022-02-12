using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace jenboc_paint
{
    struct Line
    {
        public int sX;
        public int sY;
        public int eX;
        public int eY;

        public int width;
        public Color color;
    }

    public partial class jenboc_paint : Form
    {
        Graphics graphics;
        int x = -1;
        int y = -1;
        bool mouseDown = false;
        pen_settings settings;

        public static Pen pen;

        List<Line> lines = new List<Line>();


        public jenboc_paint()
        {
            InitializeComponent();

            graphics = graphicsPanel.CreateGraphics();
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            pen = new Pen(Color.Black, 3);
            pen.StartCap = pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
        }

        private void drawLine(Line line)
        {
            Pen linePen = new Pen(line.color, line.width);
            linePen.StartCap = linePen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            graphics.DrawLine(linePen, new Point(line.sX, line.sY), new Point(line.eX, line.eY));
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
                Line newLine = new Line();
                newLine.sX = x;
                newLine.sY = y;
                newLine.eX = e.X;
                newLine.eY = e.Y;
                newLine.width = Convert.ToInt32(pen.Width);
                newLine.color = pen.Color;

                drawLine(newLine);
                lines.Add(newLine);

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

        private void penSettingsMenuOption(object sender, EventArgs e)
        {
            settings = new pen_settings();
            settings.Show();
        }

        private void saveAsMenuOption(object sender, EventArgs e)
        {
            Stream stream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "jenboc paint files (*.jpnt) | *.jpnt";
            saveFileDialog.FilterIndex = 1;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                stream = saveFileDialog.OpenFile();

                if (stream != null)
                {
                    Loader.Save(lines, stream);
                    MessageBox.Show("Saved Successfully");
                }
            }
        }

        private void loadMenuOption(object sender, EventArgs e)
        {
            Stream stream;
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "jenboc paint files (*.jpnt) | *.jpnt";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                stream = openFileDialog.OpenFile();

                if (stream != null) {
                    lines = Loader.Load(stream);

                    graphics.Clear(Color.White);
                    foreach (Line line in lines)
                    {
                        drawLine(line);
                    }
                }
            }

        }


        private void closeMenuOption(object sender, EventArgs e)
        {
            if (settings != null)
            {
                settings.Close();
            }

            Close();
        }
    }
}
