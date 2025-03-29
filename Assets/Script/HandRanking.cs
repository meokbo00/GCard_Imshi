using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class HandRanking : MonoBehaviour
{
    private DeckManager deckManager;
    private List<Card> selectedCards = new List<Card>();
    private List<Card> previousSelectedCards = new List<Card>(); // 이전 프레임의 선택된 카드들
    public TextMeshProUGUI handRankText; // UI 텍스트 연결용 변수
    public TextMeshProUGUI blueChipText; // BlueChip 점수를 표시할 UI 텍스트
    public TextMeshProUGUI redChipText; // 레드 칩 값 표시용 텍스트
    public TextMeshProUGUI sumPointText; // 총 포인트 표시용 텍스트
    public float BlueChip = 0f; // 블루 칩 값
    public float RedChip = 0f; // 레드 칩 값
    private float sumPoint = 0f; // 누적 포인트 값
    private bool isHandPlaying = false; // HandPlay 중인지 여부를 체크하는 플래그

    void Start()
    {
        deckManager = FindObjectOfType<DeckManager>();
        sumPointText.text = "0"; // 초기값 설정
    }

    void Update()
    {
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
        string handRank = "없음";

        if (selectedCards.Count == 0)
        {
            ResetChips();
        }
        else
        {
            if (IsStraightFlush(selectedCards))
            {
                handRank = "스트레이트 플러시";
                BlueChip = 100f;
                RedChip = 8f;
            }
            else if (IsFourOfAKind(selectedCards))
            {
                handRank = "포카드";
                BlueChip = 60f;
                RedChip = 7f;
            }
            else if (IsFullHouse(selectedCards))
            {
                handRank = "풀하우스";
                BlueChip = 40f;
                RedChip = 4f;
            }
            else if (IsFlush(selectedCards))
            {
                handRank = "플러시";
                BlueChip = 35f;
                RedChip = 4f;
            }
            else if (IsStraight(selectedCards))
            {
                handRank = "스트레이트";
                BlueChip = 30f;
                RedChip = 4f;
            }
            else if (IsThreeOfAKind(selectedCards))
            {
                handRank = "트리플";
                BlueChip = 30f;
                RedChip = 3f;
            }
            else if (IsTwoPair(selectedCards))
            {
                handRank = "투페어";
                BlueChip = 20f;
                RedChip = 2f;
            }
            else if (IsOnePair(selectedCards))
            {
                handRank = "원 페어";
                BlueChip = 10f;
                RedChip = 2f;
            }
            else
            {
                handRank = "하이카드";
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
        }
    }

    public void UpdateSumPoint()
    {
        float currentPoints = BlueChip * RedChip;
        sumPoint += currentPoints;  // 기존 sumPoint에 현재 점수를 더함
        if (sumPointText != null)
        {
            sumPointText.text = sumPoint.ToString();
        }
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
