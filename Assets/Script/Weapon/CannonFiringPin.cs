using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CannonFiringPin : MonoBehaviour
{
    public GameObject cannon;

    Vector3 pos;
    float maxX;

    // Start is called before the first frame update
    void Start()
    {
        maxX = transform.localPosition.x;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnMouseDown()
    {
        //pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cannon.GetComponent<Cannon>().Fire();
    }

    //private void OnMouseDrag()
    //{
    //    var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    var dx = pos.x - point.x;
    //    transform.localPosition -= new Vector3(dx, 0, 0);
    //    var p = transform.localPosition;
    //    if (p.x > maxX)
    //    {
    //        p.x = maxX;
    //        transform.localPosition = p;
    //    }
    //    else pos = point;
    //}

    //private void OnMouseUpAsButton()
    //{
    //    Debug.Log($"FIRE: {maxX} {transform.localPosition.x}");
    //}
}
