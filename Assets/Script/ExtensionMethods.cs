using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Reflection;

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

        if (forceVector.y < 0) forceVector.y *= -1;
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
            if (sr == null || sr.gameObject == null) break;
            sr.color = Color.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        if (sr != null && sr.gameObject != null && repeat) yield return sr.LerpColor(end, start, duration, repeat);
    }

    public static IEnumerator LerpColor(this SpriteRenderer sr, Color end, float duration, bool repeat = false)
    {
        return sr.LerpColor(sr.color, end, duration, repeat);
    }

    public static IEnumerator LerpPos(this Transform tr, Vector3 start, Vector3 end, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            tr.position = Vector3.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return tr.position;
        }
    }

    public static IEnumerator LerpPos(this Transform tr, Vector3 direction, float duration)
    {
        return tr.LerpPos(tr.position, tr.position + direction, duration);
    }

    public static bool IsMovingSlowly(this Rigidbody2D rb, float slowMagnitude)
    {
        return rb.velocity.magnitude <= slowMagnitude;
    }

    private static IEnumerator _scheduleFuncall<T>(Func<T, int> f, T param, float t)
    {
        yield return new WaitForSeconds(t);
        f(param);
    }

    public static void ScheduleFuncall<T>(this MonoBehaviour mb, Func<T, int> f, T param, float t)
    {
        var coroutine = _scheduleFuncall(f, param, t);
        mb.StartCoroutine(coroutine);
    }

    public static float MinY(this PolygonCollider2D pc)
    {
        return pc.points.Select(p => p.y).Min();
    }

    public static float MaxY(this PolygonCollider2D pc)
    {
        return pc.points.Select(p => p.y).Max();
    }

    public static float MinX(this PolygonCollider2D pc)
    {
        return pc.points.Select(p => p.x).Min();
    }

    public static float MaxX(this PolygonCollider2D pc)
    {
        return pc.points.Select(p => p.x).Max();
    }

    public static float Height(this PolygonCollider2D pc)
    {
        return pc.MaxY() - pc.MinY();
    }

    public static float Width(this PolygonCollider2D pc)
    {
        return pc.MaxX() - pc.MinX();
    }

    public static T RandomElement<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.RandomElementUsing<T>(new System.Random());
    }

    public static T RandomElementUsing<T>(this IEnumerable<T> enumerable, System.Random rand)
    {
        int index = rand.Next(0, enumerable.Count());
        return enumerable.ElementAt(index);
    }

    public static List<float>.Enumerator GetEnumerator(this Vector2 v)
    {
        return new List<float>() { v.x, v.y }.GetEnumerator();
    }
}
