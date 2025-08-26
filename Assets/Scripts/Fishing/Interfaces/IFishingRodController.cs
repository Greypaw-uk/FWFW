using UnityEngine;

public interface IFishingRodController
{
    void ShowRod(ulong ownerClientId);
    void HideRod();
    void Flick();
    void Initialize(Transform followTarget);
}

