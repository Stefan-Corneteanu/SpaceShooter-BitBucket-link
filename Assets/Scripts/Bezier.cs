using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bezier
{
    public static Vector2 CalculateQuadratic(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        Vector2 p1 = Vector2.Lerp(a, b, t);
        Vector2 p2 = Vector2.Lerp(b, c, t);
        Vector2 res = Vector2.Lerp(p1, p2, t);
        return res;
    }

    public static Vector2 CalculateCubic(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
    {
        Vector2 p1 = CalculateQuadratic(a, b, c, t);
        Vector2 p2 = CalculateQuadratic(b, c, d, t);
        Vector2 res = Vector2.Lerp(p1, p2, t);
        return res;
    }

    public static float CalculateCubicCurveLength(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        float netLen = Vector2.Distance(a, b) + Vector2.Distance(b, c) + Vector2.Distance(c, d);
        return Vector2.Distance(a, d) + netLen / 2;
    }
}
