using System;
using System.Collections.Generic;
using System.Linq;
using GTASaveData;
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

        public static List<CustomScript> EnumerateCustomScripts(this SaveFileGTA3 save)
        {
            var threads = save.Scripts.Threads;
            var scriptSpace = save.Scripts.ScriptSpace.ToArray();
            int scriptSpaceSize = save.Scripts.ScriptSpace.Count;

            List<CustomScript> customScripts = new List<CustomScript>();
            foreach (var t in threads)
            {
                int ip = t.InstructionPointer;
                if (ip > 0 && ip < scriptSpaceSize)
                {
                    customScripts.Add(new CustomScript(t));
                }
            }

            int end = scriptSpaceSize;
            foreach (var sc in customScripts.OrderByDescending(sc => sc.Thread.InstructionPointer))
            {
                int entry = sc.GetEntryPoint();
                int len = end - entry;
                sc.Code = new byte[len];
                Array.Copy(scriptSpace, entry, sc.Code, 0, len);
                end = entry;
            }

            return customScripts;
        }

        public static bool AddCustomScript(this SaveFileGTA3 save, string name, int entryPoint, byte[] code)
        {
            return AddCustomScript(save, new CustomScript(entryPoint, name, code));
        }

        public static bool AddCustomScript(this SaveFileGTA3 save, CustomScript script)
        {
            var threads = save.Scripts.Threads;
            var scriptSpace = save.Scripts.ScriptSpace.ToArray();
            int scriptSpaceSize = save.Scripts.ScriptSpace.Count;

            if (script.GetEntryPoint() < 0 ||
                script.Code == null ||
                script.Thread == null)
            {
                return false;
            }

            int len = script.Code.Length;
            int beg = script.GetEntryPoint();
            int end = DataBuffer.Align4(beg + len);

            byte[] newScriptSpace = (end > scriptSpaceSize)
                ? new byte[end]
                : new byte[scriptSpaceSize];

            if (beg > scriptSpaceSize)
            {
                Array.Copy(scriptSpace, 0, newScriptSpace, 0, scriptSpaceSize);
            }
            else
            {
                Array.Copy(scriptSpace, 0, newScriptSpace, 0, beg);
                Array.Copy(scriptSpace, beg, newScriptSpace, end, newScriptSpace.Length - beg);
            }
            Array.Copy(script.Code, 0, newScriptSpace, beg, len);

            save.Scripts.ScriptSpace = newScriptSpace;
            threads.Add(script.Thread);
            return true;
        }

        public static bool DeleteCustomScript(this SaveFileGTA3 save, CustomScript script)
        {
            var threads = save.Scripts.Threads;
            var scriptSpace = save.Scripts.ScriptSpace.ToArray();
            int scriptSpaceSize = save.Scripts.ScriptSpace.Count;

            if (!threads.Contains(script.Thread))
            {
                return false;
            }

            threads.Remove(script.Thread);

            // TODO: bugcheck for bound errors
            int len = script.Code.Length;
            int beg = script.GetEntryPoint();
            int end = beg + len;

            byte[] newScriptSpace = new byte[scriptSpaceSize - len];
            Array.Copy(scriptSpace, beg, newScriptSpace, 0, len);
            Array.Copy(scriptSpace, end, newScriptSpace, beg, newScriptSpace.Length - beg);

            save.Scripts.ScriptSpace = newScriptSpace;
            return true;
        }
    }
}
