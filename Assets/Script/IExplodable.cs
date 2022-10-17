using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IExplodable
{
    void DealExplosionDamage(Vector2 f)
    {
        DealExplosionDamage(f, 1);
    }
    void DealExplosionDamage(Vector2 f, float damageMultiplier);
}
