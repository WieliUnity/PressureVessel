using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PressureVessel
{
    public class ControlCalculations
    {
        public double CalculateRitningarCost(double numberOfNozzles)
        {
            const double salary = 1000.0;
            double totalCost = 0;
            double totalHours = 0;

            totalHours += numberOfNozzles * 1; //timmar för nozzlar
               


            totalCost = totalHours * salary;


            
            return totalCost;
        }

        public double CalculateOFPcost()
        {
            // Your calculation logic here
            return 0.0;
        }

        public CostCalculationResult CalculateProvtryckningCost(double numberOfNozzles, double volume, double costPerHour)
        {
            const double waterPrice = 0.5;
            CostCalculationResult result = new CostCalculationResult();

            result.TotalBuildHours = numberOfNozzles * 1; // Assuming 1 hour per nozzle
            result.TotalBuildHours += 2; // Assuming 1 hour for the vessel itself
            double waterCost = volume * waterPrice; // Assuming volume is in liters

            result.TotalCost = (result.TotalBuildHours * costPerHour) + waterCost;

            return result;
        }

        public double CalculateBetningCost()
        {
            // Your calculation logic here
            return 0.0;
        }

        public double CalculateMålningCost()
        {
            // Your calculation logic here
            return 0.0;
        }

        public double CalculateBeräkningarCost()
        {
            // Your calculation logic here
            return 15000;
        }

        public double CalculateThirdPartyCost()
        {
            // Your calculation logic here
            return 15000;
        }





        public class CostCalculationResult
        {
            public double TotalCost { get; set; }
            public double TotalBuildHours { get; set; }
        }
    }

}
