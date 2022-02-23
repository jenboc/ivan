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
using System.Numerics;

namespace ivan
{ 
    public partial class DrawingForm : Form
    {
        Graphics graphics;
        int x = -1;
        int y = -1;
        bool mouseDown = false;
        PenSettings settings;

        public static Pen pen;
        public static string penShape;

        List<Line> lines = new List<Line>();


        public DrawingForm()
        {
            InitializeComponent();

            graphics = GraphicsPanel.CreateGraphics();
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            pen = new Pen(Color.Black, 3);
            pen.StartCap = pen.EndCap = LineCap.Round;
            penShape = "normal";
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

        private void DrawStraightLine(int sX, int sY, int eX, int eY)
        {
            //Initialise Line
            Line l = new Line(sX, sY, eX, eY, (int)pen.Width, pen.Color);

            //Draw and save
            drawLine(l);
            lines.Add(l);
        }

        private void DrawSquare(MouseEventArgs e)
        {
            DrawStraightLine(x, y, x, e.Y);
            DrawStraightLine(x, e.Y, e.X, e.Y);
            DrawStraightLine(e.X, e.Y, e.X, y);
            DrawStraightLine(e.X, y, x, y);
        }
        
        private double ConvertToRad(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        private void DrawCircle(MouseEventArgs e)
        {
            //Find radius (dX/2)  
            float radius = Math.Abs(x - e.X) / 2;

            //Find (x,y) pair + draw line
            //Using degrees:
            //X coordinate = radius x sin(degrees * pi/180)
            //Y coordinate = radius x cos(degrees * pi/180)

            Vector2 coordinates;
            Vector2 oldCoordinate = new Vector2(-1f, -1f);

            for (int degrees = 0; degrees < 360; degrees++)
            {
                double radians = ConvertToRad(degrees);
                coordinates = new Vector2((float)(radius * Math.Sin(radians)+(e.X-radius)), (float)(radius * Math.Cos(radians)+(e.Y-radius)));

                if (oldCoordinate != new Vector2(-1f, -1f))
                {
                    Line l = new Line(oldCoordinate, coordinates, (int)pen.Width, pen.Color);
                    lines.Add(l);
                    drawLine(l);
                }

                oldCoordinate = coordinates;
            }
        }

        private void graphicsPanel_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            x = e.X;
            y = e.Y;
        }

        private void graphicsPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown && (x != -1 && y != -1) && penShape == "normal")
            {
                Line newLine = new Line(x, y, e.X, e.Y, (int)pen.Width, pen.Color);

                drawLine(newLine);
                lines.Add(newLine);

                x = e.X;
                y = e.Y;
            }
        }

        private void graphicsPanel_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;

            switch (penShape)
            {
                case "line":
                    DrawStraightLine(x, y, e.X, e.Y);
                    break;
                case "square":
                    DrawSquare(e);
                    break;
                case "circle":
                    DrawCircle(e);
                    break;
            }

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

    class Line
    {
        public int sX;
        public int sY;
        public int eX;
        public int eY;

        public int width;
        public Color color;

        public Line() { }
        public Line(int sX, int sY, int eX, int eY, int width, Color color)
        {
            this.sX = sX;
            this.sY = sY;
            this.eX = eX;
            this.eY = eY;
            this.width = width;
            this.color = color;
        }
        public Line(Vector2 startPos, Vector2 endPos, int width, Color color)
        {
            this.sX = (int)startPos.X;
            this.sY = (int)startPos.Y;
            this.eX = (int)endPos.X;
            this.eY = (int)endPos.Y;
            this.width = width;
            this.color = color;
        }
    }
}
