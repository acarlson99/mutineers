using Unity.VisualScripting;
using UnityEngine;

public class Banana : Exploder
{

    public override string weaponName { get; set; } = "banana";

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown((int)MouseButton.Left) && explosionEnabled)
        {
            Debug.Log("GOOOOOO");
            Explode(rb.position);
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
    }
}
