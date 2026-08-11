using InspectionSketch.Models;
using System.Collections.Generic;
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
           Camera camera,
           Wall? selectedWall)
        {

            double normalDirection = GetWallNormalDirection(sketch);

            List<Rect> occupiedDimensionBounds = new();

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
                    end,
                    wall == selectedWall);

                double feet = wall.Length / sketch.Settings.PixelsPerFoot;

                Rect dimensionBounds = dimensionRenderer.Draw(
                   drawingContext,
                   start,
                   end,
                   feet,
                   normalDirection,
                   occupiedDimensionBounds);

                if (!dimensionBounds.IsEmpty)
                {
                    occupiedDimensionBounds.Add(dimensionBounds);
                }
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
