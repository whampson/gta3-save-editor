using GTA3SaveEditor.GUI.Extensions;
using GTASaveData.Types;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Converters;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace GTA3SaveEditor.GUI.Helpers
{
    public static class MapHelper
    {
        public static readonly Point MapScaleFactor = new Point(0.256, -0.256);
        public static readonly Point MapOrigin = new Point(510, 512);

        public static UIElement MakeBlip(Vector2D loc,
            int scale = 3, double thickness = 0.5, int angle = 0,
            int color = 0, bool isBright = true,
            string toolTip = null)
        {
            const double Size = 2;
            double actualSize = Size * scale;

            SolidColorBrush brush = new SolidColorBrush
            {
                Color = GetBlipColor(color, isBright)
            };

            Rectangle rect = new Rectangle
            {
                Fill = brush,
                StrokeThickness = thickness,
                Stroke = Brushes.Black,
                Width = actualSize,
                Height = actualSize,
                ToolTip = toolTip
            };

            ApplyTransform(rect, loc, angle, actualSize);
            return rect;
        }

        public static UIElement MakeIconBlip(Vector2D loc, string iconUri,
            int scale = 4, int angle = 0,
            string toolTip = null)
        {
            const double Size = 4;
            double actualSize = Size * scale;

            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(iconUri);
            bmp.EndInit();

            Image img = new Image()
            {
                Source = bmp,
                Width = bmp.Width,
                Height = bmp.Height,
                ToolTip = toolTip
            };

            ApplyTransform(img, loc, angle, actualSize);
            return img;
        }

        private static void ApplyTransform(FrameworkElement e, Vector2D loc,
            double angle = 0, double size = 1)
        {
            const double Root2Over2 = 0.707107;
            double hypAngleRad = (Math.PI * (angle + 45)) / 180.0;

            double centerX = (size / 2) * (Math.Cos(hypAngleRad) / Root2Over2);
            double centerY = (size / 2) * (Math.Sin(hypAngleRad) / Root2Over2);
            double scaleX = size / e.Width;
            double scaleY = size / e.Height;

            Point p = GetPixelCoords(loc.ToPoint());
            Matrix m = Matrix.Identity;
            m.Rotate(angle);
            m.OffsetX = p.X - centerX;
            m.OffsetY = p.Y - centerY;
            m.ScalePrepend(scaleX, scaleY);

            e.RenderTransform = new MatrixTransform(m);
        }

        public static Point GetPixelCoords(Point loc)
        {
            return new Point()
            {
                X = (loc.X * MapScaleFactor.X) + MapOrigin.X,
                Y = (loc.Y * MapScaleFactor.Y) + MapOrigin.Y,
            };
        }

        public static Color GetBlipColor(int colorId, bool isBright)
        {
            if (colorId >= 0 && colorId < NumStandardColorTypes)
            {
                int colorIndex = (isBright) ? (colorId * 2) + 1 : colorId * 2;
                return (Color) StandardBlipColors[colorIndex].Color;
            }

            // Interesting "feature" in the game
            byte r = (byte) (colorId >> 24);
            byte g = (byte) (colorId >> 16);
            byte b = (byte) (colorId >> 8);
            return Color.FromRgb(r, g, b);
        }

        public const int NumStandardColorTypes = 7;

        public static ObservableCollection<ColorItem> StandardBlipColors => new ObservableCollection<ColorItem>()
        {
            new ColorItem(Color.FromRgb(0x7F, 0x00, 0x00), "Dark Red"),
            new ColorItem(Color.FromRgb(0x71, 0x2B, 0x49), "Red"),
            new ColorItem(Color.FromRgb(0x00, 0x7F, 0x00), "Dark Green"),
            new ColorItem(Color.FromRgb(0x5F, 0xA0, 0x6A), "Green"),
            new ColorItem(Color.FromRgb(0x00, 0x00, 0x7F), "Dark Blue"),
            new ColorItem(Color.FromRgb(0x80, 0xA7, 0xF3), "Blue"),
            new ColorItem(Color.FromRgb(0x7F, 0x7F, 0x7F), "Gray"),
            new ColorItem(Color.FromRgb(0xE1, 0xE1, 0xE1), "White"),
            new ColorItem(Color.FromRgb(0x7F, 0x7F, 0x00), "Dark Yellow"),
            new ColorItem(Color.FromRgb(0xFF, 0xFF, 0x00), "Yellow"),
            new ColorItem(Color.FromRgb(0x7F, 0x00, 0x7F), "Purple"),
            new ColorItem(Color.FromRgb(0xFF, 0x00, 0xFF), "Pink"),
            new ColorItem(Color.FromRgb(0x00, 0x7F, 0x7F), "Teal"),
            new ColorItem(Color.FromRgb(0x00, 0xFF, 0xFF), "Cyan"),
        };
    }
}
