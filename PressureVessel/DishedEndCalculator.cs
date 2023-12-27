using System;
using System.Windows;
using System.Windows.Media.Media3D;

public class DishedEndCalculator
{
    public DishedEndResult Calculate(string selection, double diameter, double thickness, double price)
    {
        switch (selection)
        {
            case "SMS482":
                return CalculateSMSEnd(diameter, thickness, price);

            case "DIN28011":
                return CalculateDinEnd(diameter, thickness, price);

            case "Cone":
                // Placeholder for Cone calculation
                return CalculateConeEnd(diameter, thickness, price);

            case "Flat":
                // Placeholder for Flat calculation
                return CalculateFlatEnd(diameter, thickness, price);

            default:
                throw new ArgumentException("Invalid selection");
        }
    }

    private DishedEndResult CalculateSMSEnd(double diameter, double thickness, double price)
    {
        // Placeholder for SMS482 and DIN28011 calculation logic
        double rondelDia = CalculateRondellDia(diameter, thickness);
        double weight = CalculateWeight(rondelDia, thickness);
        double volume = CalculateVolume(diameter, thickness);
        double weldHours = 0;
        double buildHours = 0;
        
        return new DishedEndResult { Volume = volume, Weight = weight, Price = price, WeldHours = weldHours, BuildHours = buildHours };
    }

    private DishedEndResult CalculateDinEnd(double diameter, double thickness, double price)
    {
        // Placeholder for SMS482 and DIN28011 calculation logic
        double rondelDia = CalculateRondellDia(diameter, thickness);
        double volume = CalculateVolume(diameter, thickness);
        double weight = CalculateWeight(diameter, rondelDia);
        double weldHours = 0;
        double buildHours = 0;
        return new DishedEndResult { Volume = volume, Weight = weight, Price = price, WeldHours = weldHours, BuildHours = buildHours };
    }

    private DishedEndResult CalculateConeEnd(double diameter, double thickness, double price)
    {
        double diameter1 = diameter + (50 * 2);
        double radius = diameter1 / 2;


        double coneHeight = CalculateConeHeight(diameter1, thickness);
        double coneArea = CalculateConeArea(coneHeight,diameter1);

        double coneWeight = coneArea * thickness * 8;
        double coneVolume = (1.0 / 3) * Math.PI * Math.Pow((radius/1000), 2) * coneHeight;

        (double coneEndWeldHours, double coneEndBuildHours) = CalculateConeEndHours(radius, thickness, coneHeight);


        return new DishedEndResult { Volume=coneVolume, Weight = coneWeight, Price = price, WeldHours = coneEndWeldHours, BuildHours = coneEndBuildHours };
    }

    private DishedEndResult CalculateFlatEnd(double diameter, double thickness, double price)
    {

        double rondelRad = ((diameter + (50 * 2))/2);
        double volume = 0;
        double weight = (rondelRad * rondelRad * Math.PI * thickness * 8)/1000000;
        // Placeholder for Flat calculation logic
        (double flatEndWeldHours, double flatEndBuildHours) = CalculateFlatEndHours(rondelRad, thickness);
        
        return new DishedEndResult { Volume = volume, Weight = weight, Price = price, WeldHours = flatEndWeldHours, BuildHours = flatEndBuildHours };
    }



    // Placeholder methods for volume and weight calculations
    private double CalculateVolume(double diameter, double thickness)
    {
        double radius = diameter / 2;
        double height = (0.255 * (diameter -(thickness*2)));
        double volumeSphere = 4/3 * Math.PI * radius * radius * height;
        double littleHeight = 3.5 * thickness;
        double volumeLittleHeight = (radius * radius * Math.PI) * littleHeight;
        double finalVolume = (volumeSphere + volumeLittleHeight)/ 1000000;
        
        return finalVolume;
    }

    private double CalculateWeight(double rondelDia, double thickness)
    {
        double radiusRondel = rondelDia / 2;
        double area = (radiusRondel * radiusRondel * Math.PI);
        double areainM2 = area / 1000000;
        double finalWeight = areainM2 * thickness * 8;
        return finalWeight;
    }

    private double CalculateRondellDia(double diameter, double thickness)
    {
        double cylindricHeight = thickness * 3.5; //Rakbiten
        double extraWidth = diameter * 0.02; //Extra bearbetningsmån
        double rondellDia = (1.177 * (diameter - thickness)) + (1.6 * cylindricHeight) + extraWidth;
        return rondellDia;
    }

    private double CalculateConeHeight(double diameter, double thickness)
    {
        double angleDegrees = 15; // Angle in degrees

        // Convert angle from degrees to radians
        double angleRadians = (Math.PI / 180) * angleDegrees;

        // Calculate the height using the formula: height = radius * tan(angle)
        double radius = diameter / 2;
        double height = radius * Math.Tan(angleRadians);
        return height;
    }

    private double CalculateConeArea(double height, double diameter)
    {
        double radius = diameter / 2;
        double slantHeight = Math.Sqrt(Math.Pow(radius, 2) + Math.Pow(height, 2));

        double lateralArea = (Math.PI * radius * slantHeight) / 1000000;

        return lateralArea;
        
    }

    private (double weldHours, double buildHours) CalculateFlatEndHours(double radius, double thickness)
    {
        double flatArea = (radius * radius * Math.PI) / 1000000;
        double amountPlates = Math.Ceiling(flatArea / 4.5);
        double weldMeters = (amountPlates * 3.9) - 3.8;
        double weldPerMeter = (thickness / 4.0) * 0.8;
        double weldHours = weldMeters * weldPerMeter;
        double bevelsHoursPerMeter = (thickness / 4.0) * 0.2;
        double bevelHours = weldMeters * bevelsHoursPerMeter;
        double cuttingMeters = ((radius/1000)/2) *Math.PI;
        double cuttingMetersPerHour = (thickness / 4.0) * 0.4;
        double cuttingHours = cuttingMeters *cuttingMetersPerHour;
        double buildingHours = weldMeters * 0.4;

        double buildHours = bevelHours + buildingHours + cuttingHours;

        return (weldHours, buildHours);

    }

    private (double weldHours, double buildHours) CalculateConeEndHours(double radius, double thickness, double coneHeight)
    {
        double slantHeight = Math.Sqrt(Math.Pow(radius, 2) + Math.Pow(coneHeight, 2));


        double flatArea = (slantHeight * slantHeight * Math.PI)/1000000;
        double amountPlates = Math.Ceiling(flatArea / 4.5);
        double weldMeters = ((amountPlates * 3.9) - 3.8) + (slantHeight/1000);
        double weldPerMeter = (thickness / 4.0) * 0.8;
        double weldHours = weldMeters * weldPerMeter;
        double bevelsHoursPerMeter = (thickness / 4.0) * 0.2;
        double bevelHours = weldMeters * bevelsHoursPerMeter;
        double buildingHours = weldMeters * 0.4;
        double cuttingMeters = ((radius / 1000) / 2) * Math.PI;
        double cuttingMetersPerHour = (thickness / 4.0) * 0.4;
        double cuttingHours = cuttingMeters * cuttingMetersPerHour;

        double buildHours = bevelHours + buildingHours + cuttingHours;

        return (weldHours, buildHours);

    }



}

    
public class DishedEndResult
{
    public double Volume { get; set; }
    public double Weight { get; set; }
    public double Price { get; set; }
    public double WeldHours { get; set; }
    public double BuildHours { get; set; }
}
