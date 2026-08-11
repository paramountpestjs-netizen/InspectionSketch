using System.Windows;
using System.Windows.Media;

namespace InspectionSketch.Drawing
{
    public class PreviewRenderer
    {
        public void Draw(
            DrawingContext drawingContext,
            Point start,
            Point end)
        {
            Pen previewPen = new Pen(Brushes.RoyalBlue, 2);

            drawingContext.DrawLine(
                previewPen,
                start,
                end);
        }
    }
}
