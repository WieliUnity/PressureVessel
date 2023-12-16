using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressureVessel
{
    public class Result
    {
        public string SheetSize { get; set; }
        public double TotalCost { get; set; }
        public int SheetsNeeded { get; set; }
        public double TotalWeldLength { get; set; }
        public double BendingHours { get; set; }
        public double BendingCost { get; set; }
        public double BevelingHours { get; set; }
        public double BevelingCost { get; set; }
        public double MaterialCost { get; set; }
        public double WeldHours { get; set; }
        public double BuildHours { get; set;}
        public double TotalHours { get; set; }  

        // ... other properties ...
    }


}
