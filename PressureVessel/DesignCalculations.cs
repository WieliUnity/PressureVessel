using Newtonsoft.Json;
using System;
using System.IO;

namespace PressureVessel
{
    public class DesignCalculations
    {
        private dynamic materialData;

        public DesignCalculations()
        {
            // Load the material data when an instance of the class is created
            LoadMaterialData();
        }

        private void LoadMaterialData()
        {
            // Adjust the file path as needed
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "material_data.json";
            string jsonContent = File.ReadAllText(filePath);
            materialData = JsonConvert.DeserializeObject<dynamic>(jsonContent);
        }

        public double FindFbValue(string material, int temperature)
        {
            var materialInfo = materialData[material];
            foreach (var data in materialInfo)
            {
                if (data.Temp == temperature)
                {
                    return data.fb;
                }
            }
            // Handle the case where no matching temperature is found
            return -1;
        }

        public double CalculateMinThickness(double pressure, double diameter, double fbValue)
        {
            return ((pressure / 10) * diameter) / (2 * fbValue * 0.85 + (pressure / 10));
        }

        public double CalculateMinThicknessIncSafety(double minThickness, double corrosionAllowance)
        {
            return minThickness + 0.3 + corrosionAllowance;
        }

        public int CalculateFinalThickness(double minThicknessIncSafety, double extraThickness)
        {
            return (int)Math.Ceiling(minThicknessIncSafety + extraThickness);
        }

        public double CalculateCurrentUsage(double minThicknessIncSafety, double finalThickness)
        {
            return minThicknessIncSafety / finalThickness;
        }
    }
}
