using Unity.VisualScripting;
using UnityEngine;

public class Crate : Weapon, IExplodable
{
    public override string weaponName { get; set; } = "crate";

    public float slowMagnitude = 0.1f;
    public float timeToSlow = 3;

    private float slowTime = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        // FIXME: cleanup, this is janky
        if (Input.GetMouseButtonDown((int)MouseButton.Left) && !thrown)
        {
            NotifyOfLaunch(Vector2.zero);
            var v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            v = new Vector3(v.x, v.y, transform.position.z);
            transform.position = v;

            NotifyThrowerEndWeaponUse();
            GetComponent<Collider2D>().isTrigger = false;
            enabled = false;
            return;
        }
        if (thrown && rb.IsMovingSlowly(slowMagnitude))
            slowTime += Time.deltaTime;
        else slowTime = 0;
        if (slowTime >= timeToSlow)
        {
            NotifyThrowerEndWeaponUse();
            enabled = false;
            return;
        }
    }

    public override void NotifyOfLaunch(Vector2 velocity)
    {
        base.NotifyOfLaunch(velocity);
        slowTime = 0;
    }

    public void DealExplosionDamage(Vector2 f)
    {
        Destroy(gameObject);
    }

    // TODO: OnExplode function
}
