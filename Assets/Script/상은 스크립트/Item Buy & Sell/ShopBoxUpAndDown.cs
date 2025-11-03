using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShopBoxUpAndDown : MonoBehaviour
{
    public GameObject ShopBox;
    public GameObject ItemSelectZone;
    public GameObject MoneyPackZone;
    public bool isShopBoxDown = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void ItemSelectZoneDown()
    {
        if (ItemSelectZone != null)
        {
            Debug.Log("ItemSelectZone를 아래로 내립니다.");
            // 현재 위치에서 y축으로 -30만큼 1초 동안 이동
            ItemSelectZone.transform.DOLocalMoveY(-10f, 0.5f)
                .SetRelative(true)  // 상대적 이동
                .SetEase(Ease.OutQuad);  // 부드러운 감속 효과
        }
    }
    public void MoneyPackZoneDown()
    {
        if (MoneyPackZone != null)
        {
            Debug.Log("MoneyPackZone를 아래로 내립니다.");
            // 현재 위치에서 y축으로 -30만큼 1초 동안 이동
            MoneyPackZone.transform.DOLocalMoveY(-10f, 0.5f)
                .SetRelative(true)  // 상대적 이동
                .SetEase(Ease.OutQuad);  // 부드러운 감속 효과
        }
    }
    // ShopBox를 아래로 빠르게 내리는 메서드
    public void MoveShopBoxDown()
    {
        if (ShopBox != null)
        {
            Debug.Log("ShopBox를 아래로 내립니다.");
            // 현재 위치에서 y축으로 -30만큼 1초 동안 이동
            ShopBox.transform.DOLocalMoveY(-30f, 0.5f)
                .SetRelative(true)  // 상대적 이동
                .SetEase(Ease.OutQuad);  // 부드러운 감속 효과
            isShopBoxDown = true;
        }
    }
    // ShopBox를 위로 빠르게 내리는 메서드
    public void MoveShopBoxUp()
    {
        if (ShopBox != null)
        {
            Debug.Log("ShopBox를 위로 올립니다.");
            // 현재 위치에서 y축으로 -30만큼 1초 동안 이동
            ShopBox.transform.DOLocalMoveY(30f, 0.5f)
                .SetRelative(true)  // 상대적 이동
                .SetEase(Ease.OutQuad);  // 부드러운 감속 효과
            isShopBoxDown = false;
        }
    }
}
