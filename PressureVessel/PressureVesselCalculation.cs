using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
        public double BevelingTimePerMeter => CalculateBevelingTimePerMeter(Thickness);

        private const double Density = 8000 / 1e9;  // kg/mm^3
        public int connections = 0;

        private List<(int, int)> sheetFormats = new List<(int, int)>
        {
            (6000, 2000), (2500, 1250), (3000, 1500), (4000, 2000), (6000, 1500), (6000, 2000), (6000, 2500)
        };

        private double CalculateWeldTimePerMeter(double thickness)
        {
            return (thickness / 4.0) * 0.8;
        }

        private double CalculateBevelingTimePerMeter(double thickness)
        {
            return (Thickness / 4.0) * 0.2;
        }

        public List<Result> CalculateCosts()
        {
            var results = new List<Result>();
            double vesselCircumference = Math.PI * VesselDiameter;

            foreach (var (width, height) in sheetFormats)
            {
                int parts = (int)Math.Ceiling(VesselHeight / height) + 2; // +2 for dished ends
                connections = parts - 1; // 1 less connection than parts
                double connectionTime = CalculateConnectionTime(vesselCircumference, connections);

                var (totalCost, sheetsNeeded, weldLength, bendingHours, bendingCost, bevelingHours, bevelingCost, materialCost, weldHours, buildHours,totalHours) = CalculateCost(width, height, vesselCircumference, connectionTime);
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
                    TotalHours = Math.Round(totalHours, 1),
                    ConnectionTime = Math.Round(connectionTime, 1)
                });
            }

            return results;
        }

        private double CalculateConnectionTime(double circumference, int connections)
        {
            double circumferenceInMeters = circumference / 1000;
            return (0.6 + (circumferenceInMeters * 0.4)) * connections;
        }

        private (double totalCost, int sheetsNeeded, double weldLength, double bendingHours, double bendingCost, double bevelingHours, double bevelingCost, double materialCost, double weldHours, double buildingHours, double totalHours) CalculateCost(int sheetWidth, int sheetHeight, double vesselCircumference, double connectionTime)
        {

           
            int totalSheetsNeeded = 0;
            double totalWeldLength = 0;
            double totalBendingHours = 0;
            double remainingHeight = VesselHeight;
            bool excessMaterial = false;
            double neededMaterialWidth = 0;
            double excessMaterialWidth = 0;
            
            double divisionResult = vesselCircumference / sheetWidth;
            double decimalPart = divisionResult - Math.Floor(divisionResult);
            int totalSheetsNeededPerCylinder;

            if (decimalPart > 0.5)
            {
                totalSheetsNeededPerCylinder = (int)Math.Ceiling(divisionResult); // Round up
            }
            else
            {
                totalSheetsNeededPerCylinder = (int)Math.Floor(divisionResult); // Round down
            }
            totalSheetsNeededPerCylinder = totalSheetsNeededPerCylinder == 0 ? 1 : totalSheetsNeededPerCylinder;

            while (remainingHeight > 0)
            {
                for (int i = 0; i < totalSheetsNeededPerCylinder; i++)
                {
                    totalBendingHours += CalculateBendingTime(sheetWidth);
                    totalWeldLength += sheetHeight / 1000;
                    totalSheetsNeeded++;
                }
                

                if ((excessMaterialWidth > neededMaterialWidth) && (excessMaterial == true))
                {
                    totalBendingHours += CalculateBendingTime(neededMaterialWidth);
                    totalWeldLength += sheetHeight / 1000;
                    excessMaterialWidth -= neededMaterialWidth;
                    
                    if (excessMaterialWidth > neededMaterialWidth)
                    {
                        excessMaterial = true;
                    }
                    else
                    {
                        excessMaterial = false;
                    }
                    
                }
                
                else if((totalSheetsNeededPerCylinder*sheetWidth < vesselCircumference) && (excessMaterial == false))
                {
                    neededMaterialWidth = vesselCircumference - (sheetWidth * totalSheetsNeededPerCylinder);
                    excessMaterialWidth = sheetWidth - neededMaterialWidth;
                    totalSheetsNeeded++;
                    totalBendingHours += CalculateBendingTime(neededMaterialWidth);
                    totalWeldLength += sheetHeight / 1000;
                    excessMaterial = true;
                }
                
               

                remainingHeight -= sheetHeight;
            }


            double circularWeldMeters = connections * vesselCircumference/1000;
            totalWeldLength += circularWeldMeters;
            double connectionCost = connectionTime * WeldCostPerHour;
            double totalAreaSheets = totalSheetsNeeded * sheetWidth * sheetHeight;
            double weightSheets = totalAreaSheets * Thickness * Density;
            double materialCost = weightSheets * CostPerKg;
            
            double weldHours = totalWeldLength * WeldTimePerMeter;
            double weldCost = totalWeldLength * WeldCostPerHour * WeldTimePerMeter;
            double bendingCost = totalBendingHours * WeldCostPerHour;
            double bevelCost = totalWeldLength * 0.3 * WeldCostPerHour; // Assuming this is the beveling cost


            // Assuming you have a formula for beveling hours and cost
            double bevelingHours = BevelingTimePerMeter * totalWeldLength; // This is an example, replace with your actual formula
            double bevelingCost = bevelingHours * WeldCostPerHour;

            double totalCost = materialCost + weldCost + bendingCost + bevelCost + connectionCost;
            double buildingHours = bevelingHours + totalBendingHours + connectionTime;
            double totalHours = weldHours + buildingHours + connectionTime;

            return (totalCost, totalSheetsNeeded, totalWeldLength, totalBendingHours, bendingCost, bevelingHours, bevelingCost, materialCost, weldHours, buildingHours,totalHours);
        }

        private double CalculateBendingTime(double plateWidth)
        {
            const double startHourPerPlate = 1.0; // Start hour for prebending per plate
            double additionalBendingTime = 0.7 + (plateWidth / 1000 - 1) * 0.3; // Additional time based on plate width
            return startHourPerPlate + additionalBendingTime;
        }



    }
}
