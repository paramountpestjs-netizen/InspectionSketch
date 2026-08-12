using System;

namespace InspectionSketch.Models
{
    public class InspectionReportInfo
    {
        public string PropertyAddress { get; set; } = "";

        public string CustomerName { get; set; } = "";

        public string InspectorName { get; set; } = "";

        public DateTime InspectionDate { get; set; } =
            DateTime.Today;

        public string JobNumber { get; set; } = "";

        public string Notes { get; set; } = "";
    }
}
