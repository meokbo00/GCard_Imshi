using UnityEngine;

public class DragWithMouse : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isDragging = false;
    private Vector3 offset;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnMouseDown()
    {
        isDragging = true;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - mouseWorldPos;
    }

    void OnMouseUp()
    {
        isDragging = false;
        rb.velocity = Vector2.zero; // 마우스에서 뗄 때 속도 제거
    }

    void FixedUpdate()
    {
        if (isDragging)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 targetPos = (mouseWorldPos + offset);
            rb.MovePosition(targetPos);
        }
    }
}
