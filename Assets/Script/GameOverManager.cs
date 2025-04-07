using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public GameObject MoneyBox;
    public GameObject FailBox;
    public GameObject ShopBox;
    
    HandRanking handRanking;
    GameManager gameManager;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        handRanking = FindObjectOfType<HandRanking>();
        MoneyBox.SetActive(false);
        FailBox.SetActive(false);
    }
    public void IsClearOrFail()
    {
        Debug.Log("메서드 실행!");
        if (handRanking.sumPoint >= gameManager.GoalPoint)
        {
            Debug.Log("스테이지 클리어!");
            MoneyBox.SetActive(true);
        }
        else if (gameManager.handcount == 0 && (handRanking.sumPoint < gameManager.GoalPoint))
        {
            FailBox.SetActive(true);
        }
    }

    // 머니박스와 실패박스에 달린 버튼을 눌렀을 때 실행되는 메서드
    public void OnCashOutButton()
    {
        MoneyBox.SetActive(false);
        ShopBox.SetActive(true);
    }
}