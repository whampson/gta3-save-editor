using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace GTA3SaveEditor.Core
{
    public static class Log
    {
        static Log()
        {
            AutomaticNewline = true;
            EnableVerbosity = false;
            InfoStream = Console.Out;
            ErrorStream = Console.Error;
            ErrorPrefix = "Error: ";
        }

        public static bool AutomaticNewline { get; set; }
        public static bool EnableVerbosity { get; set; }
        public static TextWriter InfoStream { get; set; }
        public static TextWriter ErrorStream { get; set; }
        public static string InfoPrefix { get; set; }
        public static string ErrorPrefix { get; set; }

        public static void Info(object value)
        {
            string txt = InfoPrefix + value.ToString();

            if (AutomaticNewline)
                InfoStream.WriteLine(txt);
            else
                InfoStream.Write(txt);
        }

        public static void InfoF(string format, params object[] args)
        {
            Info(string.Format(format, args));
        }

        public static void InfoV(object value)
        {
            if (EnableVerbosity)
                Info(value);
        }

        public static void InfoVF(string format, params object[] args)
        {
            InfoV(string.Format(format, args));
        }

        public static void Error(object value)
        {
            string txt = ErrorPrefix + value.ToString();

            if (AutomaticNewline)
                ErrorStream.WriteLine(txt);
            else
                ErrorStream.Write(txt);
        }

        public static void ErrorF(string format, params object[] args)
        {
            Error(string.Format(format, args));
        }

        public static void Exception(Exception e, [CallerMemberName] string caller = null)
        {
            Error($"{caller}(): {e.GetType().Name}: {e.Message}");
        }
    }
}
