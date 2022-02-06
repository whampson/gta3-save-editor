using System;
using GTA3SaveEditor.Core.Game;
using GTASaveData;
using GTASaveData.GTA3;

namespace GTA3SaveEditor.Core.Extensions
{
    public static class GTA3SaveExtensions
    {
        private const char GxtKeyMarker = '\uFFFF';
        private const int GxtKeyLength = 7;         // TODO: confirm, may be 8 if null-terminator isn't required

        public static bool IsTitleGxtKey(this GTA3Save save)
        {
            const int GxtKeyStart = 1;

            return save.Title.Length > GxtKeyStart
                && save.Title.Length < GxtKeyStart + GxtKeyLength + 1
                && save.Title[0] == GxtKeyMarker;
        }

        public static string GetTitleRaw(this GTA3Save save)
        {
            if (save.IsTitleGxtKey())
            {
                return save.Title.Substring(1);
            }

            return save.Title;
        }

        /// <summary>
        /// "Smart" method for getting a save's internal name. Handles the
        /// GXT string case which is possible on iOS/Android saves.
        /// </summary>
        public static string GetTitle(this GTA3Save save)
        {
            if (save.Title == null)
            {
                return null;
            }

            if (save.IsTitleGxtKey())
            {
                string gxtKey = save.Title.Substring(1);
                return GTA3.GetGxtString(gxtKey);
            }

            return save.Title;
        }

        /// <summary>
        /// "Smart" method for setting a save's name. Handles the GXT String
        /// case which is possible on iOS/Android saves.
        /// </summary>
        public static void SetTitle(this GTA3Save save, string name, bool isGxtKey)
        {
            int len = Math.Min(name.Length, GxtKeyLength);
            save.SimpleVars.LastMissionPassedName = (isGxtKey) ? (GxtKeyMarker + name.Substring(0, len)) : name;
        }

        public static FileType GetFileType(this GTA3Save save)
        {
            return save.Params.FileType;
        }

        public static bool HasPurpleNinesGlitch(this GTA3Save save)
        {
            return save.GetScriptVar(ScriptVariable.FLAG_HOOD_MISSION5_PASSED) == 0
                && save.Gangs[GangType.Hoods].PedModelOverride != -1;
        }

        public static bool FixPurpleNinesGlitch(this GTA3Save save)
        {
            if (save.HasPurpleNinesGlitch())
            {
                save.Gangs[GangType.Hoods].PedModelOverride = -1;
                return true;
            }

            return false;
        }
    }

    public enum ScmVersion
    {
        Unknown = -1,
        SCMv0,
        SCMv1,
        SCMv2,
    }
}
