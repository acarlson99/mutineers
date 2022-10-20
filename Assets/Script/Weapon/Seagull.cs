using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// TODO: killing throwing pirate with a seagull is buggy (instantly destroys seagull)
public class Seagull : Weapon
{
    public override EWeaponType WeaponType { get; } = EWeaponType.Seagull;
    public GameObject seagullBomb;
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
        else if (Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            var fab = Instantiate(seagullBomb);
            fab.transform.position = transform.position;
            var frb = fab.GetComponent<Rigidbody2D>();
            frb.AddForce(direction * speed, ForceMode2D.Impulse);
        }

        if (!Singleton.Instance.cameraBounds.OverlapPoint(transform.position))
        {
            NotifyThrowerEndWeaponUse();
            Destroy(gameObject);
        }

        if (thrown) transform.position = transform.position + direction * speed * Time.deltaTime;
    }
}
