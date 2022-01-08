using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GTA3SaveEditor.Core.Game;
using GTA3SaveEditor.Core.Util;
using GTASaveData.GTA3;

namespace GTA3SaveEditor.Core.Extensions
{
    public static class Script
    {
        public static int GetScriptVar(this GTA3Save save, ScriptVariable var)
        {
            // TODO: conversion function for ScriptVariable -> index

            return save.Script.GetGlobalVariable((int) var);
        }

        public static float GetScriptVarFloat(this GTA3Save save, ScriptVariable var)
        {
            // TODO: conversion function for ScriptVariable -> index

            return save.Script.GetGlobalVariableFloat((int) var);
        }

        public static void SetScriptVar(this GTA3Save save, ScriptVariable var, int value)
        {
            // TODO: conversion function for ScriptVariable -> index

            save.Script.SetGlobalVariable((int) var, value);
        }

        public static void SetScriptVar(this GTA3Save save, ScriptVariable var, float value)
        {
            // TODO: conversion function for ScriptVariable -> index

            save.Script.SetGlobalVariable((int) var, value);
        }

        public static void InsertScriptVar(this GTA3Save save, ScriptVariable var, int value = 0)
        {
            int offset = (int) var * 4;
            for (int i = 0; i < 4; i++)
            {
                save.Script.ScriptSpace.Insert(offset + i, 0);
            }

            save.SetScriptVar(var, value);
        }

        public static void RemoveScriptVar(this GTA3Save save, ScriptVariable var)
        {
            int offset = (int) var * 4;
            for (int i = 0; i < 4; i++)
            {
                save.Script.ScriptSpace.RemoveAt(offset + i);
            }
        }

        public static ScmVersion GetScriptVersion(this GTA3Save save)
        {
            int v0 = ScmVersionInfo[ScmVersion.SCMv0].MainSize;
            int v1 = ScmVersionInfo[ScmVersion.SCMv1].MainSize;
            int v2 = ScmVersionInfo[ScmVersion.SCMv2].MainSize;

            int mainSize = save.Script.MainScriptSize;
            if (mainSize == v0) return ScmVersion.SCMv0;
            if (mainSize == v1) return ScmVersion.SCMv1;
            if (mainSize == v2) return ScmVersion.SCMv2;

            return ScmVersion.Unknown;
        }

        public static bool SetScriptVersion(this GTA3Save save, ScmVersion version)
        {
            if (version <= ScmVersion.Unknown || version > ScmVersion.SCMv2)
            {
                return false;
            }

            ScmVersion oldVersion = save.GetScriptVersion();
            //if (oldVersion > version)
            //{
            //    Debug.Assert(false, "Cannot convert backwards to older version yet!!");
            //    return false;
            //}

            int from = (int) oldVersion;
            int to = (int) version;

            if (from < to)
            {
                do
                {
                    ConvertScriptsUpOne(save, (ScmVersion) (++from));
                }
                while (from < to);
            }
            else
            {
                do
                {
                    ConvertScriptsDownOne(save, (ScmVersion) (--from));
                }
                while (from > to);
            }

            return true;
        }

        private static void ConvertScriptsUpOne(GTA3Save save, ScmVersion toVersion)
        {
            ScmInfo info = ScmVersionInfo[toVersion];
            List<(int, int)> diffs = null;

            if (toVersion == ScmVersion.SCMv1) diffs = DiffsV0;
            if (toVersion == ScmVersion.SCMv2) diffs = DiffsV1;

            foreach (var script in save.Script.RunningScripts)
            {
                int oldIp = script.IP;
                script.IP = ConvertScriptAddress(diffs, oldIp, false);

                for (int i = 0; i < script.StackIndex; i++)
                {
                    script.Stack[i] = ConvertScriptAddress(diffs, script.Stack[i], false);
                }
            }

            save.Script.MainScriptSize = info.MainSize;
            save.Script.LargestMissionScriptSize = info.LargestMission;

            if (toVersion == ScmVersion.SCMv2)
            {
                save.InsertScriptVar((ScriptVariable) 245);
                save.InsertScriptVar((ScriptVariable) 246);
                save.InsertScriptVar((ScriptVariable) 247);
                save.InsertScriptVar((ScriptVariable) 248);
                save.RemoveScriptVar((ScriptVariable) 1867);
                save.RemoveScriptVar((ScriptVariable) 1870);
                save.RemoveScriptVar((ScriptVariable) 1877);
                save.RemoveScriptVar((ScriptVariable) 1880);
                save.InsertScriptVar((ScriptVariable) 2637);
                save.InsertScriptVar((ScriptVariable) 3441);
                save.InsertScriptVar((ScriptVariable) 3442);
                save.InsertScriptVar((ScriptVariable) 3443);
                save.InsertScriptVar((ScriptVariable) 3444);
                save.InsertScriptVar((ScriptVariable) 3445);
                save.InsertScriptVar((ScriptVariable) 3446);
            }

            Debug.Assert(save.Script.GlobalVariables.Count() == info.NumGlobals,
                $"NumGlobals Incorrect! (expected = {info.NumGlobals}, actual = {save.Script.GlobalVariables.Count()})");
        }

        private static void ConvertScriptsDownOne(GTA3Save save, ScmVersion toVersion)
        {
            ScmInfo info = ScmVersionInfo[toVersion];
            List<(int, int)> diffs = null;

            ScmVersion fromVersion = save.GetScriptVersion();
            if (toVersion == ScmVersion.SCMv0) diffs = DiffsV0;
            if (toVersion == ScmVersion.SCMv1) diffs = DiffsV1;

            foreach (var script in save.Script.RunningScripts)
            {
                int oldIp = script.IP;
                script.IP = ConvertScriptAddress(diffs, oldIp, true);

                for (int i = 0; i < script.StackIndex; i++)
                {
                    script.Stack[i] = ConvertScriptAddress(diffs, script.Stack[i], true);
                }
            }

            save.Script.MainScriptSize = info.MainSize;
            save.Script.LargestMissionScriptSize = info.LargestMission;

            int globalCount = save.Script.GlobalVariables.Count();
            if (fromVersion == ScmVersion.SCMv2)
            {
                save.RemoveScriptVar((ScriptVariable) 3446);
                save.RemoveScriptVar((ScriptVariable) 3445);
                save.RemoveScriptVar((ScriptVariable) 3444);
                save.RemoveScriptVar((ScriptVariable) 3443);
                save.RemoveScriptVar((ScriptVariable) 3442);
                save.RemoveScriptVar((ScriptVariable) 3441);
                save.RemoveScriptVar((ScriptVariable) 2637);
                save.InsertScriptVar((ScriptVariable) 1880);
                save.InsertScriptVar((ScriptVariable) 1877);
                save.InsertScriptVar((ScriptVariable) 1870);
                save.InsertScriptVar((ScriptVariable) 1867);
                save.RemoveScriptVar((ScriptVariable) 248);
                save.RemoveScriptVar((ScriptVariable) 247);
                save.RemoveScriptVar((ScriptVariable) 246);
                save.RemoveScriptVar((ScriptVariable) 245);
            }

            Debug.Assert(save.Script.GlobalVariables.Count() == info.NumGlobals,
                $"NumGlobals Incorrect! (expected = {info.NumGlobals}, actual = {save.Script.GlobalVariables.Count()})");
        }

        private static int ConvertScriptAddress(List<(int, int)> diffs, int addr, bool backwards)
        {
            for (int i = 1; i < diffs.Count; i++)
            {
                var bo1 = diffs[i - 1];
                var bo2 = diffs[i];

                int b1;
                int o1;
                int b2;
                int o2;

                if (!backwards)
                {
                    b1 = bo1.Item1;
                    o1 = bo1.Item2;
                    b2 = bo2.Item1;
                    o2 = bo2.Item2;
                }
                else
                {
                    b1 = bo1.Item1 + bo1.Item2;
                    o1 = -bo1.Item2;
                    b2 = bo2.Item1 + bo2.Item2;
                    o2 = -bo2.Item2;
                }

                int delta = o2 - o1;
                bool removal = delta < 0;

                if (!removal && addr == b1)
                {
                    // address points to a location where code was added, always return pointer to same code after added code
                    return addr + o1 + delta;
                }

                if (removal && addr >= b1 && addr < b1 + Math.Abs(delta))
                {
                    // address points to removed code, always return pointer to code immediately following removed code
                    return b1 + o1;
                }

                if (addr < b1)
                {
                    return addr + o1;
                }

                if (addr > b1 && addr < b2)
                {
                    // normal case, just apply the current offset
                    return addr + o2;
                }
            }

            // address out-of-range
            return -1;
        }

        private class ScmInfo
        {
            public int NumGlobals;
            public int MainSize;
            public int LargestMission;
            //public List<(int, int)> DiffTable;      // changes made to make next version
        }

        private static Dictionary<ScmVersion, ScmInfo> ScmVersionInfo = new Dictionary<ScmVersion, ScmInfo>()
        {
            { ScmVersion.SCMv0, new ScmInfo {
                NumGlobals = 4313,
                MainSize = 108762,
                LargestMission = 25285,
                //DiffTable = DiffsV0,
            } },
            { ScmVersion.SCMv1, new ScmInfo {
                NumGlobals = 4313,
                MainSize = 108797,
                LargestMission = 25299,
                //DiffTable = DiffsV1
            } },
            { ScmVersion.SCMv2, new ScmInfo {
                NumGlobals = 4320,
                MainSize = 108853,
                LargestMission = 25299,
                //DiffTable = null
            } },
        };

        private static List<(int, int)> DiffsV0 = new List<(int, int)>()
        {
            // Changes to v0 to make v1
            (52689, 0),
            (52966, 7),
            (53243, 14),
            (53520, 21),
            (53797, 28),
            (118193, 35),
            (118348, 63),
            (118431, 68),
            (118464, 86),
            (118948, 98),
            (119033, 103),
            (119240, 110),
            (119177, 174),
            (119177, 179),
            (119274, 184),
            (119616, 191),
            (120768, 209),
            (120768, 214),
            (120768, 219),
            (120768, 224),
            (120768, 229),
            (120768, 234),
            (120768, 239),
            (120768, 244),
            (120768, 249),
            (121050, 254),
            (127288, 320),
            (127413, 333),
            (127538, 346),
            (127663, 359),
            (127788, 372),
            (127913, 385),
            (128038, 398),
            (128163, 411),
            (128288, 424),
            (128413, 437),
            (128538, 450),
            (128663, 463),
            (128788, 476),
            (128913, 489),
            (129038, 502),
            (130966, 515),
            (131091, 528),
            (131216, 541),
            (131341, 554),
            (131466, 567),
            (131591, 580),
            (131716, 593),
            (131841, 606),
            (131966, 619),
            (132091, 632),
            (132216, 645),
            (132341, 658),
            (134594, 671),
            (134719, 684),
            (134844, 697),
            (134969, 710),
            (135094, 723),
            (135219, 736),
            (135344, 749),
            (135469, 762),
            (135594, 775),
            (135719, 788),
            (135844, 801),
            (135969, 814),
            (136094, 827),
            (136219, 840),
            (136344, 853),
            (136469, 866),
            (136594, 879),
            (136719, 892),
            (136844, 905),
            (136969, 918),
            (139201, 931),
            (139326, 944),
            (139451, 957),
            (139576, 970),
            (139701, 983),
            (139826, 996),
            (139951, 1009),
            (140076, 1022),
            (140076, 1035),
            (140193, 1043),
            (140318, 1056),
            (140443, 1069),
            (140568, 1082),
            (140693, 1095),
            (140818, 1108),
            (140943, 1121),
            (141068, 1134),
            (141193, 1147),
            (141318, 1160),
            (141443, 1173),
            (141568, 1186),
            (147790, 1199),
            (147854, 1202),
            (154082, 1205),
            (154146, 1208),
            (155039, 1211),
            (266694, 1223),
            // TODO: mission offsets?
        };

        private static List<(int, int)> DiffsV1 = new List<(int, int)>()
        {
            // Changes to v1 to make v2
            (980, 0),
            (7468, 16),
            (10548, 0),
            (13764, 4),
            (43104, 28),
            (43200, 46),
            (64734, 53),
            (86641, 168),
            (86655, 154),
            (86713, 140),
            (86727, 126),
            (86785, 112),
            (86799, 98),
            (86843, 84),
            (86857, 70),
            (203973, 56),
            // TODO: mission offsets?
            //(274674, 90),
            //(274695, 83),
            //(275788, 76),
            //(276256, 10),
            //(276698, -180),
            //(367918, -185),
            //(399606, -183),
            //(410381, -175),
        };
    }
}
