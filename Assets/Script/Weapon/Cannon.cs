using UnityEngine;

public class Cannon : Weapon
{
    public override EWeaponType WeaponType { get; } = EWeaponType.Cannon;
    public GameObject cannonballPrefab;

    public bool hasFired { get; protected set; } = false;

    // Start is called before the first frame update
    protected override void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {

    }

    private void OnMouseDrag()
    {
        thrown = true;
        Singleton.Instance.CamQuietUnfollow();
        var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        point.z = transform.position.z;
        transform.position = point;
    }

    public void Fire()
    {
        if (hasFired) return;
        hasFired = true;

        var ballDirection = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one) * Vector2.right;
        var fab = Instantiate(cannonballPrefab, transform.position, Quaternion.identity);
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), fab.GetComponent<Collider2D>());
        Debug.Log($"Cannon fired ballDir {ballDirection}");
        fab.GetComponent<Rigidbody2D>().AddForce(ballDirection * 10, ForceMode2D.Impulse);
        fab.GetComponent<Exploder>().thrower = thrower;

        Singleton.Instance.CamFollow(fab.transform);

        Destroy(gameObject, 2);
    }
}
