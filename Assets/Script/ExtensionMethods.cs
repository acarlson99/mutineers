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
        Vector2 posFromBlast = rb.position - position;
        float distFromBlast = posFromBlast.magnitude;
        float actualForce = ActualForce(distFromBlast, force, falloff);
        Vector2 upwardForce = Vector2.up / 2 * upwardEffect * actualForce;

        Vector2 forceVector = rb.GetExplosionForceVector2D(position, force, falloff);

        Vector2 f = forceVector + upwardForce;
        rb.AddForce(f);

        return forceVector;
    }

    public static Vector2 GetExplosionForceVector2D(this Rigidbody2D rb, Vector2 position, float force, float falloff)
    {
        // TODO: fix this, is kinda janky

        //position - where the blast occurs
        //force - how much force would be applied if the object were ontop of the explosion
        //upwardEffect - pushes the object up for a cool effect.
        //falloff -  a higher number increases the falloff due to the negative exponent in the equation.
        Vector2 posFromBlast = rb.position - position;
        float distFromBlast = posFromBlast.magnitude;
        float actualForce = ActualForce(distFromBlast, force, falloff);

        return posFromBlast.normalized * actualForce;
    }

    public static float ActualForce(float distFromBlast, float force, float falloff)
    {
        return force * Mathf.Pow(falloff, -distFromBlast);
    }
}
