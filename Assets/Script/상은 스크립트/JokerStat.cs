using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JokerStat : MonoBehaviour
{
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
        DiamondR4
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
        handRanking = FindAnyObjectByType<HandRanking>();
        card = GetComponent<Card>(); // Get the Card component
        pricetag.text = "$" + price.ToString();
    }
    
// 카드 포인트 추가 로그 출력 메서드
//public void LogCardPointAddition(Card targetCard, int point)
//{
//    if (targetCard != null)
//    {
//        Debug.Log($"이건 조커스탯에서 출력한거 {targetCard.suit} 모양 {targetCard.rank}에 {point}만큼 블루칩 증가");
//    }
//}

/////////////////////////////////////////////////////////////////////////////////////////////////////
/// 카드 플레이 이후 발동
     
    public void HeartR4(Card targetCard, int point)
    {
        //Debug.Log($"이건 하트스탯에서 출력한거 {targetCard.suit} 모양 {targetCard.rank}에 {point}만큼 블루칩 증가");
        if(targetCard.suit == Card.Suit.Hearts) // 만약 현재 플레이할 카드의 모양이 하트라면!!!
        {
            Debug.Log($"하트 감지! 4만큼 레드칩 증가");
            handRanking.AddRedChipValue(4);
        }
    }
    public void SpadeR4(Card targetCard, int point)
    {
        if(targetCard.suit == Card.Suit.Spades)
        {
            Debug.Log($"스페이드 감지! 4만큼 레드칩 증가");
            handRanking.AddRedChipValue(4);
        }
    }    
    public void ClubR4(Card targetCard, int point)
    {
        if(targetCard.suit == Card.Suit.Clubs)
        {
            Debug.Log($"클로버 감지! 4만큼 레드칩 증가");
            handRanking.AddRedChipValue(4);
        }
    }    
    public void DiamondR4(Card targetCard, int point)
    {
        if(targetCard.suit == Card.Suit.Diamonds)
        {
            Debug.Log($"다이아몬드 감지! 4만큼 레드칩 증가");
            handRanking.AddRedChipValue(4);
        }
    }

////////////////////////////////////////////////////////////////////////////////////////////////
/// 핸드플레이 이후 발동
    public void AllR4()
    {
        Debug.Log($"핸드플레이 완료! 무조건 4만큼 레드칩 증가");
        handRanking.AddRedChipValue(4);
    }
    public void HaveMoneyB()
    {
        SaveManager saveManager = FindAnyObjectByType<SaveManager>();
        PlayerData playerData;
        playerData = saveManager.Load();

        Debug.Log("현재 가지고 있는 돈 : " + playerData.money);
        Debug.Log($"핸드플레이 완료! 돈" + playerData.money * 2 + "만큼 블루칩 증가");
        handRanking.AddBlueChipValue(playerData.money * 2);
    }
    public void HaveMoneyR()
    {
        SaveManager saveManager = FindAnyObjectByType<SaveManager>();
        PlayerData playerData;
        playerData = saveManager.Load();

        Debug.Log("현재 가지고 있는 돈 : " + playerData.money);
        Debug.Log($"핸드플레이 완료! 돈" + playerData.money / 2 + "만큼 레드칩 증가");
        handRanking.AddRedChipValue(playerData.money / 2);
    }


}
