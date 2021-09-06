using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace GTA3SaveEditor.Core.Util
{
    public static class DebugHelper
    {
        /// <summary>
        /// Re-throws an exception while preserving it's original stack trace.
        /// </summary>
        /// <param name="e">The exception to throw.</param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void CaptureAndThrow(Exception e)
        {
            ExceptionDispatchInfo.Capture(e).Throw();
        }
    }
}
