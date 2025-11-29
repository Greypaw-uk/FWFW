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

        public Item() { }

        // Constructor for basic item
        public Item(string name, float weight, float price, Sprite icon = null)
        {
            Name = name;
            Weight = weight;
            Price = price;
            Icon = icon;
        }
    }
}
