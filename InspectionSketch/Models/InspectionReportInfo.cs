using System;

namespace InspectionSketch.Models
{
    public class InspectionReportInfo
    {
        public DateTime InspectionDate { get; set; } =
            DateTime.Today;

        public string CustomerName { get; set; } = "";

        public string PropertyAddress { get; set; } = "";

        public string InspectorName { get; set; } = "";

        public string Notes { get; set; } = "";
    }
}