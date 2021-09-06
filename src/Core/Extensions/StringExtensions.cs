using System;
using System.Collections.Generic;
using System.Text;

namespace GTA3SaveEditor.Core.Extensions
{
    public static class StringExtensions
    {
        public static byte[] GetBytes(this string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        public static byte[] GetBytes(this string str, int length)
        {
            string s = str.PadRight(length - 1, '\0').Substring(0, length - 1) + '\0';
            return s.GetBytes();
        }
    }
}
