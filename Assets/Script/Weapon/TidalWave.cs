using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// TODO: killing throwing pirate with a seagull is buggy (instantly destroys seagull)
public class TidalWave : Weapon
{
    public override EWeaponType WeaponType { get; } = EWeaponType.TidalWave;
    public float speed = 1;

    private Vector3 direction;
    public Vector3 Direction { get { return direction; } }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    private void Deploy()
    {
        NotifyOfLaunch(Vector2.zero);
        var mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mPos.x, mPos.y, transform.position.z);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (!thrown && Input.GetMouseButton((int)MouseButton.Right))
        {
            direction = Vector3.left;
            GetComponent<SpriteRenderer>().flipX = true;
            Deploy();
        }
        else if (!thrown && Input.GetMouseButton((int)MouseButton.Left))
        {
            direction = Vector3.right;
            Deploy();
        }

        if (!Singleton.Instance.cameraBounds.OverlapPoint(transform.position))
        {
            NotifyThrowerEndWeaponUse();
            Destroy(gameObject);
        }

        if (thrown) transform.position = transform.position + direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!thrown) return;
        Debug.Log($"Collision enter {collision.gameObject}");
        if (collision.gameObject.tag != "Player") return;

        // TODO: add force to hit pirate
        collision.gameObject.GetComponent<PirateController>().DealExplosionDamage(Vector2.up, 100);
    }
}
