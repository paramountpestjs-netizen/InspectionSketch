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

            double normalDirection = GetWallNormalDirection(sketch);

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
                    feet,
                    normalDirection);
            }
        }

        private double GetWallNormalDirection(Sketch sketch)
        {
            if (sketch.Walls.Count < 3)
                return 1.0;

            double signedArea = 0;

            foreach (var wall in sketch.Walls)
            {
                signedArea +=
                    wall.StartPoint.X * wall.EndPoint.Y -
                    wall.StartPoint.Y * wall.EndPoint.X;
            }

            if (signedArea > 0)
                return -1.0;

            return 1.0;
        }
    }
}
