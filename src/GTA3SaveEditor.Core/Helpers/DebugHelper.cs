using System;
using System.Diagnostics;

namespace GTA3SaveEditor.Core.Helpers
{
    public static class DebugHelper
    {
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void Throw(Exception e) => throw e;
    }
}
