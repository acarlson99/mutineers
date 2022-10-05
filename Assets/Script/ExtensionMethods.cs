using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class ExtensionMethods
{
    public static Vector2 AddExpExplosionForce(this Rigidbody2D rb, Vector2 position, float force, float upwardEffect, float falloff)
    {
        // TODO: fix this, is kinda janky

        //position - where the blast occurs
        //force - how much force would be applied if the object were ontop of the explosion
        //upwardEffect - pushes the object up for a cool effect.
        //falloff -  a higher number increases the falloff due to the negative exponent in the equation.
        Vector3 posFromBlast = rb.position - position;
        float distFromBlast = posFromBlast.magnitude;
        float actualForce = force * Mathf.Pow(falloff, -distFromBlast);
        Vector3 upwardForce = Vector2.up * upwardEffect;

        Vector2 f = posFromBlast.normalized * actualForce + upwardForce;
        //Vector2 f = posFromBlast.normalized * actualForce + upwardForce;
        rb.AddForce(f);

        return posFromBlast.normalized * actualForce;
    }
    public static Vector2 AddExpExplosionForceCalc(this Rigidbody2D rb, Vector2 position, float force, float upwardEffect, float falloff)
    {
        // TODO: fix this, is kinda janky

        //position - where the blast occurs
        //force - how much force would be applied if the object were ontop of the explosion
        //upwardEffect - pushes the object up for a cool effect.
        //falloff -  a higher number increases the falloff due to the negative exponent in the equation.
        Vector3 posFromBlast = rb.position - position;
        float distFromBlast = posFromBlast.magnitude;
        float actualForce = force * Mathf.Pow(falloff, -distFromBlast);
        Vector3 upwardForce = Vector2.up * upwardEffect;

        Vector2 f = posFromBlast.normalized * actualForce + upwardForce;
        //Vector2 f = posFromBlast.normalized * actualForce + upwardForce;
        //rb.AddForce(f);

        return posFromBlast.normalized * actualForce;
    }
}
