using UnityEngine;
using UnityEngine.UI;

public class Items : MonoBehaviour
{
    [System.Serializable]
    public class Item
    {
        public string Name;
        public float Weight;
        public float Price;
        public Image Icon;


        // Fish
        public float MinWeight;
        public float MaxWeight;
        public float PricePerKg;
    }
}
