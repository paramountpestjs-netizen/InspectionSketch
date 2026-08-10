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
        public void ZoomAt(Point focusPoint, double amount)
        {
            double oldZoom = Zoom;
            double newZoom = Zoom * amount;

            if (newZoom > 8)
                newZoom = 8;

            if (newZoom < .25)
                newZoom = .25;

            double zoomRatio = newZoom / oldZoom;

            Offset = new Point(
                focusPoint.X - (focusPoint.X - Offset.X) * zoomRatio,
                focusPoint.Y - (focusPoint.Y - Offset.Y) * zoomRatio);

            Zoom = newZoom;
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
        public void Reset()
        {
            Offset = new Point(0, 0);
            Zoom = 1.0;
        }
    }
}