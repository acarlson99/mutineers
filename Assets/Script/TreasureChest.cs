using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public EWeaponType[] weaponTypes;
    public GameObject lid;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DestroyIfOOB();

        var rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, 2);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        enabled = false;
        foreach (var wt in weaponTypes)
        {
            collision.gameObject.GetComponent<PirateController>().inventory.Add(wt);
        }
        StartCoroutine(lid.transform.LerpPos(Vector3.up / 2, 1.5f));
        Destroy(gameObject, 2);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnTriggerEnter2D(collision.collider);
    }

    public bool DestroyIfOOB()
    {
        if (transform.position.y < Singleton.Instance.killzoneY)
        {
            Destroy(gameObject);
            return true;
        }
        return false;
    }
}
