using System;
using System.Collections.Generic;
using System.IO;
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
    struct Line
    {
        public int sX;
        public int sY;
        public int eX;
        public int eY;

        public int width;
        public Color color;
    }

    public partial class DrawingForm : Form
    {
        Graphics graphics;
        int x = -1;
        int y = -1;
        bool mouseDown = false;
        PenSettings settings;

        public static Pen pen;

        List<Line> lines = new List<Line>();


        public DrawingForm()
        {
            InitializeComponent();

            graphics = GraphicsPanel.CreateGraphics();
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            pen = new Pen(Color.Black, 3);
            pen.StartCap = pen.EndCap = LineCap.Round;
        }

        private void drawLine(Line line)
        {
            Color oldColor = pen.Color;
            float oldWidth = pen.Width;

            pen.Color = line.color;
            pen.Width = line.width;

            graphics.DrawLine(pen, new Point(line.sX, line.sY), new Point(line.eX, line.eY));

            pen.Color = oldColor;
            pen.Width = oldWidth;
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
            settings = new PenSettings();
            settings.Show();
        }

        private void saveAsMenuOption(object sender, EventArgs e)
        {
            Stream stream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "Ivan Graphics File (*.ivan) | *.ivan";
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

            openFileDialog.Filter = "Ivan Graphics File (*.ivan) | *.ivan";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                stream = openFileDialog.OpenFile();

                if (stream != null)
                {
                    lines = Loader.Load(stream);

                    graphics.Clear(Color.White);
                    foreach (Line line in lines)
                    {
                        drawLine(line);
                    }
                }
            }

        }

        private void clearMenuOption(object sender, EventArgs e)
        {
            graphics.Clear(Color.White);
            lines = new List<Line>();
        }


        private void closeProgram(bool closeSelf=true)
        {
            if (settings != null)
            {
                settings.Close();
            }

            if (closeSelf) Close();
        }


        private void closeMenuOption(object sender, EventArgs e)
        {
            closeProgram();
        }

        private void DrawingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            closeProgram(false);
        }

        private void DrawingForm_SizeChanged(object sender, EventArgs e)
        {
            MenuStrip.Width = Width;
            GraphicsPanel.Size = Size;

            graphics = GraphicsPanel.CreateGraphics();
        }
    }
}
