using System.Windows;
using System.Windows.Media;

namespace InspectionSketch.Drawing
{
    public class WallRenderer
    {
        public void DrawWall(
           DrawingContext drawingContext,
           Point start,
           Point end,
           bool isSelected)
        {
            Pen wallPen = new Pen(
                isSelected ? Brushes.Red : Brushes.Black,
                2);

            drawingContext.DrawLine(
                wallPen,
                start,
                end);
        }
    }
}