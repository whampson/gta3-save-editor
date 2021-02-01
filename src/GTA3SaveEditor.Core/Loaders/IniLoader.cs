using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using GTA3SaveEditor.Core.Util;

namespace GTA3SaveEditor.Core.Loaders
{
    public class IniLoader
    {
        public static Dictionary<string, string> LoadIni(string path)
        {
            return LoadIni(File.ReadAllBytes(path));
        }

        public static Dictionary<string, string> LoadIni(byte[] data)
        {
            Dictionary<string, string> ini = new Dictionary<string, string>();

            using MemoryStream m = new MemoryStream(data);
            using StreamReader buf = new StreamReader(m);

            int lineNum = 0;
            string line, key, value;
            while ((line = buf.ReadLine()) != null)
            {
                line = line.Trim();
                lineNum++;

                if (string.IsNullOrEmpty(line) ||
                    line.StartsWith(";") ||
                    line.StartsWith("#")) continue;

                // TODO: sections

                if (!line.Contains("="))
                {
                    Log.Warn($"IniLoader: Line {lineNum}: Invalid key-value pair '{line}'");
                }

                string[] split = line.Split(new char[] { '=' }, 2);
                key = split[0].Trim();
                value = split[1].Trim();
                ini.Add(key, value);
            }

            return ini;
        }
    }
}
