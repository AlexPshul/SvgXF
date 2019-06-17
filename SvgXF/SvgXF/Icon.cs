using System;
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

        public static readonly BindableProperty ResourceIdProperty = BindableProperty.Create(
            nameof(ResourceId), typeof(string), typeof(Icon), default(string), propertyChanged: RedrawCanvas);

        public string ResourceId
        {
            get => (string)GetValue(ResourceIdProperty);
            set => SetValue(ResourceIdProperty, value);
        }

        #endregion

        #region Color

        public static readonly BindableProperty ColorProperty = BindableProperty.Create(
            nameof(Color), typeof(Color), typeof(SVGIcon),Color.Black, propertyChanged: RedrawCanvas);

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        #endregion

        #endregion

        #region Constructor

        public Icon()
        {
            Padding = new Thickness(0);
            BackgroundColor = Color.Transparent;
            HasShadow = false;
            Content = _canvasView;
            _canvasView.PaintSurface += CanvasViewOnPaintSurface;
        }

        #endregion

        #region Private Methods

        private static void RedrawCanvas(BindableObject bindable, object oldvalue, object newvalue)
        {
            Icon svgIcon = bindable as Icon;
            svgIcon?._canvasView.InvalidateSurface();
        }

        private void CanvasViewOnPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKCanvas canvas = args.Surface.Canvas;
            canvas.Clear();

            if (string.IsNullOrEmpty(ResourceId))
                return;

            using (Stream stream = GetType().Assembly.GetManifestResourceStream(ResourceId))
            {
                SKSvg svg = new SKSvg();
                svg.Load(stream);

                SKImageInfo info = args.Info;
                canvas.Translate(info.Width / 2f, info.Height / 2f);

                SKRect bounds = svg.ViewBox;
                float xRatio = info.Width / bounds.Width;
                float yRatio = info.Height / bounds.Height;

                float ratio = Math.Min(xRatio, yRatio);

                canvas.Scale(ratio);
                canvas.Translate(-bounds.MidX, -bounds.MidY);

                var paint = new SKPaint()
                {
                    ColorFilter = SKColorFilter.CreateBlendMode(
                        this.Color.ToSKColor(),
                        SKBlendMode.SrcIn
                    )
                };

                canvas.DrawPicture(svg.Picture,paint);
            }
        }

        #endregion
    }
}