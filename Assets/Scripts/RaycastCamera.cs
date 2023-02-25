using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NTC.Global.Cache;
using Unity.VisualScripting;
using UnityEngine.Rendering.RendererUtils;

[AddComponentMenu("Rendering/Raycast Camera")]
public class RaycastCamera : MonoCache
{
    [SerializeField] protected LayerMask renderMask;
    [SerializeField] protected int fov = 180;
    [SerializeField] protected int wight = 5;
    [SerializeField] protected float clippingFar = 1;
    [SerializeField] protected Transform canvas;
    [SerializeField] protected Image canvasImage;
    protected Vector2[] raysList;
    protected Dictionary<int, SpriteRenderer> rendererList;

    private void Awake()
    {
        UpdateRays();
    }
    private void UpdateRays()
    {
        rendererList = new Dictionary<int, SpriteRenderer>();
        raysList = new Vector2[wight];
        for (int i = 0; i < wight; i++)
        {
            raysList[i] = new Vector2(Mathf.Cos(((float)fov / (float)wight * (float)i + transform.rotation.eulerAngles.z) * Mathf.Deg2Rad), Mathf.Sin(((float)fov / (float)wight * (float)i + transform.rotation.eulerAngles.z) * Mathf.Deg2Rad));
        }
        for (int i = 0; i < wight; i++)
        {
            RaycastHit2D rch = Physics2D.Raycast(transform.position, raysList[i], clippingFar, renderMask);
            if (rch && rch.collider.GetComponent<SpriteRenderer>())
            {
                rendererList.Add(i, rch.collider.GetComponent<SpriteRenderer>());
                raysList[i] = rch.point - (Vector2)transform.position;
            }
            else
                raysList[i] = raysList[i] * clippingFar;
        }


        foreach (Transform child in canvas.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < wight; i++)
        {
            Image cm = Instantiate<Image>(canvasImage);
            cm.gameObject.transform.SetParent(canvas);
            if (rendererList.ContainsKey(i))
                cm.color = rendererList[i].color * (1 / raysList[i].magnitude);
            else
                cm.color = Color.black;
            try
            {
                cm.transform.localScale = new Vector2(cm.transform.localScale.x, 1f / raysList[i].magnitude);
            }
            catch
            {

            }
        }
    }
    protected override void Run()
    {
        UpdateRays();
    }
}
