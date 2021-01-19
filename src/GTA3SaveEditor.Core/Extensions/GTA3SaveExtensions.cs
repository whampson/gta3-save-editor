using GTASaveData.GTA3;

namespace GTA3SaveEditor.Core.Extensions
{
    public static class GTA3SaveExtensions
    {
        public static string GetSaveName(this SaveFileGTA3 save)
        {
            if (save.Name == null)
            {
                return null;
            }

            if (save.Name.Length > 0 && save.Name[0] == '\uFFFF')
            {
                string gxtKey = save.Name.Substring(1);
                return GTA3.GetGxtString(gxtKey);
            }

            return save.Name;
        }

        public static void SetSaveName(this SaveFileGTA3 save, string name, bool isGxtKey)
        {
            save.Name = (isGxtKey) ? ('\uFFFF' + name) : name;
        }
    }
}
