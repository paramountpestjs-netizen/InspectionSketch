using System.Windows;
using System.Windows.Media;

namespace InspectionSketch.Drawing
{
    public class GridRenderer
    {
        public double GridSpacing { get; set; } = 25;
        public double Zoom { get; set; } = 1.0;

        public void Draw(DrawingContext dc, Size size)
        {
            if (GridSpacing <= 0)
                return;

            double scaledSpacing = GridSpacing * Zoom;

            Pen pen = new Pen(
                new SolidColorBrush(Color.FromRgb(65, 65, 65)),
                0.5);

            for (double x = 0; x <= size.Width; x += scaledSpacing)
            {
                dc.DrawLine(
                    pen,
                    new Point(x, 0),
                    new Point(x, size.Height));
            }

            for (double y = 0; y <= size.Height; y += scaledSpacing)
            {
                dc.DrawLine(
                    pen,
                    new Point(0, y),
                    new Point(size.Width, y));
            }
        }
    }
}