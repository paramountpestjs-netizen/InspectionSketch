namespace InspectionSketch.Models
{
    public class DrawingSettings
    {
        // Number of pixels that represent one foot
        public double PixelsPerFoot { get; set; } = 25;

        // Size of each grid square in pixels
        // 25 pixels = 1 foot at the current drawing scale.
        public double GridSpacing { get; set; } = 25;
    }
}
