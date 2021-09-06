using System;
using System.Collections.Generic;
using System.Diagnostics;
using GTA3SaveEditor.Core.Game;
using GTA3SaveEditor.Core.Util;
using GTASaveData.GTA3;

namespace GTA3SaveEditor.Core.Extensions
{
    public static class GTA3SaveExtensions
    {
        private const char GxtKeyMarker = '\uFFFF';
        private const int GxtKeyLength = 7;         // TODO: confirm, may be 8 if null-terminator isn't required

        public static bool IsNameGxtKey(this SaveFileGTA3 save)
        {
            const int GxtKeyStart = 1;

            return save.Name.Length > GxtKeyStart
                && save.Name.Length < GxtKeyStart + GxtKeyLength + 1
                && save.Name[0] == GxtKeyMarker;
        }

        public static string GetNameRaw(this SaveFileGTA3 save)
        {
            if (save.IsNameGxtKey())
            {
                return save.Name.Substring(1);
            }

            return save.Name;
        }

        /// <summary>
        /// "Smart" method for getting a save's internal name. Handles the
        /// GXT string case which is possible on iOS/Android saves.
        /// </summary>
        public static string GetName(this SaveFileGTA3 save)
        {
            if (save.Name == null)
            {
                return null;
            }

            if (save.IsNameGxtKey())
            {
                string gxtKey = save.Name.Substring(1);
                return GTA3.GetGxtString(gxtKey);
            }

            return save.Name;
        }

        /// <summary>
        /// "Smart" method for setting a save's name. Handles the GXT String
        /// case which is possible on iOS/Android saves.
        /// </summary>
        public static void SetName(this SaveFileGTA3 save, string name, bool isGxtKey)
        {
            int len = Math.Min(name.Length, GxtKeyLength);
            save.SimpleVars.LastMissionPassedName = (isGxtKey) ? (GxtKeyMarker + name.Substring(0, len)) : name;
        }

        public static bool HasPurpleNinesGlitch(this SaveFileGTA3 save)
        {
            return save.GetScriptVar(ScriptVariable.FLAG_HOOD_MISSION5_PASSED) == 0
                && save.Gangs[GangType.Hoods].PedModelOverride != -1;
        }

        public static bool FixPurpleNinesGlitch(this SaveFileGTA3 save)
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
