using System.Windows;
using System.Windows.Media;

namespace InspectionSketch.Drawing
{
    public class SketchRenderer
    {
        private readonly WallRenderer wallRenderer = new();
        private readonly DimensionRenderer dimensionRenderer = new();

        public void Draw(
            DrawingContext drawingContext,
            Sketch sketch,
            Camera camera)
        {
            foreach (var wall in sketch.Walls)
            {
                Point start = new Point(
                    camera.Offset.X + wall.StartPoint.X * camera.Zoom,
                    camera.Offset.Y + wall.StartPoint.Y * camera.Zoom);

                Point end = new Point(
                    camera.Offset.X + wall.EndPoint.X * camera.Zoom,
                    camera.Offset.Y + wall.EndPoint.Y * camera.Zoom);

                wallRenderer.DrawWall(
                    drawingContext,
                    start,
                    end);

                double feet = wall.Length / sketch.Settings.PixelsPerFoot;

                dimensionRenderer.Draw(
                    drawingContext,
                    start,
                    end,
                    feet);
            }
        }
    }
}