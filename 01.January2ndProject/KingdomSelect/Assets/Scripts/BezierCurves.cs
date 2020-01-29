using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurves : MonoBehaviour
{
    
    public Transform p0, p1, p2;
    public Transform b;

    public LineRenderer lineRenderer;

    int numPoints = 50;
    Vector3[] bPositions = new Vector3[50];

    void Start()
    {
        lineRenderer.positionCount = numPoints;
        DrawQuadraticCurves();
    }

    void Update()
    {
        
    }

    private void DrawQuadraticCurves() {
        for (int i = 1; i < numPoints+1; i++) {
            float t = i / (float)numPoints;
            bPositions[i-1] = CalculateQuadraticBezierPoint(t, p0.position, p1.position, p2.position);
        }
        lineRenderer.SetPositions(bPositions);
    }

    Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2) {
        // QuadraticBezier 식입니다.
        // B(t) = (1-t)2P0 + 2(1-t)tP1 + t2P2 
        Vector3 b = (1 - t)*(1 - t) * 2 * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
        return b;
    }
}
