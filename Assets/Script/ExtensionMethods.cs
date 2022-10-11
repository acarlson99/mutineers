using System.Collections;
using UnityEngine;

public static class ExtensionMethods
{
    public static Vector2 AddExpExplosionForce(this Rigidbody2D rb, Vector2 position, float force, float upwardEffect, float falloff)
    {
        //position - where the blast occurs
        //force - how much force would be applied if the object were ontop of the explosion
        //upwardEffect - pushes the object up for a cool effect.
        //falloff -  a higher number increases the falloff due to the negative exponent in the equation.
        Vector2 posFromBlast = rb.position - position;
        float distFromBlast = posFromBlast.magnitude;
        float actualForce = ActualForce(distFromBlast, force, falloff);
        Vector2 upwardForce = actualForce * upwardEffect * Vector2.up;

        Vector2 forceVector = rb.GetExplosionForceVector2D(position, force, falloff);

        Vector2 f = forceVector + upwardForce;
        rb.AddForce(f);

        return forceVector + upwardForce;
    }

    public static Vector2 GetExplosionForceVector2D(this Rigidbody2D rb, Vector2 position, float force, float falloff)
    {
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

    // https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/
    //StartCoroutine(spriteRenderer.LerpColor(spriteRenderer.color, new Color(1f, 0, 0, 0.75f), 2f / 5, spriteRenderer, true));
    public static IEnumerator LerpColor(this SpriteRenderer sr, Color start, Color end, float duration, bool repeat = false)
    {
        float time = 0;
        while (time < duration)
        {
            if (!sr?.gameObject) break;
            sr.color = Color.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        if (sr?.gameObject && repeat) yield return sr.LerpColor(end, start, duration, repeat);
    }

    public static IEnumerator LerpColor(this SpriteRenderer sr, Color end, float duration, bool repeat = false)
    {
        yield return sr.LerpColor(sr.color, end, duration, repeat);
    }

    public static bool IsMovingSlowly(this Rigidbody2D rb, float slowMagnitude)
    {
        return rb.velocity.magnitude <= slowMagnitude;
    }
}
