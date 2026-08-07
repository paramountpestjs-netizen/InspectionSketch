using System.Windows;

namespace InspectionSketch.Drawing
{
    public class Camera
    {
        public Point Offset { get; private set; } = new Point(0, 0);

        public double Zoom { get; private set; } = 1.0;

        public void Pan(Vector delta)
        {
            Offset = new Point(
                Offset.X + delta.X,
                Offset.Y + delta.Y);
        }

        public void ZoomIn(double amount)
        {
            Zoom *= amount;

            if (Zoom > 8)
                Zoom = 8;
        }

        public void ZoomOut(double amount)
        {
            Zoom /= amount;

            if (Zoom < .25)
                Zoom = .25;
        }
    }
}