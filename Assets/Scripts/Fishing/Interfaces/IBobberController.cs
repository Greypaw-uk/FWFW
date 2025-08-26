using UnityEngine;

public interface IBobberController
{
    void Launch(Vector3 start, Vector3 end, float duration, IPlayerFishing owner);
    void Despawn();
}

