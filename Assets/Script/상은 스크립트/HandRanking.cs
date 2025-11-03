using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class HandRanking : MonoBehaviour
{
    private DeckManager deckManager;
    private List<Card> selectedCards = new List<Card>();
    private List<Card> previousSelectedCards = new List<Card>(); // 이전 프레임의 선택된 카드들
    public TextMeshProUGUI handRankText; // UI 텍스트 연결용 변수
    public TextMeshProUGUI blueChipText; // BlueChip 점수를 표시할 UI 텍스트
    public TextMeshProUGUI redChipText; // 레드 칩 값 표시용 텍스트
    public TextMeshProUGUI sumPointText; // 총 포인트 표시용 텍스트
    public TextMeshProUGUI currentPointText; // 현재 포인트 표시용 텍스트
    public float sumPoint = 0f; // 총 포인트
    public float BlueChip = 0f; // 블루 칩 값
    public float RedChip = 0f; // 레드 칩 값
    public float currentPoint = 0f; // 현재 포인트
    public float pointSum = 0f;

    public GameObject BluechipFire;
    public GameObject RedchipFire;

    private bool isHandPlaying = false; // HandPlay 중인지 여부를 체크하는 플래그
    public string currentHandRanking;
    private bool hasTriggeredFire = false; // 플래그 변수 추가


    private GameSaveData gameSaveData;
    private GameManager gameManager;
    PlayerData playerData;
    SaveManager saveManager;

    void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        gameManager = FindObjectOfType<GameManager>();
        gameSaveData = FindAnyObjectByType<GameSaveData>();
        deckManager = FindObjectOfType<DeckManager>();
        sumPointText.text = "0"; // 초기값 설정
        playerData = saveManager.Load();
    }

    void Update()
    {
        if(isHandPlaying && gameManager.goalPoints[playerData.round - 1] <= BlueChip * RedChip)
        {
            if (!hasTriggeredFire) // 최초 실행인 경우에만 실행
        {
            hasTriggeredFire = true; // 플래그 설정
            // 파이어 오브젝트 활성화 및 애니메이션
            BluechipFire.SetActive(true);
            RedchipFire.SetActive(true);
            
            // 초기 크기를 (0.1, 0.1, 0.1)로 설정
            BluechipFire.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            RedchipFire.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            
            // 부드럽게 (3, 3, 3)으로 크기 증가
            BluechipFire.transform.DOScale(new Vector3(3f, 3f, 3f), 3f);
            RedchipFire.transform.DOScale(new Vector3(3f, 3f, 3f), 3f);
        }
        }
        else if(!isHandPlaying && BluechipFire.activeSelf && RedchipFire.activeSelf)
        {            
            // 초기 크기를 (3, 3, 3)으로 설정
            BluechipFire.transform.localScale = new Vector3(3f, 3f, 3f);
            RedchipFire.transform.localScale = new Vector3(3f, 3f, 3f);
            
            // 부드럽게 (0.1, 0.1, 0.1)으로 크기 감소 및 y축으로 20만큼 내려가며 비활성화
            Sequence blueChipSequence = DOTween.Sequence();
            blueChipSequence.Join(BluechipFire.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 3f));
            blueChipSequence.Join(BluechipFire.transform.DOMoveY(BluechipFire.transform.position.y - 20f, 3f));
            blueChipSequence.OnComplete(() => {
                BluechipFire.SetActive(false);
            });
            blueChipSequence.Play();
            
            Sequence redChipSequence = DOTween.Sequence();
            redChipSequence.Join(RedchipFire.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 3f));
            redChipSequence.Join(RedchipFire.transform.DOMoveY(RedchipFire.transform.position.y - 20f, 3f));
            redChipSequence.OnComplete(() => {
                RedchipFire.SetActive(false);
            });
            redChipSequence.Play();
        }
        if (!isHandPlaying) // HandPlay 중이 아닐 때만 족보 체크
        {
            selectedCards.Clear();
            foreach (Card card in deckManager.GetHand())
            {
                if (card != null && card.isSelected)
                {
                    selectedCards.Add(card);
                }
            }

            // 선택된 카드가 변경되었는지 확인
            bool cardsChanged = HasSelectedCardsChanged();
            if (cardsChanged)
            {
                DetermineHandRank(false); // sumPoint를 초기화하지 않도록 false 전달

                // 이전 선택 카드 목록 업데이트
                previousSelectedCards.Clear();
                previousSelectedCards.AddRange(selectedCards);
            }
        }
    }

    private bool HasSelectedCardsChanged()
    {
        if (selectedCards.Count != previousSelectedCards.Count)
            return true;

        for (int i = 0; i < selectedCards.Count; i++)
        {
            if (!previousSelectedCards.Contains(selectedCards[i]))
                return true;
        }

        return false;
    }

    private void DetermineHandRank(bool resetSumPoint = true)
    {
        string handRank = " ";

        if (selectedCards.Count == 0)
        {
            ResetChips();
        }
        else
        {
            if (IsStraightFlush(selectedCards))
            {
                handRank = "스트레이트\n플러시";
                currentHandRanking = "StraightFlush";
                BlueChip = 100f;
                RedChip = 8f;
            }
            else if (IsFourOfAKind(selectedCards))
            {
                handRank = "포카드";
                currentHandRanking = "FourCard";
                BlueChip = 60f;
                RedChip = 7f;
            }
            else if (IsFullHouse(selectedCards))
            {
                handRank = "풀하우스";
                currentHandRanking = "FullHouse";
                BlueChip = 40f;
                RedChip = 4f;
            }
            else if (IsFlush(selectedCards))
            {
                handRank = "플러시";
                currentHandRanking = "Flush";
                BlueChip = 35f;
                RedChip = 4f;
            }
            else if (IsStraight(selectedCards))
            {
                handRank = "스트레이트";
                currentHandRanking = "Straight";
                BlueChip = 30f;
                RedChip = 4f;
            }
            else if (IsThreeOfAKind(selectedCards))
            {
                handRank = "트리플";
                currentHandRanking = "Triple";
                BlueChip = 30f;
                RedChip = 3f;
            }
            else if (IsTwoPair(selectedCards))
            {
                handRank = "투페어";
                currentHandRanking = "TwoPair";
                BlueChip = 20f;
                RedChip = 2f;
            }
            else if (IsOnePair(selectedCards))
            {
                handRank = "원 페어";
                currentHandRanking = "Pair";
                BlueChip = 10f;
                RedChip = 2f;
            }
            else
            {
                handRank = "하이카드";
                currentHandRanking = "HighCard";
                BlueChip = 5f;
                RedChip = 1f;
            }
        }

        if (handRankText != null)
        {
            handRankText.text = handRank;
        }
        if (blueChipText != null)
        {
            blueChipText.text = BlueChip.ToString();
        }
        if (redChipText != null)
        {
            redChipText.text = RedChip.ToString();
        }
    }

    private bool IsStraightFlush(List<Card> cards)
    {
        return IsFlush(cards) && IsStraight(cards);
    }

    private bool IsFourOfAKind(List<Card> cards)
    {
        Dictionary<Card.Rank, int> rankCount = new Dictionary<Card.Rank, int>();
        foreach (Card card in cards)
        {
            if (!rankCount.ContainsKey(card.rank))
            {
                rankCount[card.rank] = 0;
            }
            rankCount[card.rank]++;
        }
        return rankCount.ContainsValue(4);
    }

    private bool IsFullHouse(List<Card> cards)
    {
        Dictionary<Card.Rank, int> rankCount = new Dictionary<Card.Rank, int>();
        foreach (Card card in cards)
        {
            if (!rankCount.ContainsKey(card.rank))
            {
                rankCount[card.rank] = 0;
            }
            rankCount[card.rank]++;
        }
        return rankCount.ContainsValue(3) && rankCount.ContainsValue(2);
    }

    private bool IsFlush(List<Card> cards)
    {
        if (cards.Count < 5) return false;
        Card.Suit firstSuit = cards[0].suit;
        foreach (Card card in cards)
        {
            if (card.suit != firstSuit)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsStraight(List<Card> cards)
    {
        if (cards.Count < 5) return false;

        // 카드를 값에 따라 정렬 (2,3,4,5,6,7,8,9,10,J,Q,K,A)
        List<int> values = new List<int>();
        foreach (Card card in cards)
        {
            if (card.rank == Card.Rank.Ace)
            {
                values.Add(1);  // A를 1로도 사용
                values.Add(14); // A를 14로도 사용
            }
            else
            {
                values.Add((int)card.rank);
            }
        }
        values.Sort();

        // 중복 제거
        values = values.Distinct().ToList();

        // 연속된 5장 체크
        for (int i = 0; i <= values.Count - 5; i++)
        {
            bool isConsecutive = true;
            for (int j = 0; j < 4; j++)
            {
                if (values[i + j + 1] - values[i + j] != 1)
                {
                    isConsecutive = false;
                    break;
                }
            }
            if (isConsecutive)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsThreeOfAKind(List<Card> cards)
    {
        Dictionary<Card.Rank, int> rankCount = new Dictionary<Card.Rank, int>();
        foreach (Card card in cards)
        {
            if (!rankCount.ContainsKey(card.rank))
            {
                rankCount[card.rank] = 0;
            }
            rankCount[card.rank]++;
        }
        return rankCount.ContainsValue(3);
    }

    private bool IsTwoPair(List<Card> cards)
    {
        if (cards.Count < 4) return false;

        Dictionary<Card.Rank, int> rankCount = new Dictionary<Card.Rank, int>();
        foreach (Card card in cards)
        {
            if (!rankCount.ContainsKey(card.rank))
            {
                rankCount[card.rank] = 0;
            }
            rankCount[card.rank]++;
        }

        int pairCount = 0;
        foreach (var count in rankCount.Values)
        {
            if (count >= 2)
            {
                pairCount++;
            }
        }
        return pairCount >= 2;
    }

    private bool IsOnePair(List<Card> cards)
    {
        if (cards.Count < 2) return false;

        Dictionary<Card.Rank, int> rankCount = new Dictionary<Card.Rank, int>();
        foreach (Card card in cards)
        {
            if (!rankCount.ContainsKey(card.rank))
            {
                rankCount[card.rank] = 0;
            }
            rankCount[card.rank]++;
        }

        int pairCount = 0;
        foreach (var count in rankCount.Values)
        {
            if (count >= 2)
            {
                pairCount++;
            }
        }
        return pairCount == 1;
    }

    public void StartHandPlay()
    {
        isHandPlaying = true;
    }

    public void EndHandPlay()
    {
        isHandPlaying = false;
    }

public void AddBlueChipValue(float value)
{
    BlueChip += value;
    // UI 업데이트
    if (blueChipText != null)
    {
        blueChipText.text = BlueChip.ToString();
        // 통통튀는 효과 추가
        blueChipText.transform.DOKill(); // 기존 애니메이션 중지
        blueChipText.transform.localScale = Vector3.one; // 크기 초기화
        
        // 전체적인 스케일 애니메이션 (1.0 -> 1.5 -> 1.0)
        Sequence scaleSequence = DOTween.Sequence();
        scaleSequence.Append(blueChipText.transform.DOScale(1.5f, 0.15f).SetEase(Ease.OutQuad));
        scaleSequence.Append(blueChipText.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack));
        
        // 말랑말랑한 통통 튀는 효과
        Sequence bounceSequence = DOTween.Sequence();
        
        // 위로 빠르게 튀어오르는 효과 (세로로 늘리고 가로로 줄이기)
        bounceSequence.Append(blueChipText.transform.DOScaleX(0.6f, 0.08f).SetEase(Ease.OutQuad));
        bounceSequence.Join(blueChipText.transform.DOScaleY(2.0f, 0.08f).SetEase(Ease.OutQuad));
        
        // 아래로 착지하며 퍼지는 효과
        bounceSequence.Append(blueChipText.transform.DOScaleX(1.8f, 0.1f).SetEase(Ease.OutQuad));
        bounceSequence.Join(blueChipText.transform.DOScaleY(0.4f, 0.1f).SetEase(Ease.InQuad));
        
        // 다시 위로 살짝 튀어오르기
        bounceSequence.Append(blueChipText.transform.DOScaleX(1.3f, 0.1f).SetEase(Ease.OutQuad));
        bounceSequence.Join(blueChipText.transform.DOScaleY(1.6f, 0.1f).SetEase(Ease.OutQuad));
        
        // 원래 크기로 돌아오기
        bounceSequence.Append(blueChipText.transform.DOScale(1f, 0.2f).SetEase(Ease.OutElastic));
        
        // 두 애니메이션을 함께 실행
        DOTween.Kill(blueChipText.transform); // 기존 애니메이션 정리
        scaleSequence.Play();
        bounceSequence.Play();
    }
}

public void AddRedChipValue(float value)
{
    RedChip += value;
    // UI 업데이트
    if (redChipText != null)
    {
        redChipText.text = RedChip.ToString();
        
        // 전체적인 스케일 애니메이션 (1.0 -> 1.5 -> 1.0)
        Sequence scaleSequence = DOTween.Sequence();
        scaleSequence.Append(redChipText.transform.DOScale(1.5f, 0.15f).SetEase(Ease.OutQuad));
        scaleSequence.Append(redChipText.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack));
        
        // 말랑말랑한 통통 튀는 효과
        Sequence bounceSequence = DOTween.Sequence();
        
        // 위로 빠르게 튀어오르는 효과 (세로로 늘리고 가로로 줄이기)
        bounceSequence.Append(redChipText.transform.DOScaleX(0.6f, 0.08f).SetEase(Ease.OutQuad));
        bounceSequence.Join(redChipText.transform.DOScaleY(2.0f, 0.08f).SetEase(Ease.OutQuad));
        
        // 아래로 착지하며 퍼지는 효과
        bounceSequence.Append(redChipText.transform.DOScaleX(1.8f, 0.1f).SetEase(Ease.OutQuad));
        bounceSequence.Join(redChipText.transform.DOScaleY(0.4f, 0.1f).SetEase(Ease.InQuad));
        
        // 다시 위로 살짝 튀어오르기
        bounceSequence.Append(redChipText.transform.DOScaleX(1.3f, 0.1f).SetEase(Ease.OutQuad));
        bounceSequence.Join(redChipText.transform.DOScaleY(1.6f, 0.1f).SetEase(Ease.OutQuad));
        
        // 원래 크기으로 돌아오기
        bounceSequence.Append(redChipText.transform.DOScale(1f, 0.2f).SetEase(Ease.OutElastic));
        
        // 두 애니메이션을 함께 실행
        DOTween.Kill(redChipText.transform); // 기존 애니메이션 정리
        scaleSequence.Play();
        bounceSequence.Play();
    }
}

public void MultipleRedChipValue(float value)
{
    RedChip *= value;
    // UI 업데이트
    if (redChipText != null)
    {
        redChipText.text = RedChip.ToString();
        
        // 전체적인 스케일 애니메이션 (1.0 -> 1.5 -> 1.0)
        Sequence scaleSequence = DOTween.Sequence();
        scaleSequence.Append(redChipText.transform.DOScale(1.5f, 0.15f).SetEase(Ease.OutQuad));
        scaleSequence.Append(redChipText.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack));
        
        // 말랑말랑한 통통 튀는 효과
        Sequence bounceSequence = DOTween.Sequence();
        
        // 위로 빠르게 튀어오르는 효과 (세로로 늘리고 가로로 줄이기)
        bounceSequence.Append(redChipText.transform.DOScaleX(0.6f, 0.08f).SetEase(Ease.OutQuad));
        bounceSequence.Join(redChipText.transform.DOScaleY(2.0f, 0.08f).SetEase(Ease.OutQuad));
        
        // 아래로 착지하며 퍼지는 효과
        bounceSequence.Append(redChipText.transform.DOScaleX(1.8f, 0.1f).SetEase(Ease.OutQuad));
        bounceSequence.Join(redChipText.transform.DOScaleY(0.4f, 0.1f).SetEase(Ease.InQuad));
        
        // 다시 위로 살짝 튀어오르기
        bounceSequence.Append(redChipText.transform.DOScaleX(1.3f, 0.1f).SetEase(Ease.OutQuad));
        bounceSequence.Join(redChipText.transform.DOScaleY(1.6f, 0.1f).SetEase(Ease.OutQuad));
        
        // 원래 크기으로 돌아오기
        bounceSequence.Append(redChipText.transform.DOScale(1f, 0.2f).SetEase(Ease.OutElastic));
        
        // 두 애니메이션을 함께 실행
        DOTween.Kill(redChipText.transform); // 기존 애니메이션 정리
        scaleSequence.Play();
        bounceSequence.Play();
    }
}

    public void UpdateSumPoint()
    {
        float newSumPoint = BlueChip * RedChip;
        pointSum += newSumPoint;
        gameManager.gsumPoint = newSumPoint;
        Debug.Log("현재 최고점수 : " + playerData.bestscore);
        Debug.Log("현재 점수 : " + newSumPoint);
        if(playerData.bestscore < newSumPoint)
        {
            gameManager.NewBestScore(newSumPoint);
        }
        
        // sumPoint를 부드럽게 새로운 값으로 업데이트하는 애니메이션
        DOTween.To(
            () => sumPoint,
            x => {
                sumPoint = x;
                if (sumPointText != null)
                {
                    sumPointText.text = Mathf.FloorToInt(x).ToString();
                }
            },
            newSumPoint,
            0.5f // 0.5초 동안 애니메이션
        ).OnComplete(() => {
            // sumPoint 애니메이션이 완료된 후 2초 대기 후에 currentPoint로 이동하는 애니메이션 시작
            DOVirtual.DelayedCall(1.2f, () => {
                float targetCurrentPoint = currentPoint + sumPoint;
                
                // currentPoint를 부드럽게 증가시키는 애니메이션
                DOTween.To(
                    () => currentPoint,
                    x => {
                        currentPoint = x;
                        if (currentPointText != null)
                        {
                            currentPointText.text = Mathf.FloorToInt(x).ToString();
                        }
                    },
                    targetCurrentPoint,
                    1f // 1초 동안 애니메이션
                );
                
                // sumPoint를 0으로 부드럽게 감소시키는 애니메이션
                DOTween.To(
                    () => sumPoint,
                    x => {
                        sumPoint = x;
                        if (sumPointText != null)
                        {
                            sumPointText.text = Mathf.FloorToInt(x).ToString();
                        }
                    },
                    0f, // 0으로 감소
                    1f  // 1초 동안 애니메이션
                );
            });
        });
    }

    // BlueChip과 RedChip을 리셋하는 메서드 추가
    public void ResetChips()
    {
        BlueChip = 0f;
        RedChip = 0f;
        if (blueChipText != null)
        {
            blueChipText.text = "0";
        }
        if (redChipText != null)
        {
            redChipText.text = "0";
        }
    }

    // 족보를 구성하는 카드들을 반환하는 메서드
    public List<Card> GetRankingCards(List<Card> cards)
    {
        List<Card> rankingCards = new List<Card>();

        if (IsStraightFlush(cards))
        {
            rankingCards.AddRange(cards); // 스트레이트 플러시는 모든 카드 포함
        }
        else if (IsFourOfAKind(cards))
        {
            // 포카드 구성 카드 찾기
            var rankGroups = cards.GroupBy(c => c.rank).OrderByDescending(g => g.Count());
            var fourCards = rankGroups.First().ToList(); // 4장 카드
            rankingCards.AddRange(fourCards);
        }
        else if (IsFullHouse(cards))
        {
            // 풀하우스 구성 카드 찾기 (3장 + 2장)
            var rankGroups = cards.GroupBy(c => c.rank).OrderByDescending(g => g.Count());
            rankingCards.AddRange(rankGroups.First().ToList()); // 3장 카드
            rankingCards.AddRange(rankGroups.Skip(1).First().ToList()); // 2장 카드
        }
        else if (IsFlush(cards))
        {
            rankingCards.AddRange(cards); // 플러시는 모든 카드 포함
        }
        else if (IsStraight(cards))
        {
            rankingCards.AddRange(cards); // 스트레이트는 모든 카드 포함
        }
        else if (IsThreeOfAKind(cards))
        {
            // 트리플 구성 카드 찾기
            var rankGroups = cards.GroupBy(c => c.rank).OrderByDescending(g => g.Count());
            var threeCards = rankGroups.First().ToList(); // 3장 카드
            rankingCards.AddRange(threeCards);
        }
        else if (IsTwoPair(cards))
        {
            // 투페어 구성 카드 찾기
            var rankGroups = cards.GroupBy(c => c.rank).OrderByDescending(g => g.Count());
            var firstPair = rankGroups.First().ToList(); // 첫 번째 페어
            var secondPair = rankGroups.Skip(1).First().ToList(); // 두 번째 페어
            rankingCards.AddRange(firstPair);
            rankingCards.AddRange(secondPair);
        }
        else if (IsOnePair(cards))
        {
            // 원페어 구성 카드 찾기
            var rankGroups = cards.GroupBy(c => c.rank).OrderByDescending(g => g.Count());
            var pairCards = rankGroups.First().ToList(); // 페어 카드
            rankingCards.AddRange(pairCards);
        }
        else
        {
            // 하이카드의 경우 가장 높은 카드만 반환
            Card highestCard = cards.OrderByDescending(c => c.rank == Card.Rank.Ace ? 14 : (int)c.rank).First();
            rankingCards.Add(highestCard);
        }

        return rankingCards;
    }
}
