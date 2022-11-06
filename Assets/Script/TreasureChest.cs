using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    private static IEnumerable<int> ObjsRange(string tag)
    {
        var objs = GameObject.FindGameObjectsWithTag(tag);
        var range = Enumerable.Range(0, 0);
        foreach (var obj in objs)
        {
            if (!obj.GetComponent<BoxCollider2D>()) continue;

            var bc = obj.GetComponent<BoxCollider2D>();

            var width = Mathf.Abs(bc.bounds.max.x - bc.bounds.min.x);
            var x = obj.transform.position.x;
            var r = Enumerable.Range((int)(x - width / 2), (int)width);
            range = range.Union(r);
        }
        return range;
    }

    public static GameObject DropRandomly()
    {
        var terrain = ObjsRange("Terrain");
        var chests = ObjsRange("Chest"); // no chests falling on one another
        var range = from tx in terrain
                    where !chests.Contains(tx)
                    select tx;

        Debug.Log($"range {String.Join(", ", range.ToArray())}");
        Debug.Log($"chests {String.Join(", ", chests.ToArray())}");
        Debug.Log($"terrain {String.Join(", ", terrain.ToArray())}");

        if (range.Count() == 0) return null;

        var pos = new Vector3(range.RandomElement(), Singleton.Instance.cameraBounds.MaxY(), 0);
        Debug.Log($"Selected {pos.x}");
        GameObject chestObj = GameObject.Instantiate(Singleton.Instance.chestFab, pos, Quaternion.identity);
        var chest = chestObj.GetComponent<TreasureChest>();
        var warr = Singleton.Instance.chestWeapons;
        for (int i = 0; i < 2; i++)
        {
            if (chest.contents.Count > 0) chest.contents.Add(warr[UnityEngine.Random.Range(0, warr.Length)]);
            else Debug.LogWarning("Empty chest");
        }
        return chestObj;
    }

    public List<EWeaponType> contents = new();
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
        foreach (var wt in contents)
        {
            collision.gameObject.GetComponent<PirateController>().inventory.Add(wt);
        }
        StartCoroutine(lid.transform.LerpPos(Vector3.up / 2, 1.5f));
        // TODO: sprite animation, display contained goodies
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
