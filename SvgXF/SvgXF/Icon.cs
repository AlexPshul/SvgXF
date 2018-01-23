using System.IO;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

namespace SvgXF
{
    public class Icon : Frame
    {
        #region Private Members

        private readonly SKCanvasView _canvasView = new SKCanvasView();

        #endregion

        #region Bindable Properties

        #region ResourceId

        public static readonly BindableProperty IconFilePathProperty = BindableProperty.Create(
            nameof(ResourceId), typeof(string), typeof(Icon), default(string), propertyChanged: RedrawCanvas);

        public string ResourceId
        {
            get => (string)GetValue(IconFilePathProperty);
            set => SetValue(IconFilePathProperty, value);
        }

        #endregion

        #endregion

        #region Constructor

        public Icon()
        {
            Padding = new Thickness(0);
            Content = _canvasView;
            _canvasView.PaintSurface += CanvasViewOnPaintSurface;
        }

        #endregion

        #region Private Methods

        private static void RedrawCanvas(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (!(bindable is Icon svgIcon))
                return;

            svgIcon._canvasView.InvalidateSurface();
        }

        private void CanvasViewOnPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKCanvas canvas = args.Surface.Canvas;
            canvas.Clear();

            if (string.IsNullOrEmpty(ResourceId))
                return;

            SKImageInfo info = args.Info;

            SKSvg svg = new SKSvg();
            Stream stream = GetType().Assembly.GetManifestResourceStream(ResourceId);
            svg.Load(stream);

            SKRect bounds = svg.ViewBox;

            canvas.Translate(info.Width / 2f, info.Height / 2f);

            float ratio = bounds.Width > bounds.Height
                ? info.Width / bounds.Width
                : info.Height / bounds.Height;

            canvas.Scale(ratio);
            canvas.Translate(-bounds.MidX, -bounds.MidY);

            canvas.DrawPicture(svg.Picture);
        }

        #endregion
    }
}