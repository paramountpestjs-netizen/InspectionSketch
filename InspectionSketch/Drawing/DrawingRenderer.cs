using System.Windows;
using System.Windows.Media;

namespace InspectionSketch.Drawing
{
    public class DrawingRenderer
    {
        private readonly GridRenderer gridRenderer = new();

        public void Draw(DrawingContext drawingContext, Size size)
        {
            gridRenderer.Draw(drawingContext, size);
        }
    }
}
