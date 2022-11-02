using Unity.VisualScripting;
using UnityEngine;

// TODO: dont allow placement inside another rigidbody
public class GunpowderBarrel : Exploder, IExplodable
{
    public override EWeaponType WeaponType { get; } = EWeaponType.Gunpowder;

    private GameObject mouseSprite;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        mouseSprite = new GameObject();
        var sprite = mouseSprite.AddComponent<SpriteRenderer>();
        sprite.sprite = GetComponent<SpriteRenderer>().sprite;
        sprite.color = GetComponent<SpriteRenderer>().color - new Color(0, 0, 0, 0.3f);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (mouseSprite)
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = -1;
            mouseSprite.transform.position = pos;
        }

        if (Input.GetMouseButtonDown((int)MouseButton.Left) && !thrown)
        {
            NotifyOfLaunch(Vector2.zero);
            var v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            v = new Vector3(v.x, v.y, transform.position.z);
            transform.position = v;
            rb.velocity = Vector2.zero;

            GetComponent<Collider2D>().isTrigger = false;
            thrown = true;
        }
        else if (!thrown)
        {
            rb.velocity = Vector2.zero;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision) { }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        if (thrown && rb.IsMovingSlowly(0.01f))
        {
            NotifyThrowerEndWeaponUse();
        }
    }

    private void OnDestroy()
    {
        if (mouseSprite) Destroy(mouseSprite);
    }

    public override void NotifyOfLaunch(Vector2 velocity)
    {
        base.NotifyOfLaunch(velocity);
        explosionEnabled = false; // SMELL: resetting var set in previous call
        Destroy(mouseSprite);
        mouseSprite = null;
    }

    public void DealExplosionDamage(Vector2 f, float damageMultiplier)
    {
        if (!thrown) return;
        Debug.Log($"Explode from {f}");
        explosionEnabled = true;
        ExplodeAndDestroy(transform.position);
    }
}
