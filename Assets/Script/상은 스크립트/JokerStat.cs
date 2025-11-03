using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class JokerStat : MonoBehaviour
{
    JokerChipStack jokerChipStack;
    SoundManager2 soundManager2;
    GameManager gameManager;
    DeckManager deckManager;
    HandRanking handRanking;
    Card card;

    [Header("조커 슬롯")]
    public int slotNumber = 0;


    [Header("아이템 가격")]
    public TextMeshProUGUI pricetag;
    public int price;

    [Header("조커가 발동될 타이밍")]
    public PlayTiming playTiming;
    [Header("조커 능력")]
    public JokerSkill jokerSkill;

    [Header("부여할 점수")]
    public TextMeshProUGUI pointText;


    public enum PlayTiming
    {
        Before_HandPlay,
        After_HandPlay,
        After_CardPlay,
        Etc
    }

    public enum JokerSkill
    {
        HeartR4,
        SpadeR4,
        ClubR4,
        DiamondR4,
        EvenR4,
        OddB31,
        SuitB30,
        AceB80,
        TenOrFourB40,
        ImageR5,
        PiboR8,
        DiamondM,
        Heart05RX15,
        SpadeB50,
        ClubR7
    }

    // 필요하다면 enum → string 변환
    public string GetTimingAsString()
    {
        return playTiming.ToString();
    }
    public string GetSkillAsString()
    {
        return jokerSkill.ToString();
    }

    void Start()
    {
        soundManager2 = FindObjectOfType<SoundManager2>();
        gameManager = FindObjectOfType<GameManager>();
        deckManager = FindObjectOfType<DeckManager>();
        handRanking = FindAnyObjectByType<HandRanking>();
        jokerChipStack = FindObjectOfType<JokerChipStack>();
        card = GetComponent<Card>(); // Get the Card component
        pricetag.text = "$" + price.ToString();
    }
    
    // 점수 텍스트를 표시하는 메서드
    public enum TextColor { Red, Blue, Yellow }
    
    private void ShowPointText(string text, TextColor color = TextColor.Red)
    {
        // 텍스트 색상 설정 (RGB 값을 0~1 범위로 정규화)
        Color redColor = new Color(255f/255f, 65f/255f, 68f/255f); // #FF4144
        Color blueColor = new Color(23f/255f, 183f/255f, 255f/255f); // #17B7FF
        Color yellowColor = new Color(255f/255f, 255f/255f, 0f); // #FFFF00 (밝은 노란색)
        
        pointText.color = color switch
        {
            TextColor.Blue => blueColor,
            TextColor.Yellow => yellowColor,
            _ => redColor // 기본값은 빨간색
        };
        
        pointText.text = text;
        pointText.DOFade(1, 0); // 즉시 완전히 보이게 설정
        DOVirtual.DelayedCall(0.5f, () => {
            pointText.text = " ";
            pointText.DOFade(0, 0.3f); // 부드럽게 사라지도록 페이드 아웃
        });
    }
    
/////////////////////////////////////////////////////////////////////////////////////////////////////
/// 카드 플레이 이후 발동
     
    public void HeartR4(Card targetCard, int point)
    {
        //Debug.Log($"이건 하트스탯에서 출력한거 {targetCard.suit} 모양 {targetCard.rank}에 {point}만큼 블루칩 증가");
        if(targetCard.suit == Card.Suit.Hearts) // 만약 현재 플레이할 카드의 모양이 하트라면!!!
        {   
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"하트 감지! 4만큼 레드칩 증가");
            handRanking.AddRedChipValue(4);
            ShowPointText("+4", TextColor.Red);
            soundManager2.PlayRedChipSound();
        }
    }
    public void SpadeR4(Card targetCard, int point)
    {
        if(targetCard.suit == Card.Suit.Spades)
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"스페이드 감지! 4만큼 레드칩 증가");
            handRanking.AddRedChipValue(4);
            ShowPointText("+4", TextColor.Red);
            soundManager2.PlayRedChipSound();
        }
    }    
    public void ClubR4(Card targetCard, int point)
    {
        if(targetCard.suit == Card.Suit.Clubs)
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"클로버 감지! 4만큼 레드칩 증가");
            handRanking.AddRedChipValue(4);
            ShowPointText("+4", TextColor.Red);
            soundManager2.PlayRedChipSound();
        }
    }
    public void DiamondR4(Card targetCard, int point)
    {
        if(targetCard.suit == Card.Suit.Diamonds)
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"다이아몬드 감지! 4만큼 레드칩 증가");
            handRanking.AddRedChipValue(4);
            ShowPointText("+4", TextColor.Red);
            soundManager2.PlayRedChipSound();
        }
    }
    public void EvenR4(Card targetCard, int point)
    {
        int rankValue = (int)targetCard.rank; // enum을 int로 변환
        if(rankValue % 2 == 0)
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"짝수 감지! 4만큼 레드칩 증가");
            handRanking.AddRedChipValue(4);
            ShowPointText("+4", TextColor.Red);
            soundManager2.PlayRedChipSound();
        }
    }
    public void OddB31(Card targetCard, int point)
    {
        int rankValue = (int)targetCard.rank; // enum을 int로 변환
        if(rankValue % 2 != 0)
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"홀수 감지! 31만큼 블루칩 증가");
            handRanking.AddBlueChipValue(31);
            ShowPointText("+31", TextColor.Blue);
            soundManager2.PlayJokerBlueChipSound();
        }
    }
    public void SuitB30(Card targetCard, int point)
    {
        if(targetCard.rank == Card.Rank.Jack || targetCard.rank == Card.Rank.Queen || targetCard.rank == Card.Rank.King)
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"잭, 퀸, 킹 감지! 30만큼 블루칩 증가");
            handRanking.AddBlueChipValue(30);
            ShowPointText("+30", TextColor.Blue);
            soundManager2.PlayJokerBlueChipSound();
        }
    }
    public void AceB80(Card targetCard, int point)
    {
        if(targetCard.rank == Card.Rank.Ace)
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"에이스 감지! 80만큼 블루칩 증가");
            handRanking.AddBlueChipValue(80);
            ShowPointText("+80", TextColor.Blue);
            soundManager2.PlayJokerBlueChipSound();
        }
    }
    public void TenOrFourB40(Card targetCard, int point)
    {
        if(targetCard.rank == Card.Rank.Ten || targetCard.rank == Card.Rank.Four)
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"10, 4 감지! 40만큼 블루칩 증가");
            handRanking.AddBlueChipValue(40);
            ShowPointText("+40", TextColor.Blue);
            soundManager2.PlayJokerBlueChipSound();
        }
    }
    public void ImageR5(Card targetCard, int point)
    {
        if(targetCard.rank == Card.Rank.Jack || targetCard.rank == Card.Rank.Queen || targetCard.rank == Card.Rank.King)
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"잭, 퀸, 킹 감지! 5만큼 레드칩 증가");
            handRanking.AddRedChipValue(5);
            ShowPointText("+5", TextColor.Red);
            soundManager2.PlayRedChipSound();
        }
    }
    public void PiboR8(Card targetCard, int point)
    {
        if(targetCard.rank == Card.Rank.Two || targetCard.rank == Card.Rank.Three || targetCard.rank == Card.Rank.Five
        || targetCard.rank == Card.Rank.Eight || targetCard.rank == Card.Rank.Ace)
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"2, 3, 5, 8, 에이스 감지! 8만큼 레드칩 증가");
            handRanking.AddRedChipValue(8);
            ShowPointText("+8", TextColor.Red);
            soundManager2.PlayRedChipSound();
        }
    }
    public void DiamondM(Card targetCard, int point)
    {
        if(targetCard.suit == Card.Suit.Diamonds)
        {
            deckManager.BounceCard(gameObject.transform);
            gameManager.playerData.money += 1;
            gameManager.saveManager.Save(gameManager.playerData);
            gameManager.UpdateUI();
            ShowPointText("+1", TextColor.Yellow);
            soundManager2.PlayCoinSound();
        }
    }
    public void ClubR7(Card targetCard, int point)
    {
        if(targetCard.suit == Card.Suit.Clubs)
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"클로버 감지! 7만큼 레드칩 증가");
            handRanking.AddRedChipValue(7);
            ShowPointText("+7", TextColor.Red);
            soundManager2.PlayRedChipSound();
        }
    }
    public void SpadeB50(Card targetCard, int point)
    {
        if(targetCard.suit == Card.Suit.Spades)
        {
            deckManager.BounceCard(gameObject.transform);
            handRanking.AddBlueChipValue(50);
            ShowPointText("+50", TextColor.Blue);
            soundManager2.PlayJokerBlueChipSound();
        }
    }
    public void Heart05RX15(Card targetCard, int point)
    {
        if(targetCard.suit == Card.Suit.Hearts)
        {
            int randomnumber = Random.Range(1, 3);
            if(randomnumber == 1)
            {
                deckManager.BounceCard(gameObject.transform);
                Debug.Log($"하트 감지! 1.5만큼 곱하기");
                handRanking.MultipleRedChipValue(1.5f);
                ShowPointText("x1.5", TextColor.Red);
                soundManager2.PlayMultipleSound();
            }
        }
    }
    public void KingOrQueenRX2(Card targetCard, int point)
    {
        if(targetCard.rank == Card.Rank.King || targetCard.rank == Card.Rank.Queen)
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"킹 또는 퀸 감지! 2만큼 곱하기");
            handRanking.MultipleRedChipValue(2);
            ShowPointText("x2", TextColor.Red);
            soundManager2.PlayMultipleSound();
        }
    }
    





////////////////////////////////////////////////////////////////////////////////////////////////
/// 핸드플레이 이후 발동
    public void AllR4()
    {
        deckManager.BounceCard(gameObject.transform);
        Debug.Log($"핸드플레이 완료! 무조건 4만큼 레드칩 증가");
        handRanking.AddRedChipValue(4);
        ShowPointText("+4", TextColor.Red);
        soundManager2.PlayRedChipSound();
    }
    public void HaveMoneyB()
    {
        SaveManager saveManager = FindAnyObjectByType<SaveManager>();
        PlayerData playerData;
        playerData = saveManager.Load();
        deckManager.BounceCard(gameObject.transform);
        Debug.Log($"핸드플레이 완료! 돈" + playerData.money * 2 + "만큼 블루칩 증가");
        handRanking.AddBlueChipValue(playerData.money * 2);
        ShowPointText("+" + playerData.money * 2, TextColor.Blue);
        soundManager2.PlayJokerBlueChipSound();
    }
    public void HaveMoneyR()
    {
        SaveManager saveManager = FindAnyObjectByType<SaveManager>();
        PlayerData playerData;
        playerData = saveManager.Load();
        deckManager.BounceCard(gameObject.transform);
        Debug.Log($"핸드플레이 완료! 돈" + (playerData.money / 5) * 2 + "만큼 레드칩 증가");
        handRanking.AddRedChipValue((playerData.money / 5) * 2);
        ShowPointText("+" + (playerData.money / 5) * 2, TextColor.Red);
        soundManager2.PlayRedChipSound();
    }
    public void PairR8()
    {
        if(handRanking.currentHandRanking == "Pair")
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"Pair 감지! 8만큼 레드칩 증가");
            handRanking.AddRedChipValue(8);
            ShowPointText("+8", TextColor.Red);
            soundManager2.PlayRedChipSound();
        }
    }
    public void TripleR12()
    {
        if(handRanking.currentHandRanking == "Triple")
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"Triple 감지! 12만큼 레드칩 증가");
            handRanking.AddRedChipValue(12);
            ShowPointText("+12", TextColor.Red);
            soundManager2.PlayRedChipSound();
        }
    }
    public void StraightR12()
    {
        if(handRanking.currentHandRanking == "Straight")
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"Straight 감지! 12만큼 레드칩 증가");
            handRanking.AddRedChipValue(12);
            ShowPointText("+12", TextColor.Red);
            soundManager2.PlayRedChipSound();
        }
    }
    public void FlushR10()
    {
        if(handRanking.currentHandRanking == "Flush")
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"Flush 감지! 10만큼 레드칩 증가");
            handRanking.AddRedChipValue(10);
            ShowPointText("+10", TextColor.Red);
            soundManager2.PlayRedChipSound();
        }
    }
    public void PairB50()
    {
        if(handRanking.currentHandRanking == "Pair")
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"Pair 감지! 50만큼 블루칩 증가");
            handRanking.AddBlueChipValue(50);
            ShowPointText("+50", TextColor.Blue);
            soundManager2.PlayJokerBlueChipSound();
        }
    }
    public void TripleB100()
    {
        if(handRanking.currentHandRanking == "Triple")
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"Triple 감지! 100만큼 블루칩 증가");
            handRanking.AddBlueChipValue(100);
            ShowPointText("+100", TextColor.Blue);
            soundManager2.PlayJokerBlueChipSound();
        }
    }
    public void TwoPairB80()
    {
        if(handRanking.currentHandRanking == "Two Pair")
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"Two Pair 감지! 80만큼 블루칩 증가");
            handRanking.AddBlueChipValue(80);
            ShowPointText("+80", TextColor.Blue);
            soundManager2.PlayJokerBlueChipSound();
        }
    }
    public void StraightB100()
    {
        if(handRanking.currentHandRanking == "Straight")
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"Straight 감지! 100만큼 블루칩 증가");
            handRanking.AddBlueChipValue(100);
            ShowPointText("+100", TextColor.Blue);
            soundManager2.PlayJokerBlueChipSound();
        }
    }
    public void FlushB80()
    {
        if(handRanking.currentHandRanking == "Flush")
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"Flush 감지! 80만큼 블루칩 증가");
            handRanking.AddBlueChipValue(80);
            ShowPointText("+80", TextColor.Blue);
            soundManager2.PlayJokerBlueChipSound();
        }
    }
    public void RandomR30()
    {
        if (Random.Range(0, 3) == 0) // 1/3 chance
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"RandomR30 감지! 30만큼 레드칩 증가");
            handRanking.AddRedChipValue(30);
            ShowPointText("+30", TextColor.Red);
            soundManager2.PlayRedChipSound();
        }
    }
    public void Row3R20()
    {
        if(deckManager.selectedCards.Count <= 3)
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"Row3R20 감지! 20만큼 레드칩 증가");
            handRanking.AddRedChipValue(20);
            ShowPointText("+20", TextColor.Red);
            soundManager2.PlayRedChipSound();
        }
    }
    public void RemainTB30()
    {
        if(gameManager.currentTrashCount > 0)
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"RemainTB30 감지! {gameManager.currentTrashCount}만큼 블루칩 증가");
            handRanking.AddBlueChipValue(gameManager.currentTrashCount * 30);
            ShowPointText("+" + gameManager.currentTrashCount * 30, TextColor.Blue);
            soundManager2.PlayJokerBlueChipSound();
        }
    }
    public void RemainT0R15()
    {
        if(gameManager.currentTrashCount == 0)
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"RemainT0R15 감지! 15만큼 레드칩 증가");
            handRanking.AddRedChipValue(15);
            ShowPointText("+15", TextColor.Red);
            soundManager2.PlayRedChipSound();
        }
    }
    public void RandomR()
    {
        deckManager.BounceCard(gameObject.transform);
        int random = Random.Range(1, 30);
        handRanking.AddRedChipValue(random);
        ShowPointText("+" + random, TextColor.Red);
        soundManager2.PlayRedChipSound();
    }
    public void SmallRankX2R()
    {
        List<Card> unselectedCards = deckManager.hand.FindAll(card => !deckManager.selectedCards.Contains(card) && card != null);
        int lowestRankValue = unselectedCards.Count > 0 ? (int)unselectedCards.Min(card => card.rank) : 0;
        deckManager.BounceCard(gameObject.transform);
        Debug.Log($"SmallRankX2R 감지! {lowestRankValue * 2}만큼 레드칩 증가");
        handRanking.AddRedChipValue(lowestRankValue * 2);
        ShowPointText("+" + lowestRankValue * 2, TextColor.Red);
        soundManager2.PlayRedChipSound();
    }
    public void JokerR3()
    {
        JokerStat[] allJokers = FindObjectsOfType<JokerStat>();
        int highestSlot = allJokers.Length > 0 ? 
        allJokers.Max(j => j.slotNumber) : 0;
        deckManager.BounceCard(gameObject.transform);
        Debug.Log($"JokerR3 감지! {highestSlot * 3}만큼 레드칩 증가");
        handRanking.AddRedChipValue(highestSlot * 3);
        ShowPointText("+" + highestSlot * 3, TextColor.Red);
        soundManager2.PlayRedChipSound();
    }
    public void RemainCardB3()
    {
        RemainCards remainCards = FindObjectOfType<RemainCards>();
        deckManager.BounceCard(gameObject.transform);
        Debug.Log($"RemainCardB3 감지! {remainCards.remainCardsCount}만큼 블루칩 증가");
        handRanking.AddBlueChipValue(remainCards.remainCardsCount * 3);
        ShowPointText("+" + remainCards.remainCardsCount * 3, TextColor.Blue);
        soundManager2.PlayJokerBlueChipSound();
    }
    public void PriceR()
    {
        JokerStat[] allJokers = FindObjectsOfType<JokerStat>();
        if (allJokers.Length != 0)
        {
        int totalPrice = 0;
        foreach (var joker in allJokers)
        {
            totalPrice += joker.price;
        }
        deckManager.BounceCard(gameObject.transform);
        handRanking.AddRedChipValue(totalPrice);
        ShowPointText("+" + totalPrice, TextColor.Red);
        soundManager2.PlayRedChipSound();
        }
    }
    public void EmptyJokerRX2()
    {
        JokerStat[] allJokers = FindObjectsOfType<JokerStat>();
        int highestSlot = allJokers.Length > 0 ? 
        allJokers.Max(j => j.slotNumber) : 0;
        if(highestSlot < 5)
        {
        deckManager.BounceCard(gameObject.transform);
        Debug.Log($"EmptyJokerRX2 감지! {(6-highestSlot)}만큼 레드칩 곱하기");
        handRanking.MultipleRedChipValue((6-highestSlot));
        ShowPointText("x" + (6-highestSlot), TextColor.Red);
        soundManager2.PlayMultipleSound();
        }
    }
    public void LastHRX4()
    {
        if(gameManager.currentHandCount == 0)
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"LastHRX4 감지! 4만큼 레드칩 곱하기");
            handRanking.MultipleRedChipValue(4);
            ShowPointText("x4", TextColor.Red);
            soundManager2.PlayMultipleSound();
        }
    }
    public void PairRX2()
    {
        if(handRanking.currentHandRanking == "Pair")
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"PairRX2 감지! 2만큼 레드칩 곱하기");
            handRanking.MultipleRedChipValue(2);
            ShowPointText("x2", TextColor.Red);
            soundManager2.PlayMultipleSound();
        }
    }
    public void TripleRX3()
    {
        if(handRanking.currentHandRanking == "Triple")
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"TripleRX3 감지! 3만큼 레드칩 곱하기");
            handRanking.MultipleRedChipValue(3);
            ShowPointText("x3", TextColor.Red);
            soundManager2.PlayMultipleSound();
        }
    }
    public void FourCardRX4()
    {
        if(handRanking.currentHandRanking == "FourCard")
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"FourCardRX4 감지! 4만큼 레드칩 곱하기");
            handRanking.MultipleRedChipValue(4);
            ShowPointText("x4", TextColor.Red);
            soundManager2.PlayMultipleSound();
        }
    }
    public void StraightRX3()
    {
        if(handRanking.currentHandRanking == "Straight")
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"StraightRX3 감지! 3만큼 레드칩 곱하기");
            handRanking.MultipleRedChipValue(3);
            ShowPointText("x3", TextColor.Red);
            soundManager2.PlayMultipleSound();
        }
    }
    public void FlushRX2()
    {
        if(handRanking.currentHandRanking == "Flush")
        {
            deckManager.BounceCard(gameObject.transform);
            Debug.Log($"FlushRX2 감지! 2만큼 레드칩 곱하기");
            handRanking.MultipleRedChipValue(2);
            ShowPointText("x2", TextColor.Red);
            soundManager2.PlayMultipleSound();
        }
    }
    public void B250R15()
    {
        deckManager.BounceCard(gameObject.transform);
        Debug.Log($"B250R15 감지! 250만큼 블루칩 증가, 15만큼 레드칩 감소");
        handRanking.AddBlueChipValue(250);
        handRanking.AddRedChipValue(-15);
        ShowPointText("+250", TextColor.Blue);
        ShowPointText("-15", TextColor.Red);
        soundManager2.PlayJokerBlueChipSound();
    }
    public void RerollSR3()
    {
        deckManager.BounceCard(gameObject.transform);
        Debug.Log($"RerollR3 감지! {jokerChipStack.RerollRedChip}만큼 레드칩 증가");
        handRanking.AddRedChipValue(jokerChipStack.RerollRedChip);
        ShowPointText("+" + jokerChipStack.RerollRedChip, TextColor.Red);
        soundManager2.PlayRedChipSound();
    }

}