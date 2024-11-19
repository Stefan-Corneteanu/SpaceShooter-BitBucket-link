using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Spline
{
    [SerializeField]
    public List<Vector2> ctrlPts;

    [SerializeField]
    bool isClosed;

    [HideInInspector]
    public List<Vector2> linPts = new List<Vector2>();

    public Spline()
    {
        ctrlPts = new List<Vector2>
        {
            Vector2.zero,
            Vector2.down*2f,
            (Vector2.down+Vector2.right)*2f,
            (2*Vector2.down+Vector2.right)*2f,
        };
        CalculatePoints(0.4f);
    }

    public int GetNoSegments()
    {
        return ctrlPts.Count / 3;
    }

    public Vector2[] GetSegmentPoints(int idx, Vector2 offset)
    {
        return new Vector2[] {
            ctrlPts[3*idx] + offset,
            ctrlPts[3*idx+1] + offset,
            ctrlPts[3*idx+2] + offset,
            ctrlPts[3*idx+3] + offset,
        };
    }

    public void CalculatePoints(float speed)
    {
        linPts.Clear();
        linPts.Add(ctrlPts[0]);
        Vector2 prevPt = ctrlPts[0];
        float distSinceLast = 0;

        int noSegments = GetNoSegments();
        for (int i = 0; i < noSegments; i++)
        {
            Vector2[] segPts = GetSegmentPoints(i, Vector2.zero);

            float curveLen = Bezier.CalculateCubicCurveLength(segPts[0], segPts[1], segPts[2], segPts[3]);
            float spacing = speed;

            int divs = Mathf.CeilToInt(curveLen / speed);
            divs = Math.Min(1000, divs);
            float t = 0;
            while (t <= 1)
            {
                t += 1.0f / divs;
                Vector2 ptOnCurve = Bezier.CalculateCubic(segPts[0], segPts[1], segPts[2], segPts[3], t);
                distSinceLast += Vector2.Distance(prevPt, ptOnCurve);
                while (distSinceLast >= spacing)
                {
                    float overshoot = distSinceLast - spacing;
                    Vector2 newEvenlySpacedPt = ptOnCurve + (prevPt - ptOnCurve).normalized * overshoot;
                    linPts.Add(newEvenlySpacedPt);
                    distSinceLast = overshoot;
                    prevPt = newEvenlySpacedPt;
                }
                prevPt = ptOnCurve;
                //linPts.Add(ptOnCurve);
            }
        }
    }

    internal Vector2 FirstPoint()
    {
        return ctrlPts[0];
    }

    internal Vector2 LastPoint()
    {
        return ctrlPts[ctrlPts.Count - 1];
    }

    internal float Length()
    {
        int noSegments = GetNoSegments();
        float splineLen = 0f;
        for (int i = 0; i < noSegments; i++)
        {
            Vector2[] segPts = GetSegmentPoints(i, Vector2.zero);
            float segLen = Bezier.CalculateCubicCurveLength(segPts[0], segPts[1], segPts[2], segPts[3]);
            splineLen += segLen;
        }
        return splineLen;
    }

    public void SetCtrlPoint(int i, Vector2 newPos)
    {
        Vector2 dPos = newPos - ctrlPts[i];
        ctrlPts[i] = newPos;
        if (i % 3 == 0) // anchor point
        {
            if (i + 1 < ctrlPts.Count)
            {
                ctrlPts[i + 1] += dPos;
            }

            if (i - 1 >= 0)
            {
                ctrlPts[i - 1] += dPos;
            }
        }
        else // angle tangent point
        {
            bool nextPointIsAnchor = ((i + 1) % 3 == 0);
            int anchorIdx = nextPointIsAnchor ? i + 1 : i - 1; //angle tangent points are preceeded or succeeded by an anchor point
            int tanIdx = nextPointIsAnchor ? i + 2 : i - 2;
            if (tanIdx >=0 && tanIdx < ctrlPts.Count)
            {
                float dist = Vector2.Distance(ctrlPts[tanIdx], ctrlPts[anchorIdx]);
                Vector2 dir = (ctrlPts[anchorIdx] - newPos).normalized;
                ctrlPts[tanIdx] = ctrlPts[anchorIdx] + dir * dist;
            }
        }
    }

    internal Vector2 GetPosition(float normalizedTime)
    {
        if (normalizedTime < 0)
        {
            Debug.LogWarning("GetPosition received negative time");
            return Vector2.zero;
        }
        else if (normalizedTime > 1)
        {
            Debug.LogWarning("GetPosition received time greater than 1");
            return Vector2.zero;
        }

        if (linPts.Count == 0)
        {
            Debug.LogWarning("Spline has no points");
            return Vector2.zero;
        }

        float fIdx = (linPts.Count - 1) * normalizedTime;
        int idxA = (int)fIdx;
        int idxB = Mathf.CeilToInt(fIdx);
        float dist = fIdx - idxA;
        return Vector2.Lerp(linPts[idxA], linPts[idxB], dist);
    }

    public void AddSeg(Vector2 pos)
    {
        int lastIdx = ctrlPts.Count - 1;
        Vector2 pt1 = ctrlPts[lastIdx] + ctrlPts[lastIdx] - ctrlPts[lastIdx - 1];
        Vector2 pt2 = (pt1 + pos) * 0.5f;
        ctrlPts.Add(pt1);
        ctrlPts.Add(pt2);
        ctrlPts.Add(pos);
    }

    public void RemLastSeg()
    {
        if (ctrlPts.Count > 4)
        {
            ctrlPts.RemoveRange(ctrlPts.Count - 3, 3);
        }
        else
        {
            Debug.LogError("Spline should have at least one Bezier segment!");
        }
    }
}
