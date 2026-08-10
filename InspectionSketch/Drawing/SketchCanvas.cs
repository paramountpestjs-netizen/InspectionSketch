using System.Windows;
using System.Windows.Media;

namespace InspectionSketch.Drawing
{
    public class SketchCanvas : System.Windows.Controls.Canvas
    {
        private readonly Camera camera = new Camera();
        private readonly DrawingRenderer drawingRenderer = new();

        private bool isPanning = false;
        private Point lastPanPoint;


        public SketchCanvas()
        {
            ClipToBounds = true;
            Focusable = true;
            drawingRenderer.GridSpacing = DrawingRenderer.DefaultGridSpacing;
            MouseWheel += SketchCanvas_MouseWheel;

            MouseDown += SketchCanvas_MouseDown;
            MouseMove += SketchCanvas_MouseMove;
            MouseUp += SketchCanvas_MouseUp;
        }
        
        protected override void OnRender(DrawingContext drawingContext)
        {
            
            base.OnRender(drawingContext);


            drawingRenderer.Zoom = camera.Zoom;
            drawingRenderer.Offset = camera.Offset;
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
        private void SketchCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Middle)
            {
                isPanning = true;
                lastPanPoint = e.GetPosition(this);
                CaptureMouse();
            }
        }

        private void SketchCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isPanning)
            {
                Point currentPoint = e.GetPosition(this);

                Vector delta = currentPoint - lastPanPoint;

                camera.Pan(delta);

                lastPanPoint = currentPoint;

                InvalidateVisual();
            }
        }

        private void SketchCanvas_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Middle)
            {
                isPanning = false;
                ReleaseMouseCapture();
            }
        }
    }
}