using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HandRanking : MonoBehaviour
{
    private DeckManager deckManager;
    private List<Card> selectedCards = new List<Card>();
    public TextMeshProUGUI handRankText; // UI 텍스트 연결용 변수
    public TextMeshProUGUI blueChipText; // 블루 칩 값 표시용 텍스트
    public TextMeshProUGUI redChipText; // 레드 칩 값 표시용 텍스트
    public TextMeshProUGUI sumPointText; // 총 포인트 표시용 텍스트
    public float BlueChip = 0f; // 블루 칩 값
    public float RedChip = 0f; // 레드 칩 값

    void Start()
    {
        deckManager = FindObjectOfType<DeckManager>();
    }

    void Update()
    {
        selectedCards.Clear();
        foreach (Card card in deckManager.GetHand())
        {
            if (card != null && card.isSelected)
            {
                selectedCards.Add(card);
            }
        }

        string handRank = DetermineHandRank();
        if (handRankText != null)
        {
            handRankText.text = handRank;
        }

        // 칩 값 업데이트
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

    private string DetermineHandRank()
    {
        if (selectedCards.Count == 0)
        {
            BlueChip = 0f;
            RedChip = 0f;
            return "없음";
        }
        else
        {
            // 모든 족보 확인 (높은 순서대로)
            if (IsStraightFlush(selectedCards))
            {
                BlueChip = 100f;
                RedChip = 8f;
                return "스트레이트 플러시";
            }
            else if (IsFourOfAKind(selectedCards))
            {
                BlueChip = 60f;
                RedChip = 7f;
                return "포카드";
            }
            else if (IsFullHouse(selectedCards))
            {
                BlueChip = 40f;
                RedChip = 4f;
                return "풀하우스";
            }
            else if (IsFlush(selectedCards))
            {
                BlueChip = 35f;
                RedChip = 4f;
                return "플러시";
            }
            else if (IsStraight(selectedCards))
            {
                BlueChip = 30f;
                RedChip = 4f;
                return "스트레이트";
            }
            else if (IsThreeOfAKind(selectedCards))
            {
                BlueChip = 30f;
                RedChip = 3f;
                return "트리플";
            }
            else if (IsTwoPair(selectedCards))
            {
                BlueChip = 20f;
                RedChip = 2f;
                return "투페어";
            }
            else if (IsOnePair(selectedCards))
            {
                BlueChip = 10f;
                RedChip = 2f;
                return "원 페어";
            }
            else
            {
                BlueChip = 5f;
                RedChip = 1f;
                return "하이카드";
            }
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
}
