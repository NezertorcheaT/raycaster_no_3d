using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Rendering/Raycast Camera")]
public class RaycastCamera : MonoBehaviour
{
    [SerializeField] private LayerMask renderMask;
    [SerializeField] private float fov = 180;
    [SerializeField] private int wight = 5;
    [SerializeField] private float clippingFar = 1;
    [SerializeField] private Transform canvas;
    [SerializeField] private Image canvasImage;
    [SerializeField] private bool drawGizmos = false;
    private Vector2[] raysList;
    private float[] zList;
    private Dictionary<int, Color> colorList;

    private void Awake()
    {
        UpdateRays();
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.yellow;
            foreach (var ray in raysList)
            {
                Gizmos.DrawLine(transform.position, (Vector2)transform.position + ray);
            }
        }
    }
    private void UpdateRays()
    {
        raysList = new Vector2[wight];
        for (int i = 0; i < wight; i++)
        {
            raysList[i] = new Vector2(
                Mathf.Cos((fov / wight * i + transform.rotation.eulerAngles.z + fov / 2f) * Mathf.Deg2Rad),
                Mathf.Sin((fov / wight * i + transform.rotation.eulerAngles.z + fov / 2f) * Mathf.Deg2Rad)
                );
        }
    }
    private void CastRays()
    {
        colorList = new Dictionary<int, Color>();
        zList = new float[wight];
        for (int i = 0; i < wight; i++)
        {
            RaycastHit2D rch = Physics2D.Raycast(transform.position, raysList[i], clippingFar, renderMask);
            if (rch && rch.collider.GetComponent<SpriteRenderer>())
            {
                colorList.Add(i, rch.collider.GetComponent<SpriteRenderer>().color);
                raysList[i] = (rch.point - (Vector2)transform.position);
                zList[i] = rch.collider.gameObject.transform.localScale.z;
            }
            else
                raysList[i] = raysList[i] * clippingFar;
        }
    }
    private void RenderRays()
    {
        foreach (Transform child in canvas.transform)
        {
            Destroy(child.gameObject);
        }
        for (
            int i = 0;
            i < wight && (raysList.Length != 0 && colorList.Count != 0);
            i++
            )
        {
            Image image = Instantiate<Image>(canvasImage);
            image.gameObject.transform.SetParent(canvas);
            Color color = Color.black;
            if (colorList.ContainsKey(i) && raysList[i].magnitude != 0)
                color = colorList[i] / raysList[i].magnitude;

            color.a = 255;
            image.color = color; 

            float scale = ((wight / (2f * Mathf.Tan(fov / 2 * Mathf.Deg2Rad))) * (zList[i] / 100f)) /
                    (raysList[i].magnitude * Mathf.Cos(
                            (transform.rotation.eulerAngles.z - Vector2.SignedAngle(new Vector2(0, 1), raysList[i])
                            ) * Mathf.Deg2Rad
                        )
                    );


            image.transform.localScale = 
                new Vector2(image.transform.localScale.x, Mathf.Min(scale, 1));
        }
    }
    private void FixedUpdate()
    {
        UpdateRays();
        CastRays();
    }
    private void LateUpdate()
    {
        RenderRays();
    }
}
