using UnityEngine;

public interface ICombat
{
    void TakeDamage(int amount);
    Transform GetTransform(); 
}

