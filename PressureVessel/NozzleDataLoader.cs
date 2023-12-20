using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Converters;

namespace PressureVessel
{
    public class NozzleData
    {
        public string NozzleSize { get; set; }
        public string FlangeClass { get; set; }
        public double Weight { get; set; }
        public double WeightWithAll { get; set; }
        public double WeightWithBlind { get; set; }
        public double BuildHours { get; set; }
        public double BuildHoursWithAll { get; set; }
        public double BuildHoursWithBlind { get; set; }
        public double WeldHours { get; set; }
        public double WeldHoursWithAll { get; set; }
        public double WeldHoursWithBlind { get; set; }
        public int NumberBolts { get; set; }
        public string SizeBolts { get; set; }
    }

    public class NozzleDataLoader
    {
        private List<NozzleData> _weights;

        public NozzleDataLoader()
        {
            LoadData();
        }

        private void LoadData()
        {
            string json = File.ReadAllText("nozzleData.json");
            _weights = JsonConvert.DeserializeObject<List<NozzleData>>(json);
            
        }

        public (double? Weight, double? WeldHours, double? BuildHours, int? NumberBolts, string? SizeBolts) GetWeightAndHours(string nozzleSize, string flangeClass)
        {
            //return _weights.FirstOrDefault(n => n.NozzleSize == nozzleSize && n.FlangeClass == flangeClass)?.Weight;
            var data = _weights.FirstOrDefault(n => n.NozzleSize == nozzleSize && n.FlangeClass == flangeClass);
            return (data?.Weight, data?.WeldHours, data?.BuildHours, data?.NumberBolts, data?.SizeBolts);

        }

        public (double? WeightWithAll, double? WeldHoursWithAll, double? BuildHoursWithAll, int? NumberBolts, string? SizeBolts) GetWeightAndHoursWithAll(string nozzleSize, string flangeClass)
        {
            //return _weights.FirstOrDefault(n => n.NozzleSize == nozzleSize && n.FlangeClass == flangeClass)?.WeightWithAll;
            var data = _weights.FirstOrDefault(n => n.NozzleSize == nozzleSize && n.FlangeClass == flangeClass);
            return (data?.WeightWithAll, data?.WeldHoursWithAll, data?.BuildHoursWithAll, data?.NumberBolts, data?.SizeBolts);
        }

        public (double? WeightWithBlind, double? WeldHoursWithBlind, double? BuildHoursWithBlind, int? NumberBolts, string? SizeBolts) GetWeightAndHoursWithBlind(string nozzleSize, string flangeClass)
        {
            //return _weights.FirstOrDefault(n => n.NozzleSize == nozzleSize && n.FlangeClass == flangeClass)?.WeightWithBlind;
            var data = _weights.FirstOrDefault(n => n.NozzleSize == nozzleSize && n.FlangeClass == flangeClass);
            return (data?.WeightWithBlind, data?.WeldHoursWithBlind, data?.BuildHoursWithBlind, data?.NumberBolts, data?.SizeBolts);
        }
    }
}
