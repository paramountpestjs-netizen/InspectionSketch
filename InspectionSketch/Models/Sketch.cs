using InspectionSketch.Models;
using System.Collections.Generic;

namespace InspectionSketch
{
    public class Sketch
    {
        public List<Wall> Walls { get; set; } = new List<Wall>();
        public List<Room> Rooms { get; set; } = new List<Room>();
        public DrawingSettings Settings { get; set; } = new DrawingSettings();
    }
}
