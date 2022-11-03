using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Crate : Weapon, IExplodable
{
    public override EWeaponType WeaponType { get; } = EWeaponType.Crate;

    private GameObject mouseSprite;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        mouseSprite = new GameObject();
        var sprite = mouseSprite.AddComponent<SpriteRenderer>();
        sprite.sprite = GetComponent<SpriteRenderer>().sprite;
        sprite.color = GetComponent<SpriteRenderer>().color - new Color(0, 0, 0, 0.5f);
        var c = sprite.AddComponent<BoxCollider2D>();
        c.isTrigger = true;
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
            var outList = new List<Collider2D>();
            var filter = new ContactFilter2D
            {
                layerMask = LayerMask.GetMask(new string[] { "Terrain", "Player", "Chest", "Crate" })
            };
            if (mouseSprite.GetComponent<Collider2D>().OverlapCollider(filter, outList) > 0)
            {
                mouseSprite.GetComponent<SpriteRenderer>().color = Color.red - new Color(0, 0, 0, 0.5f);
            }
            else
            {
                NotifyOfLaunch(Vector2.zero);
                var v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                v = new Vector3(v.x, v.y, transform.position.z);
                transform.position = v;
                rb.velocity = Vector2.zero;

                GetComponent<Collider2D>().isTrigger = false;
                thrown = true;
            }
        }
        else if (!thrown)
        {
            rb.velocity = Vector2.zero;
        }
        if (Input.GetMouseButtonUp((int)MouseButton.Left) && !thrown)
        {
            mouseSprite.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color - new Color(0, 0, 0, 0.5f);
        }
    }

    protected void OnCollisionStay2D(Collision2D collision)
    {
        if (thrown && rb.IsMovingSlowly(0.01f) && thrower)
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
        Destroy(mouseSprite);
        mouseSprite = null;
    }

    public void DealExplosionDamage(Vector2 f, float damageMultiplier)
    {
        if (!thrown) return;
        Destroy(gameObject);
    }
}
