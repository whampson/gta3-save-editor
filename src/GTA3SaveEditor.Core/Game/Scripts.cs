using System;
using System.ComponentModel;
using GTASaveData.GTA3;
using WpfEssentials;

namespace GTA3SaveEditor.Core.Game
{
    public static class Scripts
    {
        public static string GenerateThreadName()
        {
            Random r = new Random();

            string id = "id_";
            for (int i = 0; i < 4; i++)
            {
                id += r.Next(10).ToString();
            }

            return id;
        }
    }

    public enum AndOrState
    {
        [Description("(none)")]
        None,
        [Description("And (1)")]
        And1 = 1,
        [Description("And (2)")]
        And2,
        [Description("And (3)")]
        And3,
        [Description("And (4)")]
        And4,
        [Description("And (5)")]
        And5,
        [Description("And (6)")]
        And6,
        [Description("And (7)")]
        And7,
        [Description("And (8)")]
        And8,
        [Description("Or (1)")]
        Or1 = 21,
        [Description("Or (2)")]
        Or2,
        [Description("Or (3)")]
        Or3,
        [Description("Or (4)")]
        Or4,
        [Description("Or (5)")]
        Or5,
        [Description("Or (6)")]
        Or6,
        [Description("Or (8)")]
        Or7,
        [Description("Or (8)")]
        Or8,
    }
}
