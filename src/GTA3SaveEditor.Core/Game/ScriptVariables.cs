using System;
using System.Collections.Generic;
using System.Text;

namespace GTA3SaveEditor.Core.Game
{
    // TODO: accommodate for different SCM versions.
    // v1: PS2/PC/Xbox
    // v2: Android/iOS
    //
    // specific differences still unknown, but at least one variable was added
    // technically, PS2 PALv1 uses a different SCM but the vars don't change (call it v0?)

    public enum ScriptVariable : int
    {
        FLAG_HOOD_MISSION5_PASSED = 363
    }
}
