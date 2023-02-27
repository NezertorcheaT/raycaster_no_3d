using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float speedang = 10f;

    private void Update()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }
    private void FixedUpdate()
    {
        transform.position += transform.up * speed * Input.GetAxisRaw("Vertical");
        transform.position -= transform.right * speed * Input.GetAxisRaw("Horizontal");
        transform.rotation *= Quaternion.Euler(0, 0, Input.GetAxis("Mouse X") * speedang);
    }
}
