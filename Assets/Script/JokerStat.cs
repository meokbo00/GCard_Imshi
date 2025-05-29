using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JokerStat : MonoBehaviour
{
    public TextMeshProUGUI pricetag;
    public int price;
    public string skill;
    void Start()
    {
        pricetag.text = "$" + price.ToString();
    }
}
