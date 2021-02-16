using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GTA3SaveEditor.Core.Game
{
    public static class GTA3
    {
        public static IEnumerable<CarColor> CarColors { get; set; }
        public static IEnumerable<IdeObject> IdeObjects { get; set; }
        public static IReadOnlyDictionary<string, string> GxtTable { get; set; }
        public static IEnumerable<VehicleModel> Vehicles { get; set; }

        static GTA3()
        {
            CarColors = new List<CarColor>();
            IdeObjects = new List<IdeObject>();
            GxtTable = new Dictionary<string, string>();
            Vehicles = new List<VehicleModel>();
        }

        public static IdeObject GetIdeObject(int id)
        {
            return IdeObjects.Where(o => o.Id == (short) id).FirstOrDefault();
        }

        public static IdeObject GetIdeObject(string name)
        {
            return IdeObjects.Where(o => o.ModelName == name).FirstOrDefault();
        }

        public static string GetGxtString(string key)
        {
            if (GetGxtString(key, out string value))
            {
                return value;
            }
            else
            {
                return $"{key} missing";
            }
        }

        public static bool GetGxtString(string key, out string value)
        {
            return GxtTable.TryGetValue(key, out value);
        }

        public static VehicleModel GetVehicle(int id)
        {
            return Vehicles.Where(x => x.Id == (short) id).FirstOrDefault();
        }
    }

    public enum GangPedModelState
    {
        [Description("Both Models")]
        Both,

        [Description("Model 1 Only")]
        Model1,

        [Description("Model 2 Only")]
        Model2
    }
}
