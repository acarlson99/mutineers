using UnityEngine;

public class SeagullExplosive : Exploder
{
    public override string weaponName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public override EWeaponType weaponType { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        NotifyOfLaunch(Vector2.zero);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        DestroyIfOOB();
    }

    public bool DestroyIfOOB()
    {
        if (transform.position.y < Singleton.Instance.killzoneY)
        {
            Destroy(gameObject);
        }
        return false;
    }
}
