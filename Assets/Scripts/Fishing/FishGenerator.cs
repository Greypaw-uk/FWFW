using System.Collections.Generic;
using UnityEngine;

public static class FishGenerator
{
    private static List<Fish> availableFish = new()
    {
        new Fish("Salmon", 1.0f, 5.0f, 10f, Resources.Load<Sprite>("Icons/Salmon")),
        new Fish("Tuna", 2.0f, 10.0f, 8f, Resources.Load<Sprite>("Icons/Tuna")),
        new Fish("Trout", 0.5f, 3.0f, 12f, Resources.Load<Sprite>("Icons/Trout")),
        new Fish("Catfish", 1.0f, 6.0f, 7f, Resources.Load<Sprite>("Icons/Catfish"))
    };

    public static IItem GenerateRandomFish()
    {
        Fish baseFish = availableFish[Random.Range(0, availableFish.Count)];
        float weight = Mathf.Round(Random.Range(baseFish.MinWeight, baseFish.MaxWeight) * 10f) / 10f;

        return new Fish(baseFish, weight);
    }
}
