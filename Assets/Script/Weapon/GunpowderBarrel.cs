using Unity.VisualScripting;
using UnityEngine;

public class GunpowderBarrel : Exploder, IExplodable
{
    public override string weaponName { get; set; } = "gunpowder barrel";

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
            rb.velocity = Vector2.zero;

            NotifyThrowerEndWeaponUse();
            GetComponent<Collider2D>().isTrigger = false;
            thrown = true;
            return;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision) { }

    public override void NotifyOfLaunch(Vector2 velocity)
    {
        base.NotifyOfLaunch(velocity);
        explosionEnabled = false; // SMELL: resetting var set in previous call
    }

    // TODO: this is WILD
    public void DealExplosionDamage(Vector2 f)
    {
        Debug.Log($"Explode from {f}");
        explosionEnabled = true;
        Explode(transform.position);
    }
}
