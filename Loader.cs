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
        
        // A - Start X | B - Start Y
        // X - End X | Y - End Y
        // W - Width | C - Color
        // N - New Line
        private static char[] identifiers = { 'n', 'a', 'b', 'x', 'y', 'w', 'c'};


        private static string lineToString(Line line, int[] lastStart, int[] lastEnd, int? lastWidth, int? lastColor, ref char lastIdentifier)
        {
            string lineProperties = "";

            if (line.sX != lastStart[0] && line.sX != lastEnd[0])
            {
                lineProperties += "a" + line.sX;
                lastIdentifier = 'a';
            }

            if (line.sY != lastStart[1] && line.sY != lastEnd[1])
            {
                lineProperties += "b" + line.sY;
                lastIdentifier = 'b';
            }

            if (line.eX != lastEnd[0])
            {
                lineProperties += "x" + line.eX;
                lastIdentifier = 'x';
            }

            if (line.eY != lastEnd[1])
            {
                lineProperties += "y" + line.eY;
                lastIdentifier = 'y';
            }

            if (line.width != lastWidth)
            {
                lineProperties += "w" + line.width;
                lastIdentifier = 'w';
            }

            if (line.color.ToArgb() != lastColor)
            {
                lineProperties += "c" + line.color.ToArgb();
                lastIdentifier = 'c';
            }

            return lineProperties;
        }


        public static void Save(List<Line> lines, StreamWriter stream)
        {
            int lastWidth = 3;
            int lastColor = System.Drawing.Color.Black.ToArgb();
            int[] lastStartCoords = { -1, -1 };
            int[] lastEndCoords = { -1, -1 };
            char lastIdentifier = 'c';

            for (int i = 0; i < lines.Count; i++)
            {
                int prevIdIndex = Array.IndexOf(identifiers, lastIdentifier);
                string sLine = lineToString(lines[i], lastStartCoords, lastEndCoords, lastWidth, lastColor, ref lastIdentifier);

                if (prevIdIndex < Array.IndexOf(identifiers, lastIdentifier))
                {
                    sLine = "n" + sLine;
                }


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

                stream.Write(sLine);
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

            int lastCharIndex = 0;
            for (int i = 0; i < sResult.Length; i++)
            {
                char character = sResult[i];

                if (isValidId(character))
                {
                    if (i != 0 && sResult[lastCharIndex] != 'n') //If it's not the first identifier => add data to current line
                    {
                        string currentData = sResult.Substring(lastCharIndex, i-lastCharIndex); //Data between previous ID and current ID e.g. a126b => 126 (where a is prev and b is current)
                        currentLine.Add(currentData);
                    }

                    int lastIdentifierIndex = Array.IndexOf(identifiers, sResult[lastCharIndex]);
                    int currentIdIndex = Array.IndexOf(identifiers, character);

                    if ((currentIdIndex <= lastIdentifierIndex || character == 'n') && i != 0) 
                    {
                        dataValues.Add(currentLine);
                        currentLine = new List<string>();
                    }

                    lastCharIndex = i;
                }
            }

            currentLine.Add(sResult.Substring(lastCharIndex));
            dataValues.Add(currentLine);

            return dataValues;
        }


        public static List<Line> Load(StreamReader stream)
        {
            List<Line> lines = new List<Line>();

            string sResult = stream.ReadToEnd();

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
