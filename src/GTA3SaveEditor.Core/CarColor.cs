using System.Drawing;

namespace GTA3SaveEditor.Core
{
    public class CarColor
    {
        public Color Color { get; }
        public string Name { get; }

        public CarColor(Color color)
            : this(color, "")
        { }

        public CarColor(Color color, string name)
        {
            Color = color;
            Name = name;
        }
    }
}
