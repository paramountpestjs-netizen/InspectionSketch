using System.Windows;
using System.Windows.Media;

namespace InspectionSketch.Drawing
{
    public class SketchCanvas : System.Windows.Controls.Canvas
    {
        private readonly Camera camera = new Camera();
        private readonly DrawingRenderer drawingRenderer = new();

        public double GridSpacing { get; set; } = 25;

        public SketchCanvas()
        {
            ClipToBounds = true;
            Focusable = true;

            MouseWheel += SketchCanvas_MouseWheel;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingRenderer.Draw(drawingContext, RenderSize);

        }

        

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            InvalidateVisual();
        }
        private void SketchCanvas_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                camera.ZoomIn(1.1);
            }
            else
            {
                camera.ZoomOut(1.1);
            }

            InvalidateVisual();
        }
    }
}