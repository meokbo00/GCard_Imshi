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
    private int originalSiblingIndex; // 원래 위치 인덱스
    private Transform parentTransform; // 부모 트랜스폼 캐시
    private List<Transform> siblingItems = new List<Transform>(); // 형제 아이템 목록

    public GameObject PriceTag;
    public GameObject ExplainBox;
    public bool isDrag = false;
    public bool isInBuyZone = false;
    public bool isInSellZone = false; // 판매 영역 안에 있는지 여부
    public string currentTag = "None";
    public bool isPurchased = false; // 구매 여부 추적

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
        // ExplainBox 표시
        if (ExplainBox != null)
        {
            ExplainBox.SetActive(true);
        }

        // JokerZone 레이어를 무시하도록 설정
        int jokerZoneLayer = LayerMask.NameToLayer("JokerZone");
        int originalLayer = gameObject.layer;
    
        if (jokerZoneLayer != -1)
        {
            // JokerZone 레이어를 잠시 무시
            Physics2D.IgnoreLayerCollision(jokerZoneLayer, originalLayer, true);
        }

        
        returnTween?.Kill();
        dragStartWorldPosition = transform.position;
        dragStartPosition = Input.mousePosition;
        isDragging = false;
        isDrag = true;
        currentTag = gameObject.tag;
        Debug.Log($"드래그 시작 - 태그: {currentTag}");
        
        // 부모와 형제 아이템 정보 저장
        parentTransform = transform.parent;
        if (parentTransform != null)
        {
            siblingItems.Clear();
            foreach (Transform child in parentTransform)
            {
                if (child.CompareTag("BuyJoker") || child.CompareTag("BuyTaro") || child.CompareTag("BuyPlanet"))
                {
                    siblingItems.Add(child);
                }
            }
            // x 좌표 기준으로 정렬
            siblingItems.Sort((a, b) => a.position.x.CompareTo(b.position.x));
            originalSiblingIndex = siblingItems.IndexOf(transform);
        }
        
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
        
        // 구매 상태에서만 아이템 재정렬 로직 실행
        if (isPurchased && parentTransform != null && (gameObject.CompareTag("BuyJoker") || gameObject.CompareTag("BuyTaro") || gameObject.CompareTag("BuyPlanet")))
        {
            UpdateSiblingPositions();
        }
    }

    // 형제 아이템들의 위치 업데이트
    private void UpdateSiblingPositions()
    {
        if (siblingItems.Count <= 1) return;
        
        // 현재 드래그 중인 아이템의 x 위치
        float currentX = transform.position.x;
        
        // 원래 위치에서의 x 위치
        float originalX = dragStartWorldPosition.x;
        
        // 현재 위치가 원래 위치보다 오른쪽에 있는지 확인
        bool movingRight = currentX > originalX;
        
        // 이동 방향에 따라 처리
        if (movingRight && originalSiblingIndex < siblingItems.Count - 1)
        {
            // 오른쪽으로 드래그 중일 때
            Transform rightSibling = siblingItems[originalSiblingIndex + 1];
            if (currentX > rightSibling.position.x)
            {
                // 오른쪽 아이템과 위치 교환 (부드럽게 이동)
                Vector3 tempPos = rightSibling.position;
                Vector3 newRightPos = new Vector3(originalX, tempPos.y, tempPos.z);
                
                // 오른쪽 아이템을 왼쪽으로 부드럽게 이동
                SmoothMoveItem(rightSibling, newRightPos);
                
                // 인덱스 업데이트
                siblingItems[originalSiblingIndex] = rightSibling;
                siblingItems[originalSiblingIndex + 1] = transform;
                originalSiblingIndex++;
                
                // 드래그 시작 위치만 업데이트 (드래그 중인 카드는 마우스 따라 움직이므로 위치 업데이트 제거)
                dragStartWorldPosition = new Vector3(tempPos.x, dragStartWorldPosition.y, dragStartWorldPosition.z);
                
                // 레이어 순서 업데이트
                UpdateSiblingLayers();
            }
        }
        else if (!movingRight && originalSiblingIndex > 0)
        {
            // 왼쪽으로 드래그 중일 때
            Transform leftSibling = siblingItems[originalSiblingIndex - 1];
            if (currentX < leftSibling.position.x)
            {
                // 왼쪽 아이템과 위치 교환 (부드럽게 이동)
                Vector3 tempPos = leftSibling.position;
                Vector3 newLeftPos = new Vector3(originalX, tempPos.y, tempPos.z);
                
                // 왼쪽 아이템을 오른쪽으로 부드럽게 이동
                SmoothMoveItem(leftSibling, newLeftPos);
                
                // 인덱스 업데이트
                siblingItems[originalSiblingIndex] = leftSibling;
                siblingItems[originalSiblingIndex - 1] = transform;
                originalSiblingIndex--;
                
                // 드래그 시작 위치만 업데이트 (드래그 중인 카드는 마우스 따라 움직이므로 위치 업데이트 제거)
                dragStartWorldPosition = new Vector3(tempPos.x, dragStartWorldPosition.y, dragStartWorldPosition.z);
                
                // 레이어 순서 업데이트
                UpdateSiblingLayers();
            }
        }
    }
    
    // 형제 아이템들의 레이어 순서를 -1, -2, -3...으로 정렬
    private void UpdateSiblingLayers()
    {
        for (int i = 0; i < siblingItems.Count; i++)
        {
            var spriteRenderer = siblingItems[i].GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = -(i + 1); // -1, -2, -3... 순서로 정렬
            }
        }
    }
    
    // 아이템을 부드럽게 이동시키는 메서드
    private void SmoothMoveItem(Transform item, Vector3 targetPosition, float duration = 0.2f)
    {
        if (item == null) return;
        
        // 해당 오브젝트의 모든 트윈 종료
        item.DOKill();
        
        // 새로운 트윈 생성 (부드러운 이동)
        item.DOMove(targetPosition, duration)
             .SetEase(Ease.OutQuad)
             .SetLink(item.gameObject);
    }
    
    private void OnMouseUp()
    {
        if (ExplainBox != null)
        {
            ExplainBox.SetActive(false);
        }
        if (!isDragging) return;
        
        // 판매 영역 안에서 놓은 경우
        if (isInSellZone && (gameObject.CompareTag("BuyJoker") || gameObject.CompareTag("BuyTaro") || gameObject.CompareTag("BuyPlanet")))
        {
            // SellZone 컴포넌트 찾기
            var sellZone = FindObjectOfType<SellZone>();
            if (sellZone != null)
            {
                sellZone.ProcessSell(gameObject);
                // 판매 후 아이템은 삭제되므로 여기서 리턴
                return;
            }
        }
        // 구매 상태에서 드롭 시 새 위치로 이동
        else if (isPurchased && (gameObject.CompareTag("BuyJoker") || gameObject.CompareTag("BuyTaro") || gameObject.CompareTag("BuyPlanet")))
        {
            // 새로운 위치로 부드럽게 이동
            returnTween?.Kill();
            returnTween = transform.DOMove(dragStartWorldPosition, returnDuration)
                .SetEase(returnEase)
                .OnComplete(() => {
                    returnTween = null;
                    // 드래그 종료 후 최종 레이어 순서 업데이트
                    UpdateSiblingLayers();
                });
                
            ResetDragState();
            return;
        }

        // 구매 영역 안에서 드롭한 경우
        if (isInBuyZone)
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            GameSaveData gameSaveData = FindObjectOfType<GameSaveData>();
            
            // Joker, Taro, Planet 태그별로 처리
            if (gameObject.CompareTag("Joker") || gameObject.CompareTag("Taro") || gameObject.CompareTag("Planet"))
            {
                int price = 0;
                string zoneTag = "";
                string buyTag = "";
                bool useItemZone = false;
                
                // 태그에 따라 적절한 Stat 컴포넌트와 존 태그 설정
                if (gameObject.CompareTag("Joker"))
                {
                    JokerStat stat = GetComponent<JokerStat>();
                    if (stat != null) price = stat.price;
                    zoneTag = "JokerZone";
                    buyTag = "BuyJoker";
                }
                else if (gameObject.CompareTag("Taro"))
                {
                    TaroStat stat = GetComponent<TaroStat>();
                    if (stat != null) price = stat.price;
                    zoneTag = "ItemZone";
                    buyTag = "BuyTaro";
                    useItemZone = true;
                }
                else if (gameObject.CompareTag("Planet"))
                {
                    PlanetStat stat = GetComponent<PlanetStat>();
                    if (stat != null) price = stat.price;
                    zoneTag = "ItemZone";
                    buyTag = "BuyPlanet";
                    useItemZone = true;
                }
                
                // 구매 전에 아이템 존의 아이템 개수 확인
                GameObject targetZone = GameObject.FindGameObjectWithTag(zoneTag);
                if (targetZone != null)
                {
                    // 아이템 존을 사용하는 경우 (Taro, Planet)
                    if (useItemZone)
                    {
                        // ItemZone 내의 BuyPlanet과 BuyTaro 태그를 가진 오브젝트 찾기
                        var items = new List<GameObject>();
                        foreach (Transform child in targetZone.transform)
                        {
                            if (child.CompareTag("BuyPlanet") || child.CompareTag("BuyTaro"))
                            {
                                items.Add(child.gameObject);
                            }
                        }
                        
                        // 최대 2개까지만 허용
                        if (items.Count >= 2)
                        {
                            ReturnToDragStartPosition();
                            ResetDragState();
                            return;
                        }
                    }
                }
                
                // 아이템 구매 처리
                if (gameManager != null && price > 0 && gameManager.playerData.money - price >= 0)
                {
                    gameManager.BuyItem(price);
                        
                    // 아이템 타입에 따른 구매 성공 메시지 출력
                    string itemType = useItemZone ? (gameObject.CompareTag("Taro") ? "타로" : "행성") : "조커";
                    
                    // 구매 성공 시 해당 Zone으로 이동
                    if (targetZone != null)
                    {
                        isDragging = false;
                        isDrag = false;
                        
                        // 아이템 존을 사용하는 경우 (Taro, Planet)
                        if (useItemZone)
                        {
                            // 이미 위에서 items 리스트를 만들었으니 다시 만들지 않음
                            var items = new List<GameObject>();
                            foreach (Transform child in targetZone.transform)
                            {
                                if (child.CompareTag("BuyPlanet") || child.CompareTag("BuyTaro"))
                                {
                                    items.Add(child.gameObject);
                                }
                            }
                            
                            // 부모를 targetZone으로 설정하고 태그 변경
                            transform.SetParent(targetZone.transform);
                            gameObject.tag = buyTag;
                            items.Add(gameObject);
                            
                            var boxCollider = targetZone.GetComponent<BoxCollider2D>();
                            if (boxCollider != null)
                            {
                                Bounds bounds = boxCollider.bounds;
                                float totalWidth = bounds.size.x * 0.9f; // 전체 너비 (90% 사용)
                                float slotWidth = totalWidth / 5f; // 5개의 슬롯으로 나눔
                                float startX = bounds.center.x - (totalWidth / 2) + (slotWidth / 2); // 첫 번째 슬롯의 x 위치
                                
                                // 아이템 개수에 따라 위치 계산 및 레이어 설정
                                if (items.Count == 1)
                                {
                                    // 1개: 가운데 (3번째 슬롯)
                                    float xPos = startX + (slotWidth * 2);
                                    Vector3 targetPos = new Vector3(
                                        xPos, 
                                        bounds.center.y, 
                                        transform.position.z);
                                    
                                    // SpriteRenderer의 sortingOrder 설정
                                    var spriteRenderer = GetComponent<SpriteRenderer>();
                                    if (spriteRenderer != null)
                                    {
                                        spriteRenderer.sortingOrder = -1; // 첫 번째 아이템은 항상 -1
                                    }
                                    
                                    // 구매 로그 메시지 출력 (Taro/Planet)
                                    int slotNum = 3; // 1개일 때는 항상 3번 슬롯(가운데)
                                    //Debug.Log($"{itemType} 아이템 구매 성공! - 프리팹 이름: {gameObject.name}, 배정된 슬롯: {slotNum}번");
                                    
                                    // 부드럽게 이동 후 구매 확정 처리
                                    transform.DOMove(targetPos, 0.3f).SetEase(Ease.OutBack).OnComplete(() => {
                                        isPurchased = true;
                                        if (PriceTag != null) PriceTag.SetActive(false);
                                        enabled = false;
                                        ResetDragState();
                                    });
                                    return;
                                }
                                else if (items.Count == 2)
                                {
                                    // 2개: 2번째와 4번째 슬롯에 배치 및 레이어 설정
                                    for (int i = 0; i < items.Count; i++)
                                    {
                                        // 첫 번째 아이템은 2번째 슬롯, 두 번째 아이템은 4번째 슬롯
                                        int slotIndex = (i == 0) ? 1 : 3;
                                        float xPos = startX + (slotWidth * slotIndex);
                                        
                                        // 부드럽게 이동
                                        items[i].transform.DOMove(
                                            new Vector3(xPos, bounds.center.y, items[i].transform.position.z),
                                            0.3f).SetEase(Ease.OutBack);
                                        
                                        // SpriteRenderer의 sortingOrder 설정
                                        var spriteRenderer = items[i].GetComponent<SpriteRenderer>();
                                        if (spriteRenderer != null)
                                        {
                                            spriteRenderer.sortingOrder = -(i + 1); // -1, -2 순서로 정렬
                                        }
                                    }
                                }
                            }
                        }
                        // 조커 존을 사용하는 경우 (Joker)
                        else
                        {
                            transform.SetParent(targetZone.transform);
                            gameObject.tag = buyTag;
                            
                            var boxCollider = targetZone.GetComponent<BoxCollider2D>();
                            if (boxCollider != null)
                            {
                                var buyJokers = new List<GameObject>();
                                foreach (Transform child in targetZone.transform)
                                {
                                    if (child.CompareTag(buyTag))
                                    {
                                        buyJokers.Add(child.gameObject);
                                    }
                                }
                                
                                int jokerCount = buyJokers.Count;
                                Bounds bounds = boxCollider.bounds;
                                float width = bounds.size.x * 0.8f;
                                float startX = bounds.center.x - (width / 2);
                                
                                // 위치 정렬 및 레이어 설정
                                if (jokerCount == 1)
                                {
                                    // 부드럽게 이동
                                    transform.DOMove(
                                        new Vector3(bounds.center.x, bounds.center.y, transform.position.z),
                                        0.3f).SetEase(Ease.OutBack);
                                    
                                    // SpriteRenderer의 sortingOrder 설정
                                    var spriteRenderer = GetComponent<SpriteRenderer>();
                                    if (spriteRenderer != null)
                                    {
                                        spriteRenderer.sortingOrder = -1; // 첫 번째 아이템은 항상 -1
                                    }
                                }
                                else if (jokerCount > 1)
                                {
                                    float spacing = width / (jokerCount - 1);
                                    for (int i = 0; i < jokerCount; i++)
                                    {
                                        float xPos = startX + (spacing * i);
                                        // 부드럽게 이동
                                        buyJokers[i].transform.DOMove(
                                            new Vector3(xPos, bounds.center.y, buyJokers[i].transform.position.z),
                                            0.3f).SetEase(Ease.OutBack);
                                                
                                        // SpriteRenderer의 sortingOrder 설정 (왼쪽이 위로 오도록)
                                        var spriteRenderer = buyJokers[i].GetComponent<SpriteRenderer>();
                                        if (spriteRenderer != null)
                                        {
                                            spriteRenderer.sortingOrder = -(i + 1); // -1, -2, -3... 순서로 정렬
                                        }
                                    }
                                }
                            }
                        }
                        
                        // 구매 확정 처리
                        int slotNumber = 1; // 기본값 (1개일 때)
                        
                        // 존에서의 슬롯 위치 계산
                        if (targetZone != null)
                        {
                            string[] tagsToCheck;
                            
                            if (!useItemZone)
                            {
                                // 조커 아이템인 경우
                                tagsToCheck = new string[] { "BuyJoker" };
                            }
                            else
                            {
                                // 타로 또는 행성 아이템인 경우
                                tagsToCheck = new string[] { "BuyTaro", "BuyPlanet" };
                            }
                            
                            // 해당 태그들을 가진 모든 오브젝트 찾기
                            var itemsInZone = new List<GameObject>();
                            foreach (var tag in tagsToCheck)
                            {
                                itemsInZone.AddRange(GameObject.FindGameObjectsWithTag(tag));
                            }
                            
                            if (itemsInZone.Count > 1)
                            {
                                // x 좌표를 기준으로 정렬
                                var sortedItems = itemsInZone.OrderBy(item => item.transform.position.x).ToList();
                                int index = sortedItems.FindIndex(item => item == gameObject);
                                if (index >= 0)
                                {
                                    slotNumber = index + 1; // 1-based 인덱스
                                }
                            }
                        }
                        
                        Debug.Log($"{itemType} 아이템 구매 성공! - 프리팹 이름: {gameObject.name}, 배정된 슬롯: {slotNumber}번");
                        ItemData itemData = FindObjectOfType<ItemData>();
                        itemData.SaveObjectData(gameObject.name);
                        isPurchased = true;
                        if (PriceTag != null) PriceTag.SetActive(false);
                        
                        enabled = false;
                        ResetDragState();
                        return;
                    }
                }
            }
        }
        
        // 구매 영역 밖이거나 구매 조건에 맞지 않는 경우 원래 위치로 복귀
        ReturnToDragStartPosition();
        ResetDragState();
    }
    
    private void ResetDragState()
    {
        isDragging = false;
        isDrag = false;
        isInBuyZone = false;
        isInSellZone = false;
        currentTag = gameObject.tag; // 현재 태그 유지
        // 구매되지 않은 경우에만 PriceTag 활성화
        if (PriceTag != null && !isPurchased)
        {
            PriceTag.SetActive(true);
        }
        
        // 형제 아이템 목록 초기화
        siblingItems.Clear();
        originalSiblingIndex = -1;
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
