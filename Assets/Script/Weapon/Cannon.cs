using UnityEngine;

public class Cannon : Weapon
{
    public override EWeaponType WeaponType { get; } = EWeaponType.Cannon;
    public GameObject cannonballPrefab;

    // Start is called before the first frame update
    protected override void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {

    }

    public void Fire()
    {
        var ballDirection = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one) * Vector2.right;
        var fab = Instantiate(cannonballPrefab, transform.position, Quaternion.identity);
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), fab.GetComponent<Collider2D>());
        Debug.Log($"Cannon fired ballDir {ballDirection}");
        fab.GetComponent<Rigidbody2D>().AddForce(ballDirection * 10, ForceMode2D.Impulse);
    }
}
