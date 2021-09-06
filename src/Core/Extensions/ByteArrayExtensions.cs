using System;
using System.Collections.Generic;
using System.Linq;

namespace GTA3SaveEditor.Core.Extensions
{
    public static class ByteArrayExtensions
    {
        public static List<int> IndexOfSequence(this byte[] buf, byte[] pattern, int startIndex = 0)
        {
            List<int> positions = new List<int>();
            int i = Array.IndexOf(buf, pattern[0], startIndex);
            while (i >= 0 && i <= buf.Length - pattern.Length)
            {
                byte[] segment = new byte[pattern.Length];
                Buffer.BlockCopy(buf, i, segment, 0, pattern.Length);
                if (segment.SequenceEqual(pattern))
                    positions.Add(i);
                i = Array.IndexOf(buf, pattern[0], i + 1);
            }
            return positions;
        }
    }
}
