using Unity.VisualScripting;
using UnityEngine;

// TODO: killing throwing pirate with a seagull is buggy (instantly destroys seagull)
public class TidalWave : Weapon
{
    public override EWeaponType WeaponType { get; } = EWeaponType.TidalWave;
    public float speed = 1;
    public float launchMultiplier = 75;
    public float damage = 300;

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
        var minY = Singleton.Instance.cameraBounds.MinY();
        var mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var height = Mathf.Abs(minY - mPos.y);
        transform.localScale = new Vector3(transform.localScale.x, height, transform.localScale.z);
        transform.position = new Vector3(mPos.x,
            mPos.y - height / 2,
            transform.position.z);
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

        var rb = collision.gameObject.GetComponent<Rigidbody2D>();
        rb.AddForce((Direction + Vector3.up) * launchMultiplier);
        var pc = collision.gameObject.GetComponent<PirateController>();
        pc.DealRawDamage(damage);
    }
}
