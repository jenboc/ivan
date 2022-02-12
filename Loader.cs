using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jenboc_paint
{
    class Loader
    {
        private static byte[] toBytes(int value)
        {
            byte[] bytes;

            bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);

            return bytes;
        }

        private static string lineToString(Line line)
        {
            string str = "";
            str += "sX=" + line.sX + "\n";
            str += "sY=" + line.sY + "\n";
            str += "eX=" + line.eX + "\n";
            str += "eY=" + line.eY + "\n";
            str += "width=" + line.width + "\n";
            str += "color=" + line.color.ToArgb();
            return str;
        }


        public static void Save(List<Line> lines, Stream stream)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                string sLine = lineToString(lines[i]);

                if (i != (lines.Count-1))
                {
                    sLine += "\n$\n";
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
            string[] sLines = sResult.Split('$');            

            foreach (string sLine in sLines)
            {
                Line line = new Line();

                string[] sLineData = sLine.Split('\n');

                foreach (string sData in sLineData)
                {
                    string[] values = sData.Split('=');

                    switch (values[0])
                    {
                        case "sX":
                            line.sX = Convert.ToInt32(values[1]);
                            break;
                        case "sY":
                            line.sY = Convert.ToInt32(values[1]);
                            break;
                        case "eX":
                            line.eX = Convert.ToInt32(values[1]);
                            break;
                        case "eY":
                            line.eY = Convert.ToInt32(values[1]);
                            break;
                        case "width":
                            line.width = Convert.ToInt32(values[1]);
                            break;
                        case "color":
                            line.color = System.Drawing.Color.FromArgb(Convert.ToInt32(values[1]));
                            break;
                    }
                }

                lines.Add(line);
            }

            return lines;
        }
    }
}
