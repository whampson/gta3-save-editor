using System.Collections.Generic;
using System.IO;
using GTA3SaveEditor.Core.Util;

namespace GTA3SaveEditor.Core
{
    public static class IdeLoader
    {
        public static List<IdeObject> LoadObjects(string path)
        {
            return LoadObjects(File.ReadAllBytes(path));
        }

        public static List<IdeObject> LoadObjects(byte[] data)
        {
            using MemoryStream m = new MemoryStream(data);
            using StreamReader buf = new StreamReader(m);

            string line;
            while (buf.ReadLine().Trim() != "objs") { }

            List<IdeObject> objects = new List<IdeObject>();
            while ((line = buf.ReadLine().Trim()) != "end")
            {
                if (string.IsNullOrEmpty(line)) continue;

                string[] objInfo = line.Split(',');
                short.TryParse(objInfo[0], out short id);
                string model = objInfo[1].Trim();

                objects.Add(new IdeObject(id, model));
            }

            Log.Info($"Loaded {objects.Count} IDE objects.");
            return objects;
        }
    }
}
