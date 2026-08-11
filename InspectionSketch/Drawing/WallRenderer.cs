using System.Windows;
using System.Windows.Media;

namespace InspectionSketch.Drawing
{
    public class WallRenderer
    {
        public void DrawWall(
     DrawingContext drawingContext,
     Point start,
     Point end)
        {
            Pen wallPen = new Pen(Brushes.Black, 2);

            drawingContext.DrawLine(
                wallPen,
                start,
                end);
    }
    }
}