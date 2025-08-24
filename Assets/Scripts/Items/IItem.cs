using UnityEngine;

public interface IItem
{
    string Name { get; set; }
    float Weight { get; set; }
    float Price { get; set; }
    Sprite Icon { get; set; }

    string GetDescription();
}
