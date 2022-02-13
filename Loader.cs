using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ivan
{
    class Loader
    {
        private static string lineToString(Line line, int[] lastStart, int[] lastEnd, int? lastWidth, int? lastColor)
        {
            string[] lineProperties = new string[6];
            string saveString = "";

            if (line.sX != lastStart[0])
            {
                lineProperties[0] = "a" + line.sX;
            }

            if (line.sY != lastStart[1])
            {
                lineProperties[1] = "b" + line.sY;
            }

            if (line.eX != lastEnd[0])
            {
                lineProperties[2] = "x" + line.eX;
            }

            if (line.eY != lastEnd[1])
            {
                lineProperties[3] = "y" + line.eY;
            }

            if (line.width != lastWidth)
            {
                lineProperties[4] = "w" + line.width;
            }

            if (line.color.ToArgb() != lastColor)
            {
                lineProperties[5] = "c" + line.color.ToArgb();
            }


            bool firstNonNull = true;
            foreach (string property in lineProperties)
            {
                if (property != null)
                {
                    if (!firstNonNull)
                    {
                        saveString += ",";
                    }

                    firstNonNull = false;

                    saveString += property;
                }
            }


            return saveString;
        }


        public static void Save(List<Line> lines, Stream stream)
        {
            int lastWidth = 3;
            int lastColor = System.Drawing.Color.Black.ToArgb();
            int[] lastStartCoords = { -1, -1 };
            int[] lastEndCoords = { -1, -1 };

            for (int i = 0; i < lines.Count; i++)
            {
                string sLine = lineToString(lines[i], lastStartCoords, lastEndCoords, lastWidth, lastColor);

                lastWidth = lines[i].width;
                lastColor = lines[i].color.ToArgb();
                lastStartCoords[0] = lines[i].sX;
                lastStartCoords[1] = lines[i].sY;
                lastEndCoords[0] = lines[i].eX;
                lastEndCoords[1] = lines[i].eY;

                if (i != (lines.Count-1))
                {
                    sLine += "\n";
                }

                byte[] WriteData = Encoding.UTF8.GetBytes(sLine);
                stream.Write(WriteData, 0, WriteData.Length);
            }

            stream.Close();
        }

        public static List<Line> Load(Stream stream)
        {
            List<Line> lines = new List<Line>();

            byte[] result = new byte[stream.Length];
            stream.Read(result, 0, Convert.ToInt32(stream.Length));

            string sResult = Encoding.UTF8.GetString(result);
            string[] sLines = sResult.Split('\n');

            System.Drawing.Color currentColor = System.Drawing.Color.Black;
            int currentWidth = 3;
            int currentSX = -1;
            int currentSY = -1;
            int currentEX = -1;
            int currentEY = -1;

            foreach (string sLine in sLines)
            {
                Line line = new Line();

                string[] dataValues = sLine.Split(',');


                for (int i = 0; i < dataValues.Length; i++)
                {
                    char identifier = dataValues[i][0];
                    int data = Convert.ToInt32(dataValues[i].Substring(1));

                    switch (identifier)
                    {
                        //START COORDINATES
                        case 'a':
                            currentSX = data;
                            break;
                        case 'b':
                            currentSY = data;
                            break;
                        //END COORDINATES
                        case 'x':
                            currentEX = data;
                            break;
                        case 'y':
                            currentEY = data;
                            break;
                        //PROPERTIES
                        case 'c':
                            currentColor = System.Drawing.Color.FromArgb(data);
                            break;
                        case 'w':
                            currentWidth = data;
                            break;
                    }
                }

                line.sX = currentSX;
                line.sY = currentSY;

                line.eX = currentEX;
                line.eY = currentEY;

                line.color = currentColor;
                line.width = currentWidth;

                lines.Add(line);
            }

            stream.Close();
            return lines;
        }
    }
}
