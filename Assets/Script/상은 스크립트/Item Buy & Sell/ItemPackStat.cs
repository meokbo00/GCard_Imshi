using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ItemPackStat : MonoBehaviour
{
    public ItemPackType itemPackType;

    public enum ItemPackType
    {
        Joker2,
        Joker4,
        Money
    }
    OverOrClear overorclear;
    public int price;
    public TextMeshProUGUI pricetag;

    void Start()
    {
        overorclear = FindObjectOfType<OverOrClear>();
        pricetag.text = "$" + price.ToString();
    }


}
