using System;
using System.Windows;
using System.Windows.Media;
using InspectionSketch.Models;

namespace InspectionSketch.Drawing
{
    public class SketchCanvas : System.Windows.Controls.Canvas
    {
        private readonly Camera camera = new Camera();
        private readonly DrawingRenderer drawingRenderer = new();
        private readonly DimensionRenderer dimensionRenderer = new();
        private readonly WallRenderer wallRenderer = new();

        private bool isPanning = false;
        private Point lastPanPoint;
        private Point? wallStartPoint = null;
        private Point? wallPreviewPoint = null;
        private readonly Sketch sketch = new();

        public SketchCanvas()
        {
            ClipToBounds = true;
            Focusable = true;
            Cursor = System.Windows.Input.Cursors.Cross;

            drawingRenderer.GridSpacing = DrawingRenderer.DefaultGridSpacing;

            MouseWheel += SketchCanvas_MouseWheel;

            MouseDown += SketchCanvas_MouseDown;
            MouseMove += SketchCanvas_MouseMove;
            MouseUp += SketchCanvas_MouseUp;
            KeyDown += SketchCanvas_KeyDown;
        }





        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            InvalidateVisual();
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingRenderer.Zoom = camera.Zoom;
            drawingRenderer.Offset = camera.Offset;
            drawingRenderer.Draw(drawingContext, RenderSize);
            
            

            foreach (var wall in sketch.Walls)
            {
                Point start = new Point(
                    camera.Offset.X + wall.StartPoint.X * camera.Zoom,
                    camera.Offset.Y + wall.StartPoint.Y * camera.Zoom);

                Point end = new Point(
                    camera.Offset.X + wall.EndPoint.X * camera.Zoom,
                    camera.Offset.Y + wall.EndPoint.Y * camera.Zoom);

                drawingContext.DrawLine(
                    new Pen(Brushes.Black, 2),
                    start,
                    end);

                double deltaX = wall.EndPoint.X - wall.StartPoint.X;
                double deltaY = wall.EndPoint.Y - wall.StartPoint.Y;

                double pixelLength = Math.Sqrt(
                    deltaX * deltaX +
                    deltaY * deltaY);

                double feet = pixelLength / sketch.Settings.PixelsPerFoot;

                dimensionRenderer.Draw(
                    drawingContext,
                    start,
                    end,
                    feet);
            }
                
            if (wallStartPoint != null && wallPreviewPoint != null)
            {
                Pen previewPen = new Pen(Brushes.RoyalBlue, 2);

                Point previewStart = new Point(
                    camera.Offset.X + wallStartPoint.Value.X * camera.Zoom,
                    camera.Offset.Y + wallStartPoint.Value.Y * camera.Zoom);

                Point previewEnd = new Point(
                    camera.Offset.X + wallPreviewPoint.Value.X * camera.Zoom,
                    camera.Offset.Y + wallPreviewPoint.Value.Y * camera.Zoom);

                drawingContext.DrawLine(
                    previewPen,
                    previewStart,
                    previewEnd);
            }
        }
        private void SketchCanvas_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            Point mousePosition = e.GetPosition(this);

            if (e.Delta > 0)
            {
                camera.ZoomAt(mousePosition, 1.1);
            }
            else
            {
                camera.ZoomAt(mousePosition, 1 / 1.1);
            }

            InvalidateVisual();
        }
        private void SketchCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Windows.Input.Keyboard.Focus(this);

            if (e.ChangedButton == System.Windows.Input.MouseButton.Middle)
            {
                isPanning = true;
                lastPanPoint = e.GetPosition(this);
                CaptureMouse();
            }

            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                Point clickPoint = SnapToGrid(e.GetPosition(this));

                if (wallStartPoint == null)
                {
                    wallStartPoint = clickPoint;
                }
                else
                {
                    sketch.Walls.Add(new Wall
                    {
                        StartPoint = wallStartPoint.Value,
                        EndPoint = clickPoint
                    });

                    wallStartPoint = clickPoint;
                    wallPreviewPoint = clickPoint;

                    InvalidateVisual();
                }
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
            if (wallStartPoint != null && !isPanning)
            {
                wallPreviewPoint = SnapToGrid(e.GetPosition(this));
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
        private Point SnapToGrid(Point point)
        {
            double worldX = (point.X - camera.Offset.X) / camera.Zoom;
            double worldY = (point.Y - camera.Offset.Y) / camera.Zoom;

            double spacing = DrawingRenderer.DefaultGridSpacing;

            double snappedX =
                Math.Round(worldX / spacing) * spacing;

            double snappedY =
                Math.Round(worldY / spacing) * spacing;

            return new Point(snappedX, snappedY);
        }
        private void SketchCanvas_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F)
            {
                camera.Reset();
                InvalidateVisual();
            }

            if (e.Key == System.Windows.Input.Key.Escape)
            {
                wallStartPoint = null;
                wallPreviewPoint = null;
                InvalidateVisual();
            }

        }
    }
    }
