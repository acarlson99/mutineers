using UnityEngine;

public class SeagullExplosive : Exploder
{
    public override EWeaponType WeaponType { get => throw new System.NotImplementedException(); }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        NotifyOfLaunch(Vector2.zero);
    }
}
