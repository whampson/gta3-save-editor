using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GTA3SaveEditor.GUI.Controls
{
    /// <summary>
    /// Interaction logic for MapControl.xaml
    /// </summary>
    public partial class MapControl : UserControl
    {
        #region Defaults
        public static readonly Vector DefaultScale = new Vector(1, 1);
        public const double DefaultZoom = 1.0;
        public const double DefaultMinimumZoom = 0.1;
        public const double DefaultMaximumZoom = 4.0;
        public const double DefaultZoomDelta = 0.1;
        public const bool DefaultPanWithMouseDrag = true;
        public const bool DefaultZoomWithMouseWheel = true;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the render transform matrix.
        /// </summary>
        public Matrix RenderMatrix
        {
            get { return (Matrix) GetValue(RenderMatrixProperty); }
            set { SetValue(RenderMatrixProperty, value); }
        }

        /// <summary>
        /// Gets or sets the map image.
        /// </summary>
        public BitmapImage Image
        {
            get { return (BitmapImage) GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        /// <summary>
        /// Gets or sets the map-to-world distance scale factor.
        /// </summary>
        /// <remarks>
        /// The map distance is measured in pixels. A scale factor of
        /// 0.5,1 means that every pixel 1 in the X direction represents
        /// 2 units in the world, and every 1 pixel in the Y direction
        /// represents 1 unit in the world.
        /// </remarks>
        public Vector Scale
        {
            get { return (Vector) GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the map origin.
        /// </summary>
        /// <remarks>
        /// The origin point is measured as a pixel offset from
        /// the top left corner of the map iamge.
        /// </remarks>
        public Point Origin
        {
            get { return (Point) GetValue(OriginProperty); }
            set { SetValue(OriginProperty, value); }
        }

        /// <summary>
        /// Gets or sets the map pan offset.
        /// </summary>
        /// <remarks>
        /// The pan offset is measured as the pixel offset of
        /// the map origin from the center of the viewport.
        /// </remarks>
        public Point Offset
        {
            get { return (Point) GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        /// <summary>
        /// Gets or sets the initial offset value.
        /// </summary>
        /// <remarks>
        /// The <see cref="Offset"/> property is set to this value
        /// when <see cref="Reset"/> is called.
        /// </remarks>
        public Point InitialOffset
        {
            get { return (Point) GetValue(InitialOffsetProperty); }
            set { SetValue(InitialOffsetProperty, value); }
        }

        /// <summary>
        /// Gets or sets the zoom factor.
        /// </summary>
        public double Zoom
        {
            get { return (double) GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        /// <summary>
        /// Gets or sets the initial zoom value.
        /// </summary>
        /// <remarks>
        /// The <see cref="Zoom"/> property is set to this value
        /// when <see cref="Reset"/> is called.
        /// </remarks>
        public double InitialZoom
        {
            get { return (double) GetValue(InitialZoomProperty); }
            set { SetValue(InitialZoomProperty, value); }
        }

        /// <summary>
        /// Gets or sets the minimum zoom value.
        /// </summary>
        /// <remarks>
        /// When <see cref="ZoomWithMouseWheel"/> is enabled, the actual
        /// minimum zoom value may be slightly above or below this value
        /// depending on the value of <see cref="ZoomDelta"/>.
        /// </remarks>
        public double MinimumZoom
        {
            get { return (double) GetValue(MinimumZoomProperty); }
            set { SetValue(MinimumZoomProperty, value); }
        }

        /// <summary>
        /// Gets or sets the maximum zoom value.
        /// </summary>
        /// <remarks>
        /// When <see cref="ZoomWithMouseWheel"/> is enabled, the actual
        /// maximum zoom value may be slightly above or below this value
        /// depending on the value of <see cref="ZoomDelta"/>.
        /// </remarks>
        public double MaximumZoom
        {
            get { return (double) GetValue(MaximumZoomProperty); }
            set { SetValue(MaximumZoomProperty, value); }
        }

        /// <summary>
        /// The amount by which to scale the <see cref="Zoom"/> factor
        /// when <see cref="ZoomWithMouseWheel"/> is enabled and the
        /// mouse wheel is moved.
        /// </summary>
        public double ZoomDelta
        {
            get { return (double) GetValue(ZoomDeltaProperty); }
            set { SetValue(ZoomDeltaProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether to zoom the map by moving the mouse wheel.
        /// </summary>
        public bool ZoomWithMouseWheel
        {
            get { return (bool) GetValue(ZoomWithMouseWheelProperty); }
            set { SetValue(ZoomWithMouseWheelProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether to pan the map by clicking and dragging the mouse.
        /// </summary>
        public bool PanWithMouseDrag
        {
            get { return (bool) GetValue(PanWithMouseDragProperty); }
            set { SetValue(PanWithMouseDragProperty, value); }
        }

        /// <summary>
        /// Gets or sets the mouse button that must be held down
        /// for map panning to occur when <see cref="PanWithMouseDrag"/>
        /// is enabled.
        /// </summary>
        public MouseButton PanWithMouseDragButton
        {
            get { return (MouseButton) GetValue(PanWithMouseDragButtonProperty); }
            set { SetValue(PanWithMouseDragButtonProperty, value); }
        }

        /// <summary>
        /// Gets the point in the control area where the mouse was last clicked.
        /// </summary>
        public Point MouseClickPoint
        {
            get { return (Point) GetValue(MouseClickPointProperty); }
            set { SetValue(MouseClickPointProperty, value); }
        }

        /// <summary>
        /// Gets the pixel offset from the <see cref="Origin"/> where
        /// the mouse was last clicked.
        /// </summary>
        public Point MouseClickOffset
        {
            get { return (Point) GetValue(MouseClickOffsetProperty); }
            set { SetValue(MouseClickOffsetProperty, value); }
        }

        /// <summary>
        /// Gets the world coordinates where the mouse was last clicked.
        /// </summary>
        public Point MouseClickCoords
        {
            get { return (Point) GetValue(MouseClickCoordsProperty); }
            set { SetValue(MouseClickCoordsProperty, value); }
        }

        /// <summary>
        /// Gets the point in the control area where the mouse was last moved.
        /// </summary>
        public Point MouseOverPoint
        {
            get { return (Point) GetValue(MouseOverPointProperty); }
            set { SetValue(MouseOverPointProperty, value); }
        }

        /// <summary>
        /// Gets the pixel offset from the <see cref="Origin"/> where
        /// the mouse was last moved.
        /// </summary>
        public Point MouseOverOffset
        {
            get { return (Point) GetValue(MouseOverOffsetProperty); }
            set { SetValue(MouseOverOffsetProperty, value); }
        }

        /// <summary>
        /// Gets the world coordinates where the mouse was last moved.
        /// </summary>
        public Point MouseOverCoords
        {
            get { return (Point) GetValue(MouseOverCoordsProperty); }
            set { SetValue(MouseOverCoordsProperty, value); }
        }
        #endregion

        #region Dependency Properties
        public static readonly DependencyProperty RenderMatrixProperty = DependencyProperty.Register(
            nameof(RenderMatrix), typeof(Matrix), typeof(MapControl), new PropertyMetadata(default(Matrix)));

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
            nameof(Image), typeof(BitmapImage), typeof(MapControl), new PropertyMetadata(default(BitmapImage)));

        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
            nameof(Scale), typeof(Vector), typeof(MapControl), new PropertyMetadata(DefaultScale));

        public static readonly DependencyProperty OriginProperty = DependencyProperty.Register(
            nameof(Origin), typeof(Point), typeof(MapControl), new PropertyMetadata(default(Point)));

        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(
            nameof(Offset), typeof(Point), typeof(MapControl), new FrameworkPropertyMetadata(
                default(Point),
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OffsetPropertyChanged));

        public static readonly DependencyProperty InitialOffsetProperty = DependencyProperty.Register(
            nameof(InitialOffset), typeof(Point), typeof(MapControl), new PropertyMetadata(default(Point)));

        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(
            nameof(Zoom), typeof(double), typeof(MapControl), new FrameworkPropertyMetadata(
                DefaultZoom,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                ZoomPropertyChanged));

        public static readonly DependencyProperty InitialZoomProperty = DependencyProperty.Register(
            nameof(InitialZoom), typeof(double), typeof(MapControl), new PropertyMetadata(DefaultZoom));

        public static readonly DependencyProperty MinimumZoomProperty = DependencyProperty.Register(
            nameof(MinimumZoom), typeof(double), typeof(MapControl), new PropertyMetadata(DefaultMinimumZoom));

        public static readonly DependencyProperty MaximumZoomProperty = DependencyProperty.Register(
            nameof(MaximumZoom), typeof(double), typeof(MapControl), new PropertyMetadata(DefaultMaximumZoom));

        public static readonly DependencyProperty ZoomDeltaProperty = DependencyProperty.Register(
            nameof(ZoomDelta), typeof(double), typeof(MapControl), new PropertyMetadata(DefaultZoomDelta));

        public static readonly DependencyProperty PanWithMouseDragProperty = DependencyProperty.Register(
            nameof(PanWithMouseDrag), typeof(bool), typeof(MapControl), new PropertyMetadata(DefaultPanWithMouseDrag));

        public static readonly DependencyProperty ZoomWithMouseWheelProperty = DependencyProperty.Register(
            nameof(ZoomWithMouseWheel), typeof(bool), typeof(MapControl), new PropertyMetadata(DefaultZoomWithMouseWheel));

        public static readonly DependencyProperty PanWithMouseDragButtonProperty = DependencyProperty.Register(
            nameof(PanWithMouseDragButton), typeof(MouseButton), typeof(MapControl), new PropertyMetadata(default(MouseButton)));

        // TODO: make these readonly
        public static readonly DependencyProperty MouseClickPointProperty = DependencyProperty.Register(
            nameof(MouseClickPoint), typeof(Point), typeof(MapControl), new FrameworkPropertyMetadata(
                default(Point),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty MouseClickOffsetProperty = DependencyProperty.Register(
            nameof(MouseClickOffset), typeof(Point), typeof(MapControl), new FrameworkPropertyMetadata(
                default(Point),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty MouseClickCoordsProperty = DependencyProperty.Register(
            nameof(MouseClickCoords), typeof(Point), typeof(MapControl), new FrameworkPropertyMetadata(
                default(Point),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty MouseOverPointProperty = DependencyProperty.Register(
            nameof(MouseOverPoint), typeof(Point), typeof(MapControl), new FrameworkPropertyMetadata(
                default(Point),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty MouseOverOffsetProperty = DependencyProperty.Register(
            nameof(MouseOverOffset), typeof(Point), typeof(MapControl), new FrameworkPropertyMetadata(
                default(Point),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty MouseOverCoordsProperty = DependencyProperty.Register(
            nameof(MouseOverCoords), typeof(Point), typeof(MapControl), new FrameworkPropertyMetadata(
                default(Point),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        #endregion

        #region Private Fields
        private Point m_prePanRenderOffset;
        private bool m_suppressZoomChangedHandler;
        private bool m_suppressOffsetChangedHandler;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="MapControl"/> instance.
        /// </summary>
        public MapControl()
        {
            InitializeComponent();
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Resets the pan offset and zoom to their initial values.
        /// </summary>
        public void Reset()
        {
            RenderMatrix = Matrix.Identity;
            Offset = InitialOffset;
            Zoom = InitialZoom;
        }

        /// <summary>
        /// Translates a set of pixel coordinates into a set of world coordinates.
        /// </summary>
        /// <remarks>
        /// Pixel coordinates are measured from the top-left of the map image.
        /// World coordinates are determined based on the <see cref="Origin"/>
        /// and <see cref="Scale"/> properties.
        /// </remarks>
        public Point PixelToWorldCoords(Point pixel)
        {
            return new Point()
            {
                X = (pixel.X - Origin.X) / Scale.X,
                Y = (pixel.Y - Origin.Y) / Scale.Y,
            };
        }

        /// <summary>
        /// Translates a set of pixel coordinates into a set of map coordinates.
        /// </summary>
        /// <remarks>
        /// Pixel coordinates are measured from the top-left of the map image.
        /// Map coordinates are pixel coordinates centered about the <see cref="Origin"/>.
        /// </remarks>
        public Point PixelToMapCoords(Point pixel)
        {
            return new Point()
            {
                X = pixel.X - Origin.X,
                Y = pixel.Y - Origin.Y
            };
        }

        /// <summary>
        /// Translates a set of world coordinates into a set of pixel coordinates.
        /// </summary>
        /// <remarks>
        /// Pixel coordinates are measured from the top-left of the map image.
        /// World coordinates are determined based on the <see cref="Origin"/>
        /// and <see cref="Scale"/> properties.
        /// </remarks>
        public Point WorldToPixelCoords(Point world)
        {
            return new Point()
            {
                X = (world.X * Scale.X) + Origin.X,
                Y = (world.Y * Scale.Y) + Origin.Y,
            };
        }

        /// <summary>
        /// Translates a set of map coordinates into a set of pixel coordinates.
        /// </summary>
        /// <remarks>
        /// Pixel coordinates are measured from the top-left of the map image.
        /// Map coordinates are pixel coordinates centered about the <see cref="Origin"/>.
        /// </remarks>
        public Point MapToPixelCoords(Point map)
        {
            return new Point()
            {
                X = map.X + Origin.X,
                Y = map.Y + Origin.Y,
            };
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Applies the current zoom value over the current pan offset.
        /// </summary>
        private void ApplyZoom()
        {
            Matrix m = Matrix.Identity;
            m.ScalePrepend(Zoom, Zoom);
            RenderMatrix = m;

            ApplyOffset();
        }

        /// <summary>
        /// Applies an incremental zoom over a point.
        /// </summary>
        private void ZoomIncrementOverPoint(Point p, double delta)
        {
            if (Zoom < MinimumZoom && delta < 0) return;
            if (Zoom > MaximumZoom && delta > 0) return;

            double scale = 1;
            if (delta > 0) scale = (delta + 1);
            if (delta < 0) scale = (1 / (-delta + 1));

            Matrix m = RenderMatrix;
            m.ScaleAtPrepend(scale, scale, p.X, p.Y);
            RenderMatrix = m;

            m_suppressZoomChangedHandler = true;
            Zoom *= scale;
            m_suppressZoomChangedHandler = false;

            UpdateOffset();
        }

        /// <summary>
        /// Sets the Offset property according to current render matrix translation offset.
        /// </summary>
        private void UpdateOffset()
        {
            Point t = GetRenderTranslate();
            double x = Origin.X + ((t.X - (m_border.ActualWidth / 2)) / Zoom);
            double y = Origin.Y + ((t.Y - (m_border.ActualHeight / 2)) / Zoom);

            m_suppressOffsetChangedHandler = true;
            Offset = new Point(x, y);
            m_suppressOffsetChangedHandler = false;
        }

        /// <summary>
        /// Translates the render matrix according to Offset property.
        /// </summary>
        private void ApplyOffset()
        {
            double tX = (m_border.ActualWidth / 2) - ((Origin.X - Offset.X) * Zoom);
            double tY = (m_border.ActualHeight / 2) - ((Origin.Y - Offset.Y) * Zoom);
            SetRenderTranslate(tX, tY);
        }

        /// <summary>
        /// Gets the current render matrix translation offset.
        /// </summary>
        private Point GetRenderTranslate()
        {
            return new Point(RenderMatrix.OffsetX, RenderMatrix.OffsetY);
        }

        /// <summary>
        /// Sets the render matrix translation offset.
        /// </summary>
        private void SetRenderTranslate(double x, double y)
        {
            Matrix m = RenderMatrix;
            m.OffsetX = x;
            m.OffsetY = y;
            RenderMatrix = m;
        }
        #endregion

        #region Event Handlers
        private void MapControl_Loaded(object sender, RoutedEventArgs e)
        {
            Reset();
            ApplyZoom();
            ApplyOffset();
        }

        private void MapControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!m_canvas.IsMouseCaptured)
            {
                MouseClickPoint = e.MouseDevice.GetPosition(m_border);
                MouseClickOffset = PixelToMapCoords(e.MouseDevice.GetPosition(m_image));
                MouseClickCoords = PixelToWorldCoords(e.MouseDevice.GetPosition(m_image));

                if (PanWithMouseDrag && PanWithMouseDragButton == e.ChangedButton)
                {
                    m_prePanRenderOffset = GetRenderTranslate();
                    m_canvas.CaptureMouse();
                }
            }
        }

        private void MapControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (m_canvas.IsMouseCaptured)
            {
                m_canvas.ReleaseMouseCapture();
            }
        }

        private void MapControl_MouseMove(object sender, MouseEventArgs e)
        {
            MouseOverPoint = e.MouseDevice.GetPosition(m_border);
            
            if (m_canvas.IsMouseCaptured)
            {
                Point p = e.MouseDevice.GetPosition(m_border);
                double x = m_prePanRenderOffset.X + (p.X - MouseClickPoint.X);
                double y = m_prePanRenderOffset.Y + (p.Y - MouseClickPoint.Y);
                SetRenderTranslate(x, y);
                UpdateOffset();
            }
            else
            {
                MouseOverOffset = PixelToMapCoords(e.MouseDevice.GetPosition(m_image));
                MouseOverCoords = PixelToWorldCoords(e.MouseDevice.GetPosition(m_image));
            }
        }

        private void MapControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!m_canvas.IsMouseCaptured && ZoomWithMouseWheel)
            {
                Point p = e.MouseDevice.GetPosition(m_canvas);
                double increment = (e.Delta > 0) ? +ZoomDelta : -ZoomDelta;
                ZoomIncrementOverPoint(p, increment);
            }
        }
        #endregion

        #region Property Changed Handlers
        private static void OffsetPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is MapControl mapControl)
            {
                if (!mapControl.m_suppressOffsetChangedHandler)
                {
                    mapControl.ApplyOffset();
                }
            }
        }

        private static void ZoomPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is MapControl mapControl)
            {
                if (!mapControl.m_suppressZoomChangedHandler)
                {
                    mapControl.ApplyZoom();
                }
            }
        }
        #endregion
    }
}