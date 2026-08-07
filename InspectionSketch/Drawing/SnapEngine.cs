using System;
using System.Windows;

namespace InspectionSketch.Drawing
{
    public class SnapEngine
    {
        public double GridSpacing { get; set; } = 25;

        public Point SnapToGrid(Point point)
        {
            if (GridSpacing <= 0)
            {
                return point;
            }

            double snappedX = Math.Round(point.X / GridSpacing) * GridSpacing;
            double snappedY = Math.Round(point.Y / GridSpacing) * GridSpacing;

            return new Point(snappedX, snappedY);
        }
    }
}