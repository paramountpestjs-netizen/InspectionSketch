using System.Windows;
using System.Windows.Media;

namespace InspectionSketch.Drawing
{
    public class DrawingRenderer
    {
        private readonly GridRenderer gridRenderer = new();
        public const double DefaultGridSpacing = 25;

        public double GridSpacing { get; set; } = DefaultGridSpacing;
        public void Draw(DrawingContext drawingContext, Size size)
        {
            gridRenderer.GridSpacing = GridSpacing;

            gridRenderer.Draw(drawingContext, size);
        }
    }
}
