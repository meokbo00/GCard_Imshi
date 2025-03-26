using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Card : MonoBehaviour
{
    public enum Suit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }

    public enum Rank
    {
        Ace = 1, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8,
        Nine = 9, Ten = 10, Jack = 11, Queen = 12, King = 13
    }

    public Suit suit;
    public Rank rank;
    public bool isFaceUp = true;
    public bool isSelected = false;
    public bool isJoker = false;

    private SpriteRenderer spriteRenderer;
    public Vector3 originalPosition;
    private Vector3 hoverPosition;
    private bool isAnimating = false;
    private Coroutine currentAnimation;
    private int originalSortingOrder; // 원래의 정렬 순서 저장

    // 애니메이션 설정
    [Header("Animation Settings")]
    public float hoverHeight = 0.3f; // 호버 시 올라가는 높이
    public float animationDuration = 0.2f; // 애니메이션 지속 시간
    private float selectedHeight = 0.5f; // 선택 시 올라가는 높이
    private Vector3 selectedPosition;

    // 카드 이미지 관리
    [Header("Card Sprites")]
    public Sprite cardBackSprite;
    private Sprite cardFrontSprite;
    private static Dictionary<string, Sprite> cardSprites = new Dictionary<string, Sprite>();

    private Vector3 originalScale;
    private Vector3 hoverScale;

    private bool isMovingToOriginal = false; // 원래 위치로 이동 중인지 확인하는 플래그
    private bool isMovingToSelected = false; // 선택된 위치로 이동 중인지 확인하는 플래그

    private DeckManager deckManager;

    private bool isMouseOver = false; // 마우스가 카드 위에 있는지 확인하는 변수 추가

    private bool isDragging = false; // 드래그 중인지 확인하는 변수
    private float dragOffset; // 드래그 시작 시 카드와 마우스 포인트의 x축 차이
    private Camera mainCamera; // 메인 카메라 참조
    private Vector3 mouseDownPosition; // 마우스 다운 시의 위치
    private float dragThreshold = 0.5f; // 드래그로 인정할 최소 이동 거리
    private bool isDragStarted = false; // 드래그가 시작되었는지 확인하는 변수
    private float cardWidth = 1.3f; // 카드의 너비 (스케일 고려)
    private float swapThreshold = 0.2f; // 카드 위치 교환을 위한 임계값
    private Vector3 lastMovedCardPosition; // 마지막으로 이동한 카드의 원래 위치
    private float lastMoveTime; // 마지막 이동 시간을 저장
    private const float DIRECTION_CHANGE_DELAY = 0.35f; // 방향 전환 딜레이
    private int lastMoveDirection = 0; // 마지막 이동 방향 (1: 오른쪽, -1: 왼쪽)
    private Vector3 lastMousePosition; // 이전 프레임의 마우스 위치

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = new Vector3(2f, 2f, 2f); // 기본 크기를 2로 설정
        hoverScale = new Vector3(2.3f, 2.3f, 2.3f); // 호버 시 크기를 2.3으로 설정
        transform.localScale = originalScale;
        
        // DeckManager 찾기
        deckManager = FindObjectOfType<DeckManager>();
        mainCamera = Camera.main;
        
        // 카드 이미지 초기화
        if (cardSprites.Count == 0)
        {
            LoadCardSprites();
        }

        // 원래의 정렬 순서 저장
        originalSortingOrder = spriteRenderer.sortingOrder;
    }

    private void LoadCardSprites()
    {
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Cards");
        foreach (Sprite sprite in allSprites)
        {
            cardSprites[sprite.name] = sprite;
        }
    }

    void OnMouseEnter()
    {
        if (!isAnimating && !isSelected)
        {
            StopCurrentAnimation();
            currentAnimation = StartCoroutine(HoverAnimation(true));
        }
        isMouseOver = true; // 마우스가 카드 위에 있음을 표시
    }

    void OnMouseExit()
    {
        if (!isMovingToOriginal && !isMovingToSelected)
        {
            StopCurrentAnimation();
            currentAnimation = StartCoroutine(HoverAnimation(false));
        }
        else if (isSelected)
        {
            transform.localScale = originalScale;
        }
        isMouseOver = false; // 마우스가 카드 위에 없음을 표시
    }

    void OnMouseDown()
    {
        if (!isMouseOver) return;
        
        isDragging = false;
        isDragStarted = false;
        mouseDownPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        StopCurrentAnimation();
        
        // 클릭 시 가장 위 레이어로 이동
        spriteRenderer.sortingOrder = 10;
    }

    void OnMouseDrag()
    {
        if (!isDragStarted && isMouseOver)
        {
            Vector3 currentMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            float dragDistance = Mathf.Abs(currentMousePosition.x - mouseDownPosition.x);

            // 드래그 거리가 임계값을 넘으면 드래그 시작
            if (dragDistance > dragThreshold)
            {
                isDragStarted = true;
                isDragging = true;
                dragOffset = transform.position.x - currentMousePosition.x;
            }
        }

        if (isDragging)
        {
            // 마우스 위치를 월드 좌표로 변환하고 x축만 업데이트
            Vector3 currentMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(currentMousePosition.x + dragOffset, transform.position.y, transform.position.z);

            // 드래그 중인 카드의 양쪽 끝 위치 계산
            float rightEdge = transform.position.x + (cardWidth * transform.localScale.x / 2);
            float leftEdge = transform.position.x - (cardWidth * transform.localScale.x / 2);
            
            // 모든 카드를 확인하여 위치 교환 체크
            foreach (Card card in deckManager.GetHand())
            {
                if (card != null && card != this)
                {
                    float cardCenter = card.originalPosition.x;
                    
                    // 오른쪽으로 드래그하는 경우
                    if (card.originalPosition.x > originalPosition.x)
                    {
                        float distanceToSwap = rightEdge - cardCenter;
                        if (distanceToSwap >= swapThreshold)
                        {
                            // 방향이 바뀌었을 때만 딜레이 체크
                            if (lastMoveDirection == -1)
                            {
                                float timeSinceLastMove = Time.time - lastMoveTime;
                                if (timeSinceLastMove < DIRECTION_CHANGE_DELAY)
                                {
                                    continue;
                                }
                            }

                            // 이동하기 전 카드의 원래 위치 저장
                            lastMovedCardPosition = card.originalPosition;
                            // 위치 교환
                            deckManager.MoveCardLeft(card);
                            // 드래그 중인 카드의 originalPosition 업데이트
                            originalPosition = new Vector3(card.originalPosition.x + 1.3f, originalPosition.y, originalPosition.z);
                            lastMoveDirection = 1;
                            lastMoveTime = Time.time;
                            break;
                        }
                    }
                    // 왼쪽으로 드래그하는 경우
                    else if (card.originalPosition.x < originalPosition.x)
                    {
                        float distanceToSwap = cardCenter - leftEdge;
                        if (distanceToSwap >= swapThreshold)
                        {
                            // 방향이 바뀌었을 때만 딜레이 체크
                            if (lastMoveDirection == 1)
                            {
                                float timeSinceLastMove = Time.time - lastMoveTime;
                                if (timeSinceLastMove < DIRECTION_CHANGE_DELAY)
                                {
                                    continue;
                                }
                            }

                            // 이동하기 전 카드의 원래 위치 저장
                            lastMovedCardPosition = card.originalPosition;
                            // 위치 교환
                            deckManager.MoveCardRight(card);
                            // 드래그 중인 카드의 originalPosition 업데이트
                            originalPosition = new Vector3(card.originalPosition.x - 1.3f, originalPosition.y, originalPosition.z);
                            lastMoveDirection = -1;
                            lastMoveTime = Time.time;
                            break;
                        }
                    }
                }
            }
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;
            isDragStarted = false;

            // 마지막으로 이동한 카드의 위치로 이동
            if (lastMovedCardPosition != Vector3.zero)
            {
                // 선택 상태에 따라 y축 위치 결정
                float targetY = isSelected ? -3f : -3.5f;
                lastMovedCardPosition.y = targetY;
                transform.position = lastMovedCardPosition;
                originalPosition = lastMovedCardPosition;
                lastMovedCardPosition = Vector3.zero; // 위치 정보 초기화
            }
            else
            {
                // 이동한 카드가 없으면 원래 위치로 돌아가기
                // 선택 상태에 따라 y축 위치 결정
                float targetY = isSelected ? -3f : -3.5f;
                originalPosition.y = targetY;
                transform.position = originalPosition;
            }
        }
        else if (isMouseOver) // 클릭 이벤트 처리
        {
            // 클릭이었다면 선택/해제 처리
            if (!isSelected)
            {
                if (!deckManager.CanSelectCard())
                {
                    return;
                }
                deckManager.AddSelectedCard(this);
            }
            else
            {
                deckManager.RemoveSelectedCard(this);
            }

            isSelected = !isSelected;
            StopCurrentAnimation();

            if (isSelected)
            {
                isMovingToOriginal = false;
                isMovingToSelected = true;
                selectedPosition = originalPosition;
                selectedPosition.y = -3f;
                currentAnimation = StartCoroutine(MoveToPosition(selectedPosition));
            }
            else
            {
                isMovingToOriginal = true;
                isMovingToSelected = false;
                Vector3 targetPosition = originalPosition;
                targetPosition.y = -3.5f;
                currentAnimation = StartCoroutine(MoveToPosition(targetPosition));
            }
        }

        // 원래의 정렬 순서로 복원
        spriteRenderer.sortingOrder = originalSortingOrder;
    }

    private void StopCurrentAnimation()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
    }

    private IEnumerator HoverAnimation(bool hover)
    {
        isAnimating = true;
        float elapsedTime = 0;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = hover ? hoverScale : originalScale;
        
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            t = Mathf.SmoothStep(0, 1, t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;
        isAnimating = false;
        currentAnimation = null;
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        isAnimating = true;
        float elapsedTime = 0;
        Vector3 startPosition = transform.position;
        
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            t = Mathf.SmoothStep(0, 1, t);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
        isAnimating = false;
        currentAnimation = null;
        isMovingToOriginal = false;
        isMovingToSelected = false;
    }

    public void Initialize(Suit newSuit, Rank newRank)
    {
        suit = newSuit;
        rank = newRank;
        isFaceUp = true;
        isSelected = false;
        isJoker = false;
        UpdateCardVisual();
    }

    public void SetOriginalPosition(Vector3 position)
    {
        originalPosition = position;
        selectedPosition = originalPosition + Vector3.up * selectedHeight;
        transform.position = originalPosition;
    }

    private void UpdateCardVisual()
    {
        if (spriteRenderer != null)
        {
            if (isFaceUp)
            {
                string spriteName = GetCardSpriteName();
                if (cardSprites.TryGetValue(spriteName, out Sprite frontSprite))
                {
                    cardFrontSprite = frontSprite;
                    spriteRenderer.sprite = cardFrontSprite;
                }
                else
                {
                    Debug.LogError($"카드 스프라이트를 찾을 수 없습니다: {spriteName}");
                }
            }
            else
            {
                spriteRenderer.sprite = cardBackSprite;
            }

            if (isSelected)
            {
                spriteRenderer.color = new Color(1f, 1f, 0.5f);
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
        }
    }

    private string GetCardSpriteName()
    {
        if (isJoker)
            return "joker";

        string suitName = suit.ToString().ToLower();
        string rankName = rank.ToString().ToLower();
        return $"{rankName}_of_{suitName}";
    }

    public string GetCardName()
    {
        if (isJoker)
            return "Joker";
        return $"{rank} of {suit}";
    }
}