using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using GTA3SaveEditor.Core.Util;

namespace GTA3SaveEditor.Core.Loaders
{
    public class IniLoader
    {
        private static readonly Regex SectionRegex = new Regex("^\\[(.*)\\]$");

        public static Dictionary<string, string> LoadIni(string path, string section = "")
        {
            return LoadIni(File.ReadAllBytes(path));
        }

        public static Dictionary<string, string> LoadIni(byte[] data, string section = "")
        {
            Dictionary<string, string> ini = new Dictionary<string, string>();

            using MemoryStream m = new MemoryStream(data);
            using StreamReader buf = new StreamReader(m);

            int lineNum = 0;
            string line, key, value;
            bool sectionFound = false;

            while ((line = buf.ReadLine()) != null)
            {
                line = line.Trim();
                lineNum++;

                if (string.IsNullOrEmpty(line) ||
                    line.StartsWith(";") ||
                    line.StartsWith("#")) continue;

                // Check if line is a section header.
                Match match = SectionRegex.Match(line);

                if (!string.IsNullOrEmpty(section))
                {
                    if (match.Success)
                    {
                        if (sectionFound)
                        {
                            // We've already found the section we're looking for and have now
                            // reached a new section. Time to quit.
                            break;
                        }
                        if (match.Groups[1].Value.Equals(section))
                        {
                            // We've just found the section we're looking for!
                            sectionFound = true;
                            continue;
                        }
                    }

                    // Do nothing until we find our section.
                    if (!sectionFound)
                    {
                        continue;
                    }
                }

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
