using InspectionSketch.Models;
using System.Collections.Generic;
using System.Windows;

namespace InspectionSketch
{
    public class Room
    {
        public string Name { get; set; } = "Unnamed Room";

        public List<Wall> Walls { get; set; } = new List<Wall>();

        public double Area { get; set; }

        public Point LabelPosition { get; set; }
    }
}