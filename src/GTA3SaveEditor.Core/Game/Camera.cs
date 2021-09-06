using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GTA3SaveEditor.Core.Game
{
    public enum CamZoom
    {
        [Description("First Person")]
        FirstPerson,

        [Description("Near")]
        Near,

        [Description("Middle")]
        Middle,

        [Description("Far")]
        Far,

        [Description("Top-Down")]
        TopDown,

        [Description("Cinematic")]
        Cinematic
    }
}
