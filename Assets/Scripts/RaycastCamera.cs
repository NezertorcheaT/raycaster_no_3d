using System;
using System.Collections.Generic;
using UnityEngine;
using NTC.Global.Cache;

public class RaycastCamera : MonoCache
{
    [SerializeField] protected LayerMask renderMask;
    [SerializeField] protected int fov;
    [SerializeField, Min(2)] protected int wight;
    protected Vector2[] raysList;

    private void Awake()
    {
        UpdateRays();
    }
    private void UpdateRays()
    {
        for (int i = 0; i < wight; i++)
        {
            raysList[i] = new Vector2(Mathf.Cos((360f / i) * Mathf.Deg2Rad), Mathf.Sin((360f / i) * Mathf.Deg2Rad));
        }
    }
    private void DebugDrawRays()
    {
        Gizmos.color = Color.yellow;
        foreach (var ray in raysList)
        {
            Gizmos.DrawRay(new Ray(Vector2.zero, ray));
        }
    }
    protected override void Run()
    {
        UpdateRays();
        DebugDrawRays();
    }
}
