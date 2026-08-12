using System.IO;
using System.Text.Json;

namespace InspectionSketch.Services
{
    public class SketchFileService
    {
        private readonly JsonSerializerOptions options =
            new JsonSerializerOptions
            {
                WriteIndented = true
            };

        public void Save(Sketch sketch, string filePath)
        {
            string json = JsonSerializer.Serialize(
                sketch,
                options);

            File.WriteAllText(
                filePath,
                json);
        }

        public Sketch? Load(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            string json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<Sketch>(
                json,
                options);
        }
    }
}
