using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaroStat : MonoBehaviour
{
    public TextMeshProUGUI pricetag;
    public int price;
    public string skill;
    void Start()
    {
        pricetag.text = "$" + price.ToString();
    }
}
