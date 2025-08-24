using UnityEngine;

public interface IFish : IItem
{
    float MinWeight { get; set; }
    float MaxWeight { get; set; }
    float PricePerKg { get; set; }
}