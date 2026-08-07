using System.Windows;
using System.Windows.Media;

namespace InspectionSketch.Drawing
{
    public class DrawingRenderer
    {
        private readonly GridRenderer gridRenderer = new();
        public const double DefaultGridSpacing = 25;

        public double GridSpacing { get; set; } = DefaultGridSpacing;
        public double Zoom { get; set; } = 1.0;
      
        public void Draw(DrawingContext drawingContext, Size size)
        {
            gridRenderer.GridSpacing = GridSpacing;           
            gridRenderer.Zoom = Zoom;
            gridRenderer.Draw(drawingContext, size);
        }
    }
}
