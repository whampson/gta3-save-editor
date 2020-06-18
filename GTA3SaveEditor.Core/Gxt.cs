using GTASaveData;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace GTA3SaveEditor.Core
{
    public class Gxt
    {
        private Dictionary<string, string> m_gxtTable;

        public string this[string key]
        {
            get { return m_gxtTable[key]; }
        }

        public Dictionary<string,string>.KeyCollection Keys
        {
            get { return m_gxtTable.Keys; }
        }

        public Dictionary<string, string>.ValueCollection Values
        {
            get { return m_gxtTable.Values; }
        }

        public IReadOnlyDictionary<string, string> Items
        {
            get { return new ReadOnlyDictionary<string, string>(m_gxtTable); }
        }

        public Gxt()
        {
            m_gxtTable = new Dictionary<string, string>();
        }

        public bool ContainsKey(string key)
        {
            return m_gxtTable.ContainsKey(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return m_gxtTable.TryGetValue(key, out value);
        }

        public static Gxt LoadFromFile(string path)
        {
            const int SectionHeaderSize = 8;
            const int SectionIdLength = 4;
            const int KeyRecordSize = 12;
            const int KeyLength = 8;

            Gxt gxt = new Gxt();

            byte[] gxtFile = File.ReadAllBytes(path);
            int gxtSize = gxtFile.Length;

            using (StreamBuffer buf = new StreamBuffer(gxtFile))
            {
                string tkey = buf.ReadString(SectionIdLength);
                int tkeySize = buf.ReadInt32();
                int tkeyStart = buf.Mark();
                if (tkey != "TKEY") throw InvalidGxtFile();

                Debug.Assert(tkeyStart < gxtSize);
                Debug.Assert(tkeySize < gxtSize);

                buf.Skip(tkeySize);

                string tdat = buf.ReadString(SectionIdLength);
                int tdatSize = buf.ReadInt32();
                int tdatStart = buf.Mark();
                if (tdat != "TDAT") throw InvalidGxtFile();

                Debug.Assert(tdatStart < gxtSize);
                Debug.Assert(tdatSize < gxtSize);

                buf.Seek(tkeyStart);

                int tkeyPos = 0;
                while (tkeyPos < tkeySize)
                {
                    buf.Seek(SectionHeaderSize + tkeyPos);
                    int valueOffset = buf.ReadInt32();
                    string key = buf.ReadString(KeyLength);
                    tkeyPos += KeyRecordSize;

                    Debug.Assert(valueOffset < tdatSize);

                    buf.Seek(tdatStart + valueOffset);
                    string value = buf.ReadString(unicode: true);

                    gxt.m_gxtTable.Add(key, value);
                }
            }

            return gxt;
        }

        private static InvalidDataException InvalidGxtFile()
        {
            return new InvalidDataException("Invalid GTA3 GXT file!");
        }
    }
}
