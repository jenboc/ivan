using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace ivan
{
    class Loader
    {
        private static char[] identifiers = { 'a', 'b', 'x', 'y', 'w', 'c' };


        private static string lineToString(Line line, int[] lastStart, int[] lastEnd, int? lastWidth, int? lastColor)
        {
            string lineProperties = "";

            if (line.sX != lastStart[0] && line.sX != lastEnd[0])
            {
                lineProperties += "a" + line.sX;
            }

            if (line.sY != lastStart[1] && line.sY != lastEnd[1])
            {
                lineProperties += "b" + line.sY;
            }

            if (line.eX != lastEnd[0])
            {
                lineProperties += "x" + line.eX;
            }

            if (line.eY != lastEnd[1])
            {
                lineProperties += "y" + line.eY;
            }

            if (line.width != lastWidth)
            {
                lineProperties += "w" + line.width;
            }

            if (line.color.ToArgb() != lastColor)
            {
                lineProperties += "c" + line.color.ToArgb();
            }

            return lineProperties;
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


        private static bool isValidId(char identifier)
        {
            foreach (char id in identifiers)
            {
                if (id == identifier)
                {
                    return true;
                }
            }

            return false;
        }



        private static List<List<string>> getData(string sResult)
        {
            List<List<string>> dataValues = new List<List<string>>();
            List<string> currentLine = new List<string>();

            int lastIdIndex = 0;
            for (int i = 0; i < sResult.Length; i++)
            {
                char character = sResult[i];

                if (isValidId(character))
                {
                    if (i != 0) //If it's not the first identifier => add data to current line
                    {
                        string currentData = sResult.Substring(lastIdIndex, i-lastIdIndex);
                        currentLine.Add(currentData);
                    }

                    int lastIdentifierIndex = Array.IndexOf(identifiers, sResult[lastIdIndex]);
                    int currentIdIndex = Array.IndexOf(identifiers, character);

                    if (currentIdIndex <= lastIdentifierIndex && i != 0)
                    {
                        dataValues.Add(currentLine);
                        currentLine = new List<string>();
                    }

                    lastIdIndex = i;
                }
            }

            currentLine.Add(sResult.Substring(lastIdIndex));

            return dataValues;
        }


        public static List<Line> Load(Stream stream)
        {
            List<Line> lines = new List<Line>();

            byte[] result = new byte[stream.Length];
            stream.Read(result, 0, Convert.ToInt32(stream.Length));

            string sResult = Encoding.UTF8.GetString(result);

            List<List<string>> parsedData = getData(sResult);

            System.Drawing.Color currentColor = System.Drawing.Color.Black;
            int currentWidth = 3;
            int currentSX = -1;
            int currentSY = -1;
            int currentEX = -1;
            int currentEY = -1;

            foreach (List<string> lineData in parsedData)
            {
                Line line = new Line();

                for (int i = 0; i < lineData.Count; i++)
                {
                    char identifier = lineData[i][0];
                    int data = Convert.ToInt32(lineData[i].Substring(1));

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

                currentSX = currentEX;
                currentSY = currentEY;

                lines.Add(line);
            }

            stream.Close();
            return lines;
        }
    }
}
