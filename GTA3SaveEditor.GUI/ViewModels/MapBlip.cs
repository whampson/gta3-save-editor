using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public class MapBlip
    {
        public UIElement UIElement { get; set; }
        public Point WorldCoords { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Angle { get; set; }
        public double Scale { get; set; }
        public int ZIndex { get; set; }
        public string ToolTip { get; set; }

        public MapBlip()
        {
            Scale = 1;
        }

        public static MapBlip CreateFromSprite(Point worldCoords, string spriteUri,
            int size = 16, int decodeSize = 0,
            double angle = 0, double scale = 1,
            string toolTip = null)
        {
            int pxWidth = size;
            if (decodeSize > 0) pxWidth = decodeSize;

            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(spriteUri);
            bmp.DecodePixelWidth = pxWidth;
            bmp.EndInit();

            Image img = new Image()
            {
                Source = bmp,
                Width = bmp.Width,
                Height = bmp.Height,
                ToolTip = toolTip
            };

            return new MapBlip()
            {
                WorldCoords = worldCoords,
                UIElement = img,
                Height = img.Height,
                Width = img.Width,
                Angle = angle,
                Scale = ((double) size / pxWidth) * scale,
            };
        }
    }
}
