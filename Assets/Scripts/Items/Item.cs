using UnityEngine;

public class Items : MonoBehaviour
{
    [System.Serializable]
    public class Item
    {
        public string Name;
        public float Weight;
        public float Price;
        public Sprite Icon;


        // Fish
        public float MinWeight;
        public float MaxWeight;
        public float PricePerKg;

        public Item() { }

        // Constructor for basic item
        public Item(string name, float weight, float price, Sprite icon = null)
        {
            Name = name;
            Weight = weight;
            Price = price;
            Icon = icon;
        }

        // Constructor for fish item
        public Item(string name, float minWeight, float maxWeight, float pricePerKg, Sprite icon = null)
        {
            Name = name;
            MinWeight = minWeight;
            MaxWeight = maxWeight;
            PricePerKg = pricePerKg;
            Icon = icon;
        }
    }
}
