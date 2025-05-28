using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragWithMouse : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Rigidbody2D rb;
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 originalPosition;
    private float zPosition;
    private Canvas canvas;

    void Start()
    {
        // Rigidbody2D 설정
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        // 캔버스 찾기 (자식에서)
        canvas = GetComponentInChildren<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found in children!");
        }
        else
        {
            // 캔버스가 월드 스페이스인지 확인
            if (canvas.renderMode != RenderMode.WorldSpace)
            {
                Debug.Log("Canvas render mode changed to World Space");
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.worldCamera = Camera.main;
            }
            
            // 캔버스에 GraphicRaycaster 추가
            if (canvas.GetComponent<GraphicRaycaster>() == null)
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }
        }
        
        originalPosition = transform.position;
        zPosition = transform.position.z;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        isDragging = true;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);
        offset.z = 0;
        
        // 물리 시뮬레이션 일시 중지
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        isDragging = false;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        // UI 드래그 이벤트를 사용하지만, 부모 오브젝트를 이동시킵니다.
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector3 worldPoint);
            
        // z 위치는 유지
        worldPoint.z = zPosition;
        
        // 오프셋 적용
        worldPoint += offset;
        
        // Rigidbody를 사용해 부드럽게 이동
        if (rb != null)
        {
            Vector3 moveDirection = (worldPoint - transform.position) * 10f;
            rb.velocity = new Vector2(moveDirection.x, moveDirection.y);
        }
        else
        {
            transform.position = worldPoint;
        }
    }
    
    // 필요시 원래 위치로 되돌리는 메서드
    public void ResetPosition()
    {
        transform.position = originalPosition;
    }
}
