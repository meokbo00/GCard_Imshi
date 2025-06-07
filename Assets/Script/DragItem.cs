using UnityEngine;
using DG.Tweening;

public class DragItem : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCamera;
    private bool isDragging = false;
    private float dragThreshold = 0.1f; // 드래그 시작을 위한 최소 거리
    private Vector3 dragStartPosition;
    private Vector3 dragStartWorldPosition; // 드래그 시작 시 월드 위치
    private Tween returnTween; // 현재 진행 중인 트윈 참조

    public GameObject PriceTag;
    public bool isDrag = false;
    public string currentTag = "None";

    [Header("Tween Settings")]
    [SerializeField] private float returnDuration = 0.3f; // 복귀 애니메이션 지속 시간
    [SerializeField] private Ease returnEase = Ease.OutBack; // 복귀 애니메이션 이징
    [SerializeField] private float dragSmoothing = 0.1f; // 드래그 시 부드러운 움직임 강도
    
    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnMouseDown()
    {
        returnTween?.Kill();
        
        dragStartWorldPosition = transform.position;
        dragStartPosition = Input.mousePosition;
        isDragging = false;
        isDrag = true;
        currentTag = gameObject.tag;
        Debug.Log($"드래그 시작 - 태그: {currentTag}");
        
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 
            transform.position.z - mainCamera.transform.position.z));
        offset = transform.position - mouseWorldPos;
    }

    private void OnMouseDrag()
    {
        // 드래그 시작 확인
        if (!isDragging)
        {
            // 일정 거리 이상 움직였을 때 드래그 시작
            if (Vector3.Distance(dragStartPosition, Input.mousePosition) > dragThreshold)
            {
                isDragging = true;
                PriceTag.SetActive(false);
            }
            return; // 드래그 시작 전에는 아무 동작도 하지 않음
        }
        
        // 부드러운 드래그를 위해 DOTween 사용
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 
            transform.position.z - mainCamera.transform.position.z)) + offset;
            
        // 즉시 위치 업데이트 (부드러운 움직임 제거)
        transform.position = targetPosition;
    }

    private void OnMouseUp()
    {
        if (isDragging)
        {
            // 드래그 중이었다면 드래그 시작 위치로 부드럽게 복귀
            ReturnToDragStartPosition();
        }
        isDragging = false;
        isDrag = false;
        currentTag = "None";
        PriceTag.SetActive(true);
        Debug.Log("드래그 종료 - 태그 초기화");
    }
    
    private void ReturnToDragStartPosition()
    {
        // 기존 트윈이 있다면 중지
        returnTween?.Kill();
        
        // DOTween을 사용해 드래그 시작 위치로 부드럽게 복귀
        returnTween = transform.DOMove(dragStartWorldPosition, returnDuration)
            .SetEase(returnEase)
            .OnComplete(() => returnTween = null);
    }
}
