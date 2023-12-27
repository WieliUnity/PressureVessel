using System;
using System.Windows;

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
        
        return new DishedEndResult { Volume = volume, Weight = weight, Price = price };
    }

    private DishedEndResult CalculateDinEnd(double diameter, double thickness, double price)
    {
        // Placeholder for SMS482 and DIN28011 calculation logic
        double rondelDia = CalculateRondellDia(diameter, thickness);
        double volume = CalculateVolume(diameter, thickness);
        double weight = CalculateWeight(diameter, rondelDia);
        return new DishedEndResult { Volume = volume, Weight = weight, Price = price };
    }

    private DishedEndResult CalculateConeEnd(double diameter, double thickness, double price)
    {
        // Placeholder for Cone calculation logic
        return new DishedEndResult();
    }

    private DishedEndResult CalculateFlatEnd(double diameter, double thickness, double price)
    {
        // Placeholder for Flat calculation logic
        return new DishedEndResult();
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
}

public class DishedEndResult
{
    public double Volume { get; set; }
    public double Weight { get; set; }
    public double Price { get; set; }
}
