using System;
using System.Collections.Generic;

namespace PressureVessel
{
    public class PressureVesselCalculation
    {
        public double VesselHeight { get; set; }
        public double VesselDiameter { get; set; }
        public double Thickness { get; set; }
        public double CostPerKg { get; set; }
        public double WeldCostPerHour { get; set; }
        public double WeldTimePerMeter => CalculateWeldTimePerMeter(Thickness);

        private const double Density = 8000 / 1e9;  // kg/mm^3

        private List<(int, int)> sheetFormats = new List<(int, int)>
        {
            (1000, 2000), (1250, 2500), (1500, 3000), (2000, 4000), (2000, 6000),
            (2000, 1000), (2500, 1250), (3000, 1500), (4000, 2000), (6000, 2000)
        };

        private double CalculateWeldTimePerMeter(double thickness)
        {
            return (thickness / 3.0) * 0.8;
        }

        public List<Result> CalculateCosts()
        {
            var results = new List<Result>();
            double vesselCircumference = Math.PI * VesselDiameter;

            foreach (var (width, height) in sheetFormats)
            {
                var (totalCost, sheetsNeeded, weldLength, bendingHours, bendingCost, bevelingHours, bevelingCost, materialCost, weldHours, buildHours,totalHours) = CalculateCost(width, height, vesselCircumference);
                results.Add(new Result
                {
                    SheetSize = $"{width}x{height}",
                    TotalCost = Math.Round(totalCost, 1),
                    SheetsNeeded = sheetsNeeded,
                    TotalWeldLength = Math.Round(weldLength, 1),
                    BendingHours = Math.Round(bendingHours, 1),
                    BendingCost = Math.Round(bendingCost, 1),
                    BevelingHours = Math.Round(bevelingHours, 1),
                    BevelingCost = Math.Round(bevelingCost, 1),
                    MaterialCost = Math.Round(materialCost, 1),
                    WeldHours = Math.Round(weldHours, 1),
                    BuildHours = Math.Round(buildHours, 1),
                    TotalHours = Math.Round(totalHours, 1)
                });
            }

            return results;
        }

        private (double totalCost, int sheetsNeeded, double weldLength, double bendingHours, double bendingCost, double bevelingHours, double bevelingCost, double materialCost, double weldHours, double buildingHours, double totalHours) CalculateCost(int sheetWidth, int sheetHeight, double vesselCircumference)
        {
            int totalSheetsNeeded = (int)Math.Ceiling(vesselCircumference / sheetWidth);
            double totalWeldLength = 0;
            double totalBendingHours = 0;

            for (int i = 0; i < totalSheetsNeeded; i++)
            {
                totalBendingHours += CalculateBendingTime(sheetWidth);
                if (i < totalSheetsNeeded - 1)
                {
                    totalWeldLength += vesselCircumference / 1000;
                }
            }
            double excessMaterial = 0;
            double excessMaterialWidth = vesselCircumference % sheetWidth;
            if (excessMaterialWidth > 0)
            {
                totalSheetsNeeded++;
                totalBendingHours += CalculateBendingTime(sheetWidth);
                totalWeldLength += vesselCircumference / 1000;
            }
            excessMaterial += excessMaterialWidth;

            double dishedEndsWeldLength = 2 * (vesselCircumference / 1000);
            totalWeldLength += dishedEndsWeldLength;

            double totalAreaSheets = totalSheetsNeeded * sheetWidth * sheetHeight;
            double weightSheets = totalAreaSheets * Thickness * Density;
            double materialCost = weightSheets * CostPerKg;
            double weldHours = totalWeldLength * WeldTimePerMeter;
            double weldCost = totalWeldLength * WeldCostPerHour * WeldTimePerMeter;
            double bendingCost = totalBendingHours * WeldCostPerHour;
            double bevelCost = totalWeldLength * 0.3 * WeldCostPerHour; // Assuming this is the beveling cost
            double excessMaterialWeight = excessMaterial * sheetHeight * Thickness * Density;
            double excessMaterialValue = excessMaterialWeight * (CostPerKg * 0.2);

            // Assuming you have a formula for beveling hours and cost
            double bevelingHours = totalWeldLength * 0.3; // This is an example, replace with your actual formula
            double bevelingCost = bevelingHours * WeldCostPerHour;

            double totalCost = materialCost + weldCost + bendingCost + bevelCost - excessMaterialValue;
            double buildingHours = bevelingHours + totalBendingHours;
            double totalHours = weldHours + buildingHours;

            return (totalCost, totalSheetsNeeded, totalWeldLength, totalBendingHours, bendingCost, bevelingHours, bevelingCost, materialCost, weldHours, buildingHours,totalHours);
        }

        private double CalculateBendingTime(int plateWidth)
        {
            const double startHourPerPlate = 1.0; // Start hour for prebending per plate
            double additionalBendingTime = 0.7 + (plateWidth / 1000 - 1) * 0.3; // Additional time based on plate width
            return startHourPerPlate + additionalBendingTime;
        }

    }
}
