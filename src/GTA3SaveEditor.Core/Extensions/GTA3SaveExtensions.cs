using GTASaveData.GTA3;

namespace GTA3SaveEditor.Core.Extensions
{
    public static class GTA3SaveExtensions
    {
        public static string GetSaveName(this GTA3Save save)
        {
            if (save.Name == null)
            {
                return null;
            }

            if (save.Name.StartsWith("\uFFFF"))
            {
                string gxtKey = save.Name.Substring(1);
                if (SaveEditor.GxtTable.TryGetValue(gxtKey, out string value))
                {
                    return value;
                }
                else
                {
                    return $"(invalid GXT key: {gxtKey})";
                }
            }

            return save.Name;
        }

        public static void SetSaveName(this GTA3Save save, string name, bool isGxtKey)
        {
            save.Name = (isGxtKey) ? ('\uFFFF' + name) : name;
        }
    }
}
