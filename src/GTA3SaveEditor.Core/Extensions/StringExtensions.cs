using System;

namespace GTA3SaveEditor.Core.Extensions
{
    public static class StringExtensions
    {
        public static string CreateBanner(this string s, char bannerChar, int sideLength)
        {
            if (sideLength < 0) throw new ArgumentOutOfRangeException(nameof(sideLength));

            string side = new string(bannerChar, sideLength);
            return $"{side} {s} {side}";
        }
    }
}
