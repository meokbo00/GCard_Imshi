using UnityEngine;
using DG.Tweening;
using System.Linq;
using System.Collections.Generic;

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
    public bool isInBuyZone = false;
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
            // 구매 영역 안에서 드롭한 경우
            if (isInBuyZone)
            {
                GameManager gameManager = FindObjectOfType<GameManager>();
                // 현재 드래그 중인 오브젝트의 JokerStat 컴포넌트 가져오기
                JokerStat jokerStat = GetComponent<JokerStat>();
                
                if (gameManager != null)
                {
                    if (jokerStat != null)
                    {
                        // 현재 오브젝트의 JokerStat에서 가격 가져오기
                        if (gameManager.money - jokerStat.price >= 0)
                        {
                            gameManager.BuyItem(jokerStat.price);
                            Debug.Log($"구매확정!\n보유 금액: ${gameManager.money}\n아이템 가격: ${jokerStat.price}");
                            
                            // 구매 성공 시 JokerZone으로 이동
                            GameObject jokerZone = GameObject.FindGameObjectWithTag("JokerZone");
                            if (jokerZone != null)
                            {
                                // 드래그 중지 및 위치 고정
                                isDragging = false;
                                isDrag = false;
                                
                                // JokerZone의 자식으로 설정
                                transform.SetParent(jokerZone.transform);
                                
                                // BoxCollider2D 가져오기
                                var boxCollider = jokerZone.GetComponent<BoxCollider2D>();
                                if (boxCollider != null)
                                {
                                    // 태그를 "BuyJoker"로 변경 (먼저 태그를 변경해야 함)
                                    gameObject.tag = "BuyJoker";
                                    
                                    // JokerZone 내의 모든 BuyJoker 태그를 가진 오브젝트 찾기 (자식 오브젝트 중에서)
                                    var buyJokers = new List<GameObject>();
                                    foreach (Transform child in jokerZone.transform)
                                    {
                                        if (child.CompareTag("BuyJoker"))
                                        {
                                            buyJokers.Add(child.gameObject);
                                        }
                                    }
                                    
                                    int jokerCount = buyJokers.Count;
                                    
                                    // BoxCollider2D의 경계 가져오기
                                    Bounds bounds = boxCollider.bounds;
                                    float width = bounds.size.x * 0.8f; // 80%만 사용하도록 조정 (경계선 여유 공간 확보)
                                    float startX = bounds.center.x - (width / 2); // 중앙을 기준으로 좌우로 퍼지도록
                                    
                                    // 아이템이 1개일 때는 가운데, 2개 이상일 때는 균등 간격으로 배치
                                    if (jokerCount == 1)
                                    {
                                        // 1개: 가운데
                                        transform.position = new Vector3(bounds.center.x, bounds.center.y, transform.position.z);
                                    }
                                    else if (jokerCount > 1)
                                    {
                                        // 2개 이상: 균등 간격으로 배치
                                        float spacing = width / (jokerCount - 1);
                                        
                                        // 모든 BuyJoker 오브젝트 재배치
                                        for (int i = 0; i < jokerCount; i++)
                                        {
                                            float xPos = startX + (spacing * i);
                                            buyJokers[i].transform.position = new Vector3(
                                                xPos, 
                                                bounds.center.y, 
                                                buyJokers[i].transform.position.z);
                                        }
                                    }
                                }
                                
                                // 드래그 비활성화
                                enabled = false;
                                return; // 여기서 함수 종료하여 ReturnToDragStartPosition이 실행되지 않도록 함
                            }
                        }
                        else
                        {
                            Debug.Log($"구매실패!\n보유 금액: ${gameManager.money}\n아이템 가격: ${jokerStat.price}");
                        }
                    }
                    else
                    {
                        Debug.Log("JokerStat 컴포넌트를 찾을 수 없습니다!");
                    }
                }
                else
                {
                    Debug.Log("GameManager를 찾을 수 없습니다!");
                }
            }
            
            // 드래그 중이었다면 드래그 시작 위치로 부드럽게 복귀
            ReturnToDragStartPosition();
        }
        isDragging = false;
        isDrag = false;
        isInBuyZone = false; // 구매 영역 상태 초기화
        currentTag = "None";
        if (PriceTag != null) PriceTag.SetActive(true);
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
