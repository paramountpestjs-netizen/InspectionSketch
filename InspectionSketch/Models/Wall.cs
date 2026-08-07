using System.Windows;

namespace InspectionSketch.Models
{
    public class Wall
    {
        public Point StartPoint { get; set; }

        public Point EndPoint { get; set; }

        public double Length
        {
            get
            {
                return (StartPoint - EndPoint).Length;
            }
        }

        public string Material { get; set; } = "Unknown";

        public string Notes { get; set; } = "";
    }
}