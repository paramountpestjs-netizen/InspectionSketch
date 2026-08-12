using System;
using InspectionSketch.Models;

namespace InspectionSketch.Services
{
    public class SketchMeasurementService
    {
        public double CalculatePerimeterFeet(Sketch sketch)
        {
            double totalPixels = 0;

            foreach (Wall wall in sketch.Walls)
            {
                totalPixels += wall.Length;
            }

            return totalPixels / sketch.Settings.PixelsPerFoot;
        }
        public bool IsClosedOutline(Sketch sketch)
        {
            if (sketch.Walls.Count < 3)
                return false;

            for (int i = 0; i < sketch.Walls.Count - 1; i++)
            {
                Wall currentWall = sketch.Walls[i];
                Wall nextWall = sketch.Walls[i + 1];

                if (currentWall.EndPoint != nextWall.StartPoint)
                    return false;
            }

            Wall lastWall = sketch.Walls[sketch.Walls.Count - 1];
            Wall firstWall = sketch.Walls[0];

            return lastWall.EndPoint == firstWall.StartPoint;
        }
        public double CalculateAreaSquareFeet(Sketch sketch)
        {
            if (!IsClosedOutline(sketch))
                return 0;

            double areaPixelsSquared = 0;

            foreach (Wall wall in sketch.Walls)
            {
                areaPixelsSquared +=
                    wall.StartPoint.X * wall.EndPoint.Y -
                    wall.EndPoint.X * wall.StartPoint.Y;
            }

            areaPixelsSquared =
                Math.Abs(areaPixelsSquared) / 2.0;

            double pixelsPerFoot =
                sketch.Settings.PixelsPerFoot;

            return areaPixelsSquared /
                (pixelsPerFoot * pixelsPerFoot);
        }
    }
}
