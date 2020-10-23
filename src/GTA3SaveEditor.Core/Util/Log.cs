using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace GTA3SaveEditor.Core.Util
{
    public static class Log
    {
        public static event EventHandler LogEvent;

        public static string InfoPrefix  => $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}  Info:  ";
        public static string ErrorPrefix => $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} Error:  ";
        public static string DebugPrefix => $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} Debug:  ";

        public static TextWriter InfoStream { get; set; }
        public static TextWriter ErrorStream { get; set; }
        public static TextWriter DebugStream { get; set; }
        public static bool AutomaticNewline { get; set; }

        static Log()
        {
            InfoStream = Console.Out;
            ErrorStream = Console.Error;
            DebugStream = Console.Out;
            AutomaticNewline = true;
        }

        public static void Info(object message, params object[] args)
        {
            WriteLogEntry(InfoStream, InfoPrefix, message, args);
        }

        public static void Error(object message, params object[] args)
        {
            WriteLogEntry(ErrorStream, ErrorPrefix, message, args);
        }

        public static void Exception(Exception e, [CallerMemberName] string caller = null)
        {
            Error($"{caller}(): {e.GetType().Name}: {e.Message}");
        }

        [Conditional("DEBUG")]
        public static void Debug(object message, params object[] args)
        {
            WriteLogEntry(DebugStream, DebugPrefix, message, args);
        }

        private static void WriteLogEntry(TextWriter stream, string prefix, object message, params object[] args)
        {
            string entry = prefix;
            entry += (args.Length == 0)
                ? message
                : string.Format(message as string, args);

            if (AutomaticNewline)
                stream.WriteLine(entry);
            else
                stream.Write(entry);

            LogEvent?.Invoke(null, EventArgs.Empty);
        }
    }
}
