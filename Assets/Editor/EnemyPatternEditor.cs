using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyPattern))]
public class EnemyPatternEditor : Editor
{
    Mesh previewMesh = null;
    float editorDeltaTime = 0f;
    float timer = 0f;
    double lastTimeSinceStartup = 0;
    private void OnEnable()
    {
        if (previewMesh == null)
        {
            previewMesh = new Mesh();
            Vector3[] verts = new Vector3[4];
            Vector2[] uvs = new Vector2[4];
            int[] trids = new int[6];

            const float halfSize = 8f;
            verts[0] = new Vector3(-halfSize, halfSize);
            verts[1] = new Vector3(halfSize, halfSize);
            verts[2] = new Vector3(-halfSize, -halfSize);
            verts[3] = new Vector3(halfSize, -halfSize);

            uvs[0] = new Vector2(0, 1);
            uvs[1] = new Vector2(1, 1);
            uvs[2] = new Vector2(0, 0);
            uvs[3] = new Vector2(1, 0);

            trids[0] = 0;
            trids[1] = 1;
            trids[2] = 2;
            trids[3] = 2;
            trids[4] = 1;
            trids[5] = 3;

            previewMesh.vertices = verts;
            previewMesh.uv = uvs;
            previewMesh.triangles = trids;
        }
    }
    private void OnSceneGUI()
    {
        UpdateEditorTime();
        EnemyPattern pattern = (EnemyPattern)target;
        if (pattern)
        {
            UpdatePreview(pattern);
            ProcessInput(pattern);

            // force scene repaint
            if (Event.current.type == EventType.Repaint)
            {
                SceneView.RepaintAll();
            }
        }
    }

    private void UpdateEditorTime()
    {
        if (lastTimeSinceStartup == 0)
        {
            lastTimeSinceStartup = EditorApplication.timeSinceStartup;
        }
        editorDeltaTime = (float)(EditorApplication.timeSinceStartup - lastTimeSinceStartup) * 60f;
        lastTimeSinceStartup = EditorApplication.timeSinceStartup;
    }

    private void UpdatePreview(EnemyPattern pattern)
    {
        Vector2 endOfLastStep = pattern.transform.position;
        foreach (EnemyStep step in pattern.steps)
        {
            switch (step.mvmtType)
            {
                case EnemyStep.MvmtType.NONE:
                    break;
                case EnemyStep.MvmtType.DIRECTION:
                    {
                        endOfLastStep = DrawLine(endOfLastStep, step);
                        break;
                    }
                case EnemyStep.MvmtType.SPLINE:
                    {
                        endOfLastStep = DrawSpline(step.spline, endOfLastStep, step.mvmtSpeed);
                        break;
                    }
                case EnemyStep.MvmtType.HOMING:
                    {
                        if (GameManager.instance && GameManager.instance.playerCrafts[0])
                        {
                            if (GameManager.instance.playerCrafts[1]
                            && Vector2.Distance(GameManager.instance.playerCrafts[1].transform.position, endOfLastStep) 
                            < Vector2.Distance(GameManager.instance.playerCrafts[0].transform.position, endOfLastStep))
                            {
                                Handles.DrawDottedLine(endOfLastStep, GameManager.instance.playerCrafts[1].transform.position, 1f);
                                endOfLastStep = GameManager.instance.playerCrafts[1].transform.position;
                            }
                            else
                            {
                                Handles.DrawDottedLine(endOfLastStep, GameManager.instance.playerCrafts[0].transform.position, 1f);
                                endOfLastStep = GameManager.instance.playerCrafts[0].transform.position;
                            }
                        }
                        break;
                    }
                default:
                    {
                        Debug.LogError("Trying to preview unknown movement type");
                        break;
                    }
                    
            }
        }

        // Draw animated preview
        timer += editorDeltaTime;
        if (timer >= pattern.TotalTime())
        {
            timer = 0;
        }
        SpriteRenderer renderer = pattern.enemyPrefab.GetComponentInChildren<SpriteRenderer>();
        if (renderer != null)
        {
            Texture tex = renderer.sprite.texture;
            Material mat = renderer.sharedMaterial;

            Vector3 pos = pattern.CalculatePosition(timer);
            Quaternion rot = pattern.CalculateRotation(timer);
            Matrix4x4 matrix = Matrix4x4.TRS(pos, rot, pattern.transform.localScale * 0.05f);
            mat.SetPass(0);
            Graphics.DrawMeshNow(previewMesh, matrix);
        }
    }

    private void ProcessInput(EnemyPattern pattern)
    {
        Event guiEvent = Event.current;
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            Spline s = pattern.steps[0].spline;
            Vector2 offset = pattern.transform.position;
            s.AddSeg(mousePos - offset);
            s.CalculatePoints(pattern.steps[0].mvmtSpeed);
        }
        else if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1 && guiEvent.shift)
        {
            Spline s = pattern.steps[0].spline;
            s.RemLastSeg();
            s.CalculatePoints(pattern.steps[0].mvmtSpeed);
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EnemyPattern pattern = (EnemyPattern)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Idle"))
        {
            pattern.AddStep(EnemyStep.MvmtType.NONE);
        }
        if (GUILayout.Button("Add Linear"))
        {
            pattern.AddStep(EnemyStep.MvmtType.DIRECTION);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Homing"))
        {
            pattern.AddStep(EnemyStep.MvmtType.HOMING);
        }
        if (GUILayout.Button("Add Spline"))
        {
            EnemyStep lastStep = pattern.steps[pattern.steps.Count - 1];
            if (lastStep.mvmtType == EnemyStep.MvmtType.SPLINE)
            {
                Debug.LogWarning("Last step is a spline too, adding a new Bezier segment to it");
                Spline s = lastStep.spline;
                s.AddSeg(s.ctrlPts[s.ctrlPts.Count-1] + (new Vector2(1, 1)));
                s.CalculatePoints(lastStep.mvmtSpeed);
            }
            else
            {
                pattern.AddStep(EnemyStep.MvmtType.SPLINE);
            }
        }
        GUILayout.EndHorizontal();
    }

    private Vector2 DrawLine(Vector2 endOfLastStep, EnemyStep step)
    {
        Vector2 endOfCrtStep = endOfLastStep + step.direction;
        //draw line
        Handles.DrawDottedLine(endOfLastStep, endOfCrtStep, 1f);
        //draw Handle
        Handles.color = Color.green;
        float size = 0.2f;
        Vector2 newPos = Handles.FreeMoveHandle(endOfCrtStep, size, Vector3.zero, Handles.CylinderHandleCap);
        if (newPos != endOfCrtStep)
        {
            step.direction = newPos - endOfLastStep;
        }
        Handles.color = Color.white;
        return endOfCrtStep;
    }

    private Vector2 DrawSpline(Spline spline, Vector2 endOfLastStep, float speed)
    {
        int noSegments = spline.GetNoSegments();
        //draw handler lines
        Handles.color = Color.red;
        for (int i = 0; i < noSegments; i++)
        {
            Vector2[] ctrlPts = spline.GetSegmentPoints(i, endOfLastStep);
            Handles.DrawLine(ctrlPts[0], ctrlPts[1], 1);
            Handles.DrawLine(ctrlPts[2], ctrlPts[3], 1);
        }

        //draw spline pts
        Handles.color = Color.white;
        for (int i = 0; i < spline.linPts.Count; i++)
        {
            Handles.CylinderHandleCap(0, spline.linPts[i] + endOfLastStep, Quaternion.identity, 0.05f, EventType.Repaint);
        }

        //draw control points
        Handles.color = Color.green;
        for (int i = 0; i < spline.ctrlPts.Count; i++)
        {
            Vector2 pos = spline.ctrlPts[i] + endOfLastStep;
            float size = 0.1f;
            if (i % 3 == 0)
            {
                size = 0.2f;
            }
            Vector2 newPos = Handles.FreeMoveHandle(pos, size, Vector3.zero, Handles.CylinderHandleCap);
            if (i > 0 && pos != newPos)
            {
                spline.SetCtrlPoint(i, newPos - endOfLastStep);
                spline.CalculatePoints(speed);
            }
        }

        //reset Handles color to white
        Handles.color = Color.white;
        return endOfLastStep;
    }
}
