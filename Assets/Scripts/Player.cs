using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float speedang = 10f;

    private void FixedUpdate()
    {
        transform.position += transform.up * speed * Input.GetAxisRaw("Vertical");
        transform.Rotate(new Vector3(0, 0, speedang * Input.GetAxisRaw("Horizontal")));
    }
}
