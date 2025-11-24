using UnityEngine;

[System.Serializable]
public class Fish : IFish
{
    public string Name { get; set; }
    public float Weight { get; set; }
    public float Price { get; set; }
    public Sprite Icon { get; set; }
    
    public float MinWeight { get; set; }
    public float MaxWeight { get; set; }
    public float PricePerKg { get; set; }

    // For defining the "fish type" (like Salmon, Trout, etc.)
    public Fish(string name, float minWeight, float maxWeight, float pricePerKg, Sprite icon)
    {
        Name = name;
        MinWeight = minWeight;
        MaxWeight = maxWeight;
        PricePerKg = pricePerKg;
        Icon = icon;
    }

    // For when you actually "catch" one and generate instance data
    public Fish(Fish template, float actualWeight)
    {
        Name = template.Name;
        MinWeight = template.MinWeight;
        MaxWeight = template.MaxWeight;
        PricePerKg = template.PricePerKg;
        Icon = template.Icon;

        Weight = actualWeight;
        Price = Mathf.Round(actualWeight * PricePerKg);
    }

    public string GetDescription()
    {
        return $"{Name} ({Weight}kg, {Price}g)";
    }
}
