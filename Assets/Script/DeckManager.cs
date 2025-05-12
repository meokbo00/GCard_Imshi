using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DeckManager : MonoBehaviour
{
    public GameObject cardPrefab;
    GameOverManager gameovermanager;

    public Canvas canvas; // UI 캔버스 참조 추가
    public HandRanking handRanking; // HandRanking 참조 추가
    private List<Card> deck = new List<Card>();
    private List<Card> discardPile = new List<Card>();
    private List<Card> hand = new List<Card>();
    private Vector3 cardScale = new Vector3(2f, 2f, 2f); // 카드 스케일 설정
    private List<Card> selectedCards = new List<Card>();
    private const int MAX_SELECTED_CARDS = 5;

    void Start()
    {
        gameovermanager = FindObjectOfType<GameOverManager>();
        InitializeDeck();
        ShuffleDeck();
        DealInitialCards(8); // 8장의 카드를 뽑아서 배치
    }

    private void InitializeDeck()
    {
        // 52장의 카드 생성
        foreach (Card.Suit suit in System.Enum.GetValues(typeof(Card.Suit)))
        {
            foreach (Card.Rank rank in System.Enum.GetValues(typeof(Card.Rank)))
            {
                GameObject cardObj = Instantiate(cardPrefab);
                Card card = cardObj.GetComponent<Card>();
                card.Initialize(suit, rank);

                // 카드의 초기 위치와 스케일 설정
                cardObj.transform.position = new Vector3(0, 0, 0);
                // DOTween을 사용하여 카드 스케일 설정 (팝업 효과)
                cardObj.transform.localScale = Vector3.zero;
                cardObj.transform.DOScale(cardScale, 0.3f)
                    .SetEase(Ease.OutBack);

                // BoxCollider2D 크기 자동 조정
                BoxCollider2D boxCollider = cardObj.GetComponent<BoxCollider2D>();
                if (boxCollider != null)
                {
                    SpriteRenderer spriteRenderer = cardObj.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null && spriteRenderer.sprite != null)
                    {
                        boxCollider.size = spriteRenderer.sprite.bounds.size;
                    }
                }

                cardObj.SetActive(false); // 초기에는 비활성화
                deck.Add(card);
            }
        }
    }

    public void ShuffleDeck()
    {
        // Fisher-Yates 셔플 알고리즘
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Card temp = deck[i];
            deck[i] = deck[j];
            deck[j] = temp;
        }
    }

    public Card DrawCard()
    {
        if (deck.Count == 0)
        {
            ReshuffleDiscardPile();
        }

        if (deck.Count > 0)
        {
            // 사용 가능한 카드들 중에서만 선택
            List<Card> availableCards = deck.FindAll(card => !card.gameObject.activeSelf);
            if (availableCards.Count == 0)
            {
                return null;
            }

            // 사용 가능한 카드들 중에서 랜덤하게 하나 선택
            int randomIndex = Random.Range(0, availableCards.Count);
            Card drawnCard = availableCards[randomIndex];
            deck.Remove(drawnCard);
            hand.Add(drawnCard);

            // 카드를 활성화하고 초기 위치 설정
            drawnCard.gameObject.SetActive(true);
            drawnCard.transform.position = new Vector3(0, 0, 0);
            // DOTween을 사용하여 카드 스케일 설정 (팝업 효과)
            drawnCard.transform.localScale = Vector3.zero;
            drawnCard.transform.DOScale(cardScale, 0.3f)
                .SetEase(Ease.OutBack);

            // BoxCollider2D 크기 재조정
            BoxCollider2D boxCollider = drawnCard.GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                SpriteRenderer spriteRenderer = drawnCard.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.sprite != null)
                {
                    boxCollider.size = spriteRenderer.sprite.bounds.size;
                }
            }

            return drawnCard;
        }

        return null;
    }

    public void DiscardCard(Card card)
    {
        if (hand.Contains(card))
        {
            hand.Remove(card);
            discardPile.Add(card);
            card.gameObject.SetActive(false); // 버린 카드는 비활성화
        }
    }

    private void ReshuffleDiscardPile()
    {
        deck.AddRange(discardPile);
        discardPile.Clear();
        ShuffleDeck();
    }

    public List<Card> GetHand()
    {
        return hand;
    }

    public int GetDeckCount()
    {
        return deck.Count;
    }

    public int GetDiscardPileCount()
    {
        return discardPile.Count;
    }

    public void ClearHand()
    {
        hand.Clear();
    }

    public void AddJoker()
    {
        GameObject cardObj = Instantiate(cardPrefab);
        Card joker = cardObj.GetComponent<Card>();
        joker.isJoker = true;
        deck.Add(joker);
    }

    private void DealInitialCards(int count)
    {
        Vector3 startPosition = new Vector3(-4f, -2.5f, 0f);
        float xOffset = 1.3f; // x축 간격
        float maxX = 10f; // 시작 위치 (오른쪽)

        // 먼저 모든 카드를 오른쪽에 배치
        for (int i = 0; i < count; i++)
        {
            Card drawnCard = DrawCard();
            if (drawnCard != null)
            {
                // 카드를 오른쪽에 배치
                Vector3 cardPosition = new Vector3(maxX, -2.5f, 0f);
                drawnCard.transform.position = cardPosition;
                drawnCard.SetOriginalPosition(cardPosition);
            }
        }

        // 카드들을 왼쪽으로 이동
        StartCoroutine(MoveInitialCardsCoroutine(count, startPosition, xOffset));
    }

    private IEnumerator MoveInitialCardsCoroutine(int count, Vector3 startPosition, float xOffset)
    {
        float moveDuration = 0.5f; // 이동 시간
        float maxX = 10f; // 시작 위치 (오른쪽)

        Dictionary<Card, Vector3> targetPositions = new Dictionary<Card, Vector3>();
        for (int i = 0; i < count; i++)
        {
            Card card = hand[i];
            if (card != null)
            {
                Vector3 targetPos = startPosition + new Vector3(i * xOffset, 0f, 0f);
                targetPositions[card] = targetPos;
                
                // DOTween을 사용하여 카드 이동
                card.transform.DOMove(targetPos, moveDuration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => 
                    {
                        card.SetOriginalPosition(targetPos);
                    });
            }
        }

        // 모든 애니메이션이 완료될 때까지 대기
        yield return new WaitForSeconds(moveDuration);
    }

    public bool CanSelectCard()
    {
        return selectedCards.Count < MAX_SELECTED_CARDS;
    }

    public void AddSelectedCard(Card card)
    {
        if (!selectedCards.Contains(card))
        {
            selectedCards.Add(card);
        }
    }

    public void RemoveSelectedCard(Card card)
    {
        selectedCards.Remove(card);
    }

    public void TrashMove()
    {
        if (selectedCards.Count == 0) return; // 선택된 카드가 없으면 아무것도 하지 않음
        StartCoroutine(MoveCardsRightCoroutine(false)); // false = 점수 업데이트 하지 않음
    }

    public void HandPlay()
    {
        // 선택된 카드가 없으면 아무 동작도 하지 않음
        if (selectedCards.Count == 0) return;

        // 선택된 카드들을 정렬 (x 좌표 기준으로)
        List<Card> sortedCards = new List<Card>(selectedCards);
        sortedCards.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        StartCoroutine(HandPlayCoroutine(sortedCards));
    }

    private IEnumerator HandPlayCoroutine(List<Card> sortedCards)
    {
        // 먼저 모든 카드를 위로 이동
        foreach (Card card in sortedCards)
        {
            if (card != null)
            {
                Vector3 currentPos = card.transform.position;
                Vector3 targetPos = new Vector3(currentPos.x, currentPos.y + 2.5f, currentPos.z);
                
                // DOTween을 사용하여 카드 이동
                card.transform.DOMove(targetPos, 0.3f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => 
                    {
                        card.originalPosition = targetPos;
                    });
            }
        }

        // 모든 카드의 이동이 완료될 때까지 대기
        yield return new WaitForSeconds(0.3f);

        // 0.35초 대기
        yield return new WaitForSeconds(0.35f);

        // 족보를 구성하는 카드들 찾기
        List<Card> rankingCards = new List<Card>();
        if (handRanking != null)
        {
            rankingCards = handRanking.GetRankingCards(sortedCards);
        }

        // 족보를 구성하는 카드들만 포인트 텍스트 표시
        foreach (Card card in sortedCards)
        {
            if (card != null)
            {
                TextMeshProUGUI pointText = card.GetComponentInChildren<TextMeshProUGUI>(true);
                if (pointText != null && rankingCards.Contains(card))  // 족보 구성 카드만 처리
                {
                    // 카드 값에 따른 포인트 계산
                    int point = 0;
                    if (card.rank == Card.Rank.Ace)
                    {
                        point = 11;
                    }
                    else if (card.rank == Card.Rank.King || card.rank == Card.Rank.Queen || card.rank == Card.Rank.Jack)
                    {
                        point = 10;
                    }
                    else
                    {
                        point = (int)card.rank;
                    }

                    pointText.text = "+" + point.ToString();
                    pointText.gameObject.SetActive(true);

                    // 포인트 값만큼 블루칩 증가
                    if (handRanking != null)
                    {
                        handRanking.AddBlueChipValue(point);
                    }

                    // 0.2초 대기 후 텍스트 비활성화
                    yield return new WaitForSeconds(0.2f);
                    pointText.gameObject.SetActive(false);
                }
            }
        }

        // 모든 포인트 텍스트 표시가 끝난 후 오른쪽으로 이동 및 새 카드 생성
        yield return StartCoroutine(MoveCardsRightCoroutine());
    }

    public void Suit()
    {
        Debug.Log("suit 메서드 실행");
        StartCoroutine(SuitCoroutine());
    }

    public void Rank()
    {
        Debug.Log("rank 메서드 실행");
        StartCoroutine(RankCoroutine());
    }

    private IEnumerator SuitCoroutine()
    {
        // 현재 활성화된 카드들을 수집
        List<Card> activeCards = new List<Card>();
        foreach (Card card in hand)
        {
            if (card.gameObject.activeSelf)
            {
                activeCards.Add(card);
            }
        }

        // 카드를 모양에 따라 정렬 (스페이드, 다이아, 하트, 클로버 순서)
        activeCards.Sort((a, b) => {
            // 모양 순서 정의
            Dictionary<Card.Suit, int> suitOrder = new Dictionary<Card.Suit, int>
            {
                { Card.Suit.Spades, 0 },
                { Card.Suit.Diamonds, 1 },
                { Card.Suit.Hearts, 2 },
                { Card.Suit.Clubs, 3 }
            };

            // 모양이 같으면 숫자 순으로 정렬
            if (a.suit == b.suit)
            {
                return a.rank.CompareTo(b.rank);
            }
            return suitOrder[a.suit].CompareTo(suitOrder[b.suit]);
        });

        // 정렬된 카드들의 시작 위치와 목표 위치 저장
        Vector3 startPosition = new Vector3(-4f, -2.5f, 0f);
        float xOffset = 1.3f; // x축 간격
        float moveDuration = 0.2f; // 이동 시간

        // 각 카드의 목표 위치 계산 및 이동
        for (int i = 0; i < activeCards.Count; i++)
        {
            Card card = activeCards[i];
            Vector3 targetPos = startPosition + new Vector3(i * xOffset, 0f, 0f);
            
            // 선택된 카드의 경우 현재 y축 위치 유지
            if (card.isSelected)
            {
                targetPos.y = card.transform.position.y;
            }

            // DOTween을 사용하여 카드 이동
            card.transform.DOMove(targetPos, moveDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => 
                {
                    card.SetOriginalPosition(targetPos);
                });
        }

        // 모든 애니메이션이 완료될 때까지 대기
        yield return new WaitForSeconds(moveDuration);
    }

    private IEnumerator RankCoroutine()
    {
        // 현재 활성화된 카드들을 수집
        List<Card> activeCards = new List<Card>();
        foreach (Card card in hand)
        {
            if (card.gameObject.activeSelf)
            {
                activeCards.Add(card);
            }
        }

        // 카드를 값에 따라 정렬 (A, K, Q, J, 10, 9, 8, 7, 6, 5, 4, 3, 2 순서)
        activeCards.Sort((a, b) => {
            // 값 순서 정의
            Dictionary<Card.Rank, int> rankOrder = new Dictionary<Card.Rank, int>
            {
                { Card.Rank.Ace, 0 },
                { Card.Rank.King, 1 },
                { Card.Rank.Queen, 2 },
                { Card.Rank.Jack, 3 },
                { Card.Rank.Ten, 4 },
                { Card.Rank.Nine, 5 },
                { Card.Rank.Eight, 6 },
                { Card.Rank.Seven, 7 },
                { Card.Rank.Six, 8 },
                { Card.Rank.Five, 9 },
                { Card.Rank.Four, 10 },
                { Card.Rank.Three, 11 },
                { Card.Rank.Two, 12 }
            };

            // 모양 순서 정의 (같은 값일 경우 모양으로 정렬)
            Dictionary<Card.Suit, int> suitOrder = new Dictionary<Card.Suit, int>
            {
                { Card.Suit.Spades, 0 },
                { Card.Suit.Hearts, 1 },
                { Card.Suit.Diamonds, 2 },
                { Card.Suit.Clubs, 3 }
            };

            // 값이 같으면 모양 순으로 정렬
            if (a.rank == b.rank)
            {
                return suitOrder[a.suit].CompareTo(suitOrder[b.suit]);
            }
            return rankOrder[a.rank].CompareTo(rankOrder[b.rank]);
        });

        // 정렬된 카드들의 시작 위치와 목표 위치 저장
        Vector3 startPosition = new Vector3(-4f, -2.5f, 0f);
        float xOffset = 1.3f; // x축 간격
        float moveDuration = 0.2f; // 이동 시간

        // 각 카드의 목표 위치 계산 및 이동
        for (int i = 0; i < activeCards.Count; i++)
        {
            Card card = activeCards[i];
            Vector3 targetPos = startPosition + new Vector3(i * xOffset, 0f, 0f);
            
            // 선택된 카드의 경우 현재 y축 위치 유지
            if (card.isSelected)
            {
                targetPos.y = card.transform.position.y;
            }

            // DOTween을 사용하여 카드 이동
            card.transform.DOMove(targetPos, moveDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => 
                {
                    card.SetOriginalPosition(targetPos);
                });
        }

        // 모든 애니메이션이 완료될 때까지 대기
        yield return new WaitForSeconds(moveDuration);
    }

    public Card GetCardAtPosition(float xPosition)
    {
        foreach (Card card in hand)
        {
            if (card != null && Mathf.Approximately(card.originalPosition.x, xPosition))
            {
                return card;
            }
        }
        return null;
    }

    public void MoveCardLeft(Card card)
    {
        float targetY = card.isSelected ? -2f : -2.5f; // 선택 상태에 따라 y축 위치 결정
        Vector3 newPosition = new Vector3(card.originalPosition.x - 1.3f, targetY, card.originalPosition.z);
        card.originalPosition = newPosition;
        
        // DOTween을 사용하여 직접 카드 이동
        card.transform.DOMove(newPosition, 0.2f)
            .SetEase(Ease.OutQuad);
    }

    public void MoveCardRight(Card card)
    {
        float targetY = card.isSelected ? -2f : -2.5f; // 선택 상태에 따라 y축 위치 결정
        Vector3 newPosition = new Vector3(card.originalPosition.x + 1.3f, targetY, card.originalPosition.z);
        card.originalPosition = newPosition;
        
        // DOTween을 사용하여 직접 카드 이동
        card.transform.DOMove(newPosition, 0.2f)
            .SetEase(Ease.OutQuad);
    }

    private IEnumerator MoveCardToPosition(Card card, Vector3 targetPosition)
    {
        // DOTween을 사용하여 카드 이동
        card.transform.DOMove(targetPosition, 0.2f)
            .SetEase(Ease.OutQuad);
            
        // 애니메이션이 완료될 때까지 대기
        yield return new WaitForSeconds(0.2f);
    }

    public Vector3 FindNearestEmptyPosition(Vector3 currentPosition)
    {
        float xOffset = 1.3f; // 카드 간격
        float startX = -4f; // 첫 번째 카드의 x 좌표
        List<float> occupiedPositions = new List<float>();

        // 현재 사용 중인 모든 x 좌표 수집
        foreach (Card card in hand)
        {
            if (card != null)
            {
                occupiedPositions.Add(card.originalPosition.x);
            }
        }

        // 현재 위치에서 가장 가까운 왼쪽 빈 자리 찾기
        float nearestEmptyX = startX;
        float minDistance = float.MaxValue;

        // 왼쪽에서 오른쪽으로 순차적으로 검사
        for (int i = 0; i < 8; i++)
        {
            float potentialX = startX + (i * xOffset);
            if (!occupiedPositions.Contains(potentialX))
            {
                // 현재 위치보다 왼쪽에 있는 빈 자리만 고려
                if (potentialX < currentPosition.x)
                {
                    float distance = Mathf.Abs(potentialX - currentPosition.x);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestEmptyX = potentialX;
                    }
                }
            }
        }

        // 만약 왼쪽에 빈 자리가 없다면, 현재 위치에서 가장 가까운 오른쪽 빈 자리 찾기
        if (nearestEmptyX == startX)
        {
            for (int i = 0; i < 8; i++)
            {
                float potentialX = startX + (i * xOffset);
                if (!occupiedPositions.Contains(potentialX))
                {
                    float distance = Mathf.Abs(potentialX - currentPosition.x);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestEmptyX = potentialX;
                    }
                }
            }
        }

        return new Vector3(nearestEmptyX, currentPosition.y, currentPosition.z);
    }

    private IEnumerator MoveCardsRightCoroutine(bool updateScore = true)
    {
        float moveDuration = 0.3f; // 이동 시간
        float maxX = 10f; // 최대 x 좌표
        List<Vector3> emptyPositions = new List<Vector3>(); // 빈 자리 위치들을 저장할 리스트

        // 선택된 카드들의 시작 위치와 목표 위치 저장
        Dictionary<Card, Vector3> cardPositions = new Dictionary<Card, Vector3>();
        foreach (Card card in selectedCards)
        {
            if (card != null)
            {
                // x축 위치만 저장하고 y축은 -2.5로 고정
                Vector3 emptyPos = card.transform.position;
                emptyPos.y = -2.5f;
                emptyPositions.Add(emptyPos);

                // 시작 위치와 목표 위치 저장
                Vector3 targetPos = new Vector3(maxX, card.transform.position.y, 0f);
                cardPositions[card] = targetPos;
            }
        }

        // updateScore가 true일 때만 점수 업데이트
        if (updateScore && handRanking != null)
        {
            handRanking.UpdateSumPoint();
        }

        // 모든 카드를 동시에 이동
        foreach (var card in selectedCards)
        {
            if (card != null && cardPositions.ContainsKey(card))
            {
                Vector3 targetPos = cardPositions[card];
                // DOTween을 사용하여 카드 이동
                card.transform.DOMove(targetPos, moveDuration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => 
                    {
                        // x 좌표가 maxX에 도달하면 카드 비활성화
                        if (card.transform.position.x >= maxX)
                        {
                            card.gameObject.SetActive(false);
                            card.isSelected = false; // 선택 상태도 해제
                        }
                    });
            }
        }

        // 모든 애니메이션이 완료될 때까지 대기
        yield return new WaitForSeconds(moveDuration);

        // 모든 선택된 카드의 선택 상태 해제 및 리스트 비우기
        foreach (Card card in selectedCards)
        {
            if (card != null)
            {
                card.isSelected = false;
                DiscardCard(card); // 카드를 버린 카드 더미로 이동
            }
        }
        selectedCards.Clear(); // selectedCards 리스트 비우기

        // 모든 새로운 카드를 먼저 생성
        List<(Card card, Vector3 target)> newCards = new List<(Card, Vector3)>();
        foreach (Vector3 emptyPos in emptyPositions)
        {
            Card newCard = DrawCard();
            if (newCard != null)
            {
                // 새로운 카드를 (10, -2.5) 위치에서 시작
                Vector3 startPos = new Vector3(10f, -2.5f, 0f);
                newCard.transform.position = startPos;
                
                // 카드의 원래 위치를 빈 자리로 설정
                newCard.SetOriginalPosition(emptyPos);
                
                // 새로운 카드의 초기 상태 설정
                newCard.isSelected = false;
                newCard.transform.localScale = cardScale;
                
                // 정렬 순서 초기화
                SpriteRenderer spriteRenderer = newCard.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingOrder = 0;
                }

                newCards.Add((newCard, emptyPos));
            }
        }

        // 모든 카드를 동시에 이동
        foreach (var (card, target) in newCards)
        {
            // DOTween을 사용하여 카드 이동
            card.transform.DOMove(target, moveDuration)
                .SetEase(Ease.OutQuad)
                .From(new Vector3(10f, -2.5f, 0f)) // 시작 위치를 명시적으로 설정
                .OnComplete(() => 
                {
                    card.SetOriginalPosition(target); // 최종 위치를 원래 위치로 설정
                    
                    // BoxCollider2D 크기 재조정
                    BoxCollider2D boxCollider = card.GetComponent<BoxCollider2D>();
                    if (boxCollider != null)
                    {
                        SpriteRenderer spriteRenderer = card.GetComponent<SpriteRenderer>();
                        if (spriteRenderer != null && spriteRenderer.sprite != null)
                        {
                            boxCollider.size = spriteRenderer.sprite.bounds.size;
                        }
                    }
                });
        }

        // 모든 애니메이션이 완료될 때까지 대기
        yield return new WaitForSeconds(moveDuration);
        gameovermanager.IsClearOrFail();
    }

    public List<Card> GetSelectedCards()
    {
        return selectedCards;
    }
}