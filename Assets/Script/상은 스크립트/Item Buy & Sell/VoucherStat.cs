using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VoucherStat : MonoBehaviour
{
    GameManager gameManager;
    SaveManager saveManager;
    public VoucherType voucherType;

    public enum VoucherType
    {
        HPVoucher,
        MVoucher,
        TVoucher,
    }
    OverOrClear overorclear;
    public int price;
    public TextMeshProUGUI pricetag;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        saveManager = FindObjectOfType<SaveManager>();
        overorclear = FindObjectOfType<OverOrClear>();
        if(gameManager.playerData.handcount + gameManager.playerData.trashcount > 8)
        {
            int baseIncrement = 10;
            int incrementStep = 5;
            int currentIncrement = baseIncrement;
            
            for(int i = 0; i < gameManager.playerData.handcount + gameManager.playerData.trashcount + (gameManager.playerData.moneyLimit/5) - 8; i++)
            {
                price += currentIncrement;
                // Increase the increment by 5 for the next iteration
                currentIncrement += incrementStep;
            }
        }
        pricetag.text = "$" + price.ToString();
    }
}
