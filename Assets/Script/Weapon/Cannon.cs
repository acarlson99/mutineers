using UnityEngine;

public class Cannon : Weapon
{
    // TODO: make this not pass through walls, add rigidbody
    public override EWeaponType WeaponType { get; } = EWeaponType.Cannon;
    public GameObject cannonballPrefab;

    public bool hasFired { get; protected set; } = false;

    private Vector3 origin;
    private GameObject rangeIndicator;

    public Sprite rangeIndicatorSprite;
    public Color rangeIndicatorColor;
    public float rangeRadius;

    // Start is called before the first frame update
    protected override void Start()
    {
        origin = transform.position;
        rangeIndicator = new GameObject("range indicator");
        rangeIndicator.transform.position = origin;
        rangeIndicator.transform.localScale = new Vector2(rangeRadius * 2, rangeRadius * 2);
        SpriteRenderer spriteRenderer = rangeIndicator.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = rangeIndicatorSprite;
        spriteRenderer.color = rangeIndicatorColor - new Color(0, 0, 0, 0.5f);
    }

    // Update is called once per frame
    protected override void Update()
    {

    }

    private void OnDestroy()
    {
        Destroy(rangeIndicator);
    }

    private void OnMouseDrag()
    {
        thrown = true;
        Singleton.Instance.CamQuietUnfollow();
        var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        point.z = transform.position.z;

        point = origin + Vector3.ClampMagnitude(point - origin, rangeRadius);
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
