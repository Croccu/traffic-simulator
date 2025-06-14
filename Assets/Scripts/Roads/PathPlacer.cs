﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPlacer : MonoBehaviour {

    public float spacing = .1f;
    public float resolution = 1;

    void Start () {
        PathCreator creator = Object.FindFirstObjectByType<PathCreator>();
        if (creator == null) {
            Debug.LogError("No PathCreator found in the scene.");
            return;
        }

        Vector2[] points = creator.path.CalculateEvenlySpacedPoints(spacing, resolution);
        foreach (Vector2 p in points)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.transform.position = p;
            g.transform.localScale = Vector3.one * spacing * .5f;
        }
    }
}
