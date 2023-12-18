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
        public double Hours { get; set; }
        public double HoursWithAll { get; set; }
        public double HoursWithBlind { get; set; }
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

        public (double? Weight, double? Hours) GetWeightAndHours(string nozzleSize, string flangeClass)
        {
            //return _weights.FirstOrDefault(n => n.NozzleSize == nozzleSize && n.FlangeClass == flangeClass)?.Weight;
            var data = _weights.FirstOrDefault(n => n.NozzleSize == nozzleSize && n.FlangeClass == flangeClass);
            return (data?.Weight, data?.Hours);

        }

        public (double? Weight, double? Hours) GetWeightAndHoursWithAll(string nozzleSize, string flangeClass)
        {
            //return _weights.FirstOrDefault(n => n.NozzleSize == nozzleSize && n.FlangeClass == flangeClass)?.WeightWithAll;
            var data = _weights.FirstOrDefault(n => n.NozzleSize == nozzleSize && n.FlangeClass == flangeClass);
            return (data?.WeightWithAll, data?.HoursWithAll);
        }

        public (double? Weight, double? Hours) GetWeightAndHoursWithBlind(string nozzleSize, string flangeClass)
        {
            //return _weights.FirstOrDefault(n => n.NozzleSize == nozzleSize && n.FlangeClass == flangeClass)?.WeightWithBlind;
            var data = _weights.FirstOrDefault(n => n.NozzleSize == nozzleSize && n.FlangeClass == flangeClass);
            return (data?.WeightWithBlind, data?.HoursWithBlind);
        }
    }
}
