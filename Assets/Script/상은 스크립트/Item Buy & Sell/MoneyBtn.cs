using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyBtn : MonoBehaviour
{
    SoundManager2 soundManager2;
    GameManager gameManager;
    

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        soundManager2 = FindObjectOfType<SoundManager2>();
    }
    public void OnPlus5MoneyBtnClick()
    {
        gameManager.PlusMoneyBtn(5);
        soundManager2.PlayCashOutSound();
    }

    public void OnPlus10MoneyBtnClick()
    {
        gameManager.PlusMoneyBtn(10);
        soundManager2.PlayCashOutSound();
    }

    public void OnPlus20MoneyBtnClick()
    {
        gameManager.PlusMoneyBtn(20);
        soundManager2.PlayCashOutSound();
    }

    public void OnPlus30MoneyBtnClick()
    {
        gameManager.PlusMoneyBtn(30);
        soundManager2.PlayCashOutSound();
    }

    public void OnMultiplyMoneyBtnClick()
    {
        gameManager.MultiplyMoneyBtn(2);
        soundManager2.PlayCashOutSound();
    }

}
