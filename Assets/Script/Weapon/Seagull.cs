using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// TODO: killing throwing pirate with a seagull is buggy (instantly destroys seagull)
public class Seagull : Weapon
{
    public override string weaponName { get; set; } = "seagull";
    public GameObject seagullBomb;
    public float maxX = 30;
    public float speed = 1;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (!thrown && Input.GetMouseButton((int)MouseButton.Left))
        {
            NotifyOfLaunch(Vector2.zero);
            var mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mPos.x, mPos.y, transform.position.z);
        }

        if (Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            var fab = Instantiate(seagullBomb);
            fab.transform.position = transform.position;
            var frb = fab.GetComponent<Rigidbody2D>();
            frb.AddForce(new Vector3(speed, 0, 0), ForceMode2D.Impulse);
        }
        transform.position = transform.position + new Vector3(speed, 0, 0) * Time.deltaTime;
        if (transform.position.x >= maxX)
        {
            NotifyThrowerEndWeaponUse();
            Destroy(gameObject);
        }
    }
}
