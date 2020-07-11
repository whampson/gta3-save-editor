using GTASaveData.Types;
using System.Windows;

namespace GTA3SaveEditor.GUI.Extensions
{
    public static class VectorExtensions
    {
        public static Point ToPoint(this Vector2D v)
        {
            return new Point(v.X, v.Y);
        }
    }
}
