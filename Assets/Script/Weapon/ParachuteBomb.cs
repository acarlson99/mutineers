using Unity.VisualScripting;
using UnityEngine;

public class ParachuteBomb : Exploder
{
    public override string weaponName { get; } = "parachute bomb";
    public override EWeaponType weaponType { get; } = EWeaponType.Parachute;

    public float maxFallSpeed = -2f;

    private float g = 0f;
    private bool parachuteDeployed = false;
    private GameObject parachute;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        g = rb.gravityScale;
        if (lc && lc.gravityScale != g) g = lc.gravityScale;

        parachute = transform.Find("Parachute").gameObject;
        parachute.SetActive(false);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (!thrown) return;

        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton((int)MouseButton.Left))
        {
            if (mouseWorldPos.x < transform.position.x)
                rb.AddForce(Vector2.right / 4, ForceMode2D.Force);
            else
                rb.AddForce(Vector2.left / 4, ForceMode2D.Force);
        }

        if (!parachuteDeployed && mouseWorldPos.y < transform.position.y && rb.velocity.y < maxFallSpeed)
            DeployParachute();
        else if (parachuteDeployed && mouseWorldPos.y > transform.position.y)
            StoreParachute();
        if (parachuteDeployed) ParachuteDeployedUpdate();
        else ParachuteStoredUpdate();
    }

    public void StoreParachute()
    {
        parachuteDeployed = false;
    }

    public void DeployParachute()
    {
        parachuteDeployed = true;
    }

    public void ParachuteDeployedUpdate()
    {
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, maxFallSpeed));
        parachute.SetActive(true);
    }

    public void ParachuteStoredUpdate()
    {
        parachute.SetActive(false);
    }
}
