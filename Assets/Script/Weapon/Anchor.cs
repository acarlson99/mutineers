using Unity.VisualScripting;
using UnityEngine;

public class Anchor : Weapon
{
    private Vector3 direction;
    public Vector3 Direction { get { return direction; } }
    public float launchMultiplier = 75;
    public float damage = 300;

    public override EWeaponType WeaponType { get; } = EWeaponType.Anchor;

    private bool toBeDestroyed = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!thrown || toBeDestroyed) return;
        if (collision.gameObject.tag == "Player") return;
        toBeDestroyed = true;
        NotifyThrowerEndWeaponUse();
        Destroy(gameObject, 2);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!thrown) return;
        Debug.Log($"Collision enter {collision.gameObject}");
        if (collision.gameObject.tag != "Player") return;

        var rb = collision.gameObject.GetComponent<Rigidbody2D>();
        rb.AddForce((Direction + Vector3.up) * launchMultiplier);
        var pc = collision.gameObject.GetComponent<PirateController>();
        pc.DealRawDamage(damage);
    }

    private void Deploy()
    {
        rb.velocity = Vector3.zero;
        var mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mPos.x, mPos.y, transform.position.z);
        NotifyOfLaunch(Vector2.zero);
    }

    protected override void Update()
    {
        base.Update();

        if (!thrown && Input.GetMouseButton((int)MouseButton.Left))
        {
            Deploy();
        }

        if (!Singleton.Instance.cameraBounds.OverlapPoint(transform.position))
        {
            NotifyThrowerEndWeaponUse();
            Destroy(gameObject);
        }
    }

    public override void NotifyOfLaunch(Vector2 velocity)
    {
        base.NotifyOfLaunch(velocity);
    }
}
