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
        private static string lineToString(Line line)
        {
            string str = $"{line.sX},{line.sY},{line.eX},{line.eY},{line.width},{line.color.ToArgb()}";
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

                string[] dataValues = sLine.Split(',');

                line.sX = Convert.ToInt32(dataValues[0]);
                line.sY = Convert.ToInt32(dataValues[1]);
                line.eX = Convert.ToInt32(dataValues[2]);
                line.eY = Convert.ToInt32(dataValues[3]);
                line.width = Convert.ToInt32(dataValues[4]);
                line.color = System.Drawing.Color.FromArgb(Convert.ToInt32(dataValues[5]));

                lines.Add(line);
            }

            return lines;
        }
    }
}
