using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GTA3SaveEditor.Core.Util;
using GTASaveData;

namespace GTA3SaveEditor.Core
{
    public static class GxtLoader
    {
        public static Dictionary<string, string> Load(string path)
        {
            return Load(File.ReadAllBytes(path));
        }

        public static Dictionary<string, string> Load(byte[] data)
        {
            const int SectionHeaderSize = 8;
            const int SectionIdLength = 4;
            const int KeyRecordSize = 12;
            const int KeyLength = 8;

            using DataBuffer buf = new DataBuffer(data);
            Dictionary<string, string> gxtTable = new Dictionary<string, string>();

            string tkey = buf.ReadString(SectionIdLength);
            int tkeySize = buf.ReadInt32();
            int tkeyStart = buf.Mark();
            if (tkey != "TKEY") throw InvalidGxtFile();

            Debug.Assert(tkeyStart < data.Length);
            Debug.Assert(tkeySize < data.Length);

            buf.Skip(tkeySize);

            string tdat = buf.ReadString(SectionIdLength);
            int tdatSize = buf.ReadInt32();
            int tdatStart = buf.Mark();
            if (tdat != "TDAT") throw InvalidGxtFile();

            Debug.Assert(tdatStart < data.Length);
            Debug.Assert(tdatSize < data.Length);

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

                gxtTable.Add(key, value);
            }

            Log.Info($"Loaded {gxtTable.Count} GXT entries.");
            return gxtTable;
        }

        private static InvalidDataException InvalidGxtFile()
        {
            return new InvalidDataException("Not a valid GTA3 GXT file!");
        }
    }
}
