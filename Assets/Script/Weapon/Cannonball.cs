using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : Exploder
{
    public override EWeaponType WeaponType => throw new System.NotImplementedException();

    protected override void Start() { base.Start(); NotifyOfLaunch(GetComponent<Rigidbody2D>().velocity); }
}
