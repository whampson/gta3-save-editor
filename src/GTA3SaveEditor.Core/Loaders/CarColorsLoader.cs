using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using GTA3SaveEditor.Core.Util;
using GTASaveData.GTA3;

namespace GTA3SaveEditor.Core.Loaders
{
    public static class CarColorsLoader
    {
        public static List<CarColor> LoadColors(string path)
        {
            return LoadColors(File.ReadAllBytes(path));
        }

        public static List<CarColor> LoadColors(byte[] data)
        {
            using MemoryStream m = new MemoryStream(data);
            using StreamReader buf = new StreamReader(m);
            List<CarColor> carcols = new List<CarColor>();

            string line;
            while (buf.ReadLine().Trim() != "col") { }

            while ((line = buf.ReadLine().Trim()) != "end")
            {
                if (string.IsNullOrEmpty(line)) continue;

                string[] colorComment = line.Split('#');
                string[] rgb = colorComment[0].Split(',');
                int.TryParse(rgb[0], out int r);
                int.TryParse(rgb[1], out int g);
                int.TryParse(rgb[2], out int b);
                Color color = Color.FromArgb(r, g, b);
                string name = "";

                if (colorComment.Length > 1)
                {
                    string[] indexNameClass = colorComment[1].Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] indexName = indexNameClass[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    name = indexName.Skip(1).Aggregate((x, y) => x + " " + y);
                    name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
                }

                carcols.Add(new CarColor(color, name));
            }

            Log.Info($"Loaded {carcols.Count} car colors.");
            return carcols;
        }
    }
}
