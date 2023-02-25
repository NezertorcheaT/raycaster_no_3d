using System;
using System.Collections.Generic;
using UnityEngine;
using NTC.Global.Cache;

public class Player : MonoCache
{
    public float speed = 1f;
    public float speedang = 10f;
    protected override void Run()
    {
        transform.position += transform.up * speed * Input.GetAxisRaw("Vertical");
        transform.Rotate(new Vector3(0, 0, speedang * Input.GetAxisRaw("Horizontal")));
    }
}
