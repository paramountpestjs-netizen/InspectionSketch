using System.Windows;
using System.Windows.Media;
using InspectionSketch.Models;

namespace InspectionSketch.Drawing
{
    public class WallRenderer
    {
        public void Draw(
            DrawingContext drawingContext,
            Sketch sketch,
            Camera camera)
        {
            Pen wallPen = new Pen(Brushes.Black, 2);

            foreach (var wall in sketch.Walls)
            {
                Point start = new Point(
                    camera.Offset.X + wall.StartPoint.X * camera.Zoom,
                    camera.Offset.Y + wall.StartPoint.Y * camera.Zoom);

                Point end = new Point(
                    camera.Offset.X + wall.EndPoint.X * camera.Zoom,
                    camera.Offset.Y + wall.EndPoint.Y * camera.Zoom);

                drawingContext.DrawLine(
                    wallPen,
                    start,
                    end);
            }
        }
    }
}