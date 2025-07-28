using System.Collections.Generic;
using UnityEngine;

public static class FishGenerator
{
    private static List<Items.Item> availableFish = new()
    {
        new Items.Item { Name = "Salmon", MinWeight = 1.0f, MaxWeight = 5.0f, PricePerKg = 10f},
        new Items.Item { Name = "Tuna", MinWeight = 2.0f, MaxWeight = 10.0f, PricePerKg = 8f },
        new Items.Item { Name = "Trout", MinWeight = 0.5f, MaxWeight = 3.0f, PricePerKg = 12f },
        new Items.Item { Name = "Catfish", MinWeight = 1.0f, MaxWeight = 6.0f, PricePerKg = 7f }
    };

    public static Items.Item GenerateRandomFish()
    {
        var fish = new Items.Item();
        Items.Item selectedFish = availableFish[Random.Range(0, availableFish.Count)];

        fish.Name = selectedFish.Name;
        fish.Weight = Mathf.Round(Random.Range(selectedFish.MinWeight, selectedFish.MaxWeight) * 10f) / 10f;
        fish.Price = Mathf.Round(fish.Weight * selectedFish.PricePerKg);

        return fish;
    }
}