using System.Collections.Generic;
using System.IO;
using GTA3SaveEditor.Core.Game;
using GTA3SaveEditor.Core.Util;

namespace GTA3SaveEditor.Core.Loaders
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
            int lineNum = 0;
            while (buf.ReadLine().Trim() != "objs") lineNum++;

            List<IdeObject> objects = new List<IdeObject>();
            while ((line = buf.ReadLine().Trim()) != "end")
            {
                lineNum++;
                if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;

                string[] objInfo = line.Split(',');
                if (objInfo.Length < 2)
                {
                    Log.Warn($"IdeLoader: Line {lineNum}: Malformed 'objs' entry");
                    continue;
                }
                short.TryParse(objInfo[0], out short id);
                string model = objInfo[1].Trim();

                objects.Add(new IdeObject(id, model));
            }

            Log.Info($"IdeLoader: Loaded {objects.Count} objects.");
            return objects;
        }

        public static List<VehicleModel> LoadCars(string path)
        {
            return LoadCars(File.ReadAllBytes(path));
        }

        public static List<VehicleModel> LoadCars(byte[] data)
        {
            using MemoryStream m = new MemoryStream(data);
            using StreamReader buf = new StreamReader(m);

            string line;
            int lineNum = 0;
            while (buf.ReadLine().Trim() != "cars") lineNum++;

            List<VehicleModel> cars = new List<VehicleModel>();
            while ((line = buf.ReadLine().Trim()) != "end")
            {
                lineNum++;
                if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;

                string[] objInfo = line.Split(',');
                if (objInfo.Length < 6)
                {
                    Log.Warn($"IdeLoader: Line {lineNum}: Malformed 'cars' entry");
                    continue;
                }
                short.TryParse(objInfo[0], out short id);
                string model = objInfo[1].Trim();
                string gameName = objInfo[5].Trim();

                cars.Add(new VehicleModel(id, model, gameName));
            }

            Log.Info($"IdeLoader: Loaded {cars.Count} cars.");
            return cars;
        }
    }
}
