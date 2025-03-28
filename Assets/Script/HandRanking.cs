using System.Collections;
using System.Collections.Generic;
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
    private bool isHandPlaying = false; // HandPlay 중인지 여부를 체크하는 플래그

    void Start()
    {
        deckManager = FindObjectOfType<DeckManager>();
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
                DetermineHandRank();

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

    private void DetermineHandRank()
    {
        string handRank = "없음";

        if (selectedCards.Count == 0)
        {
            BlueChip = 0f;
            RedChip = 0f;
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
        if (sumPointText != null)
        {
            float totalPoint = BlueChip * RedChip;
            sumPointText.text = totalPoint.ToString();
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
        List<int> ranks = new List<int>();
        foreach (Card card in cards)
        {
            ranks.Add((int)card.rank);
        }
        ranks.Sort();

        // A-5 스트레이트 특수 처리
        if (ranks.Contains((int)Card.Rank.Ace))
        {
            ranks.Add(1); // Ace를 1로도 사용할 수 있음
        }

        for (int i = 0; i <= ranks.Count - 5; i++)
        {
            bool isStraight = true;
            for (int j = 0; j < 4; j++)
            {
                if (ranks[i + j + 1] - ranks[i + j] != 1)
                {
                    isStraight = false;
                    break;
                }
            }
            if (isStraight) return true;
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
        if (sumPointText != null)
        {
            float totalPoint = BlueChip * RedChip;
            sumPointText.text = totalPoint.ToString();
        }
    }
}
