using UnityEngine;
using TMPro;
using System.Linq;

public class BuyZone : MonoBehaviour
{
    public TextMeshProUGUI Itemcost;
    
    // 유효한 태그 목록
    private readonly string[] validTags = 
    {
        "Joker",
        "ItemPack",
        "Planet",
        "Taro",
        "Voucher"
    };

    private void Update()
    {
        // 현재 드래그 중인 오브젝트 찾기
        var dragItems = FindObjectsOfType<DragItem>();
        var draggingItem = dragItems.FirstOrDefault(item => item.isDrag);
        
        if (draggingItem != null && Itemcost != null)
        {
            // 드래그 중인 오브젝트의 태그가 유효한지 확인
            if (validTags.Contains(draggingItem.tag))
            {
                // JokerStat 컴포넌트 가져오기
                var jokerStat = draggingItem.GetComponent<JokerStat>();
                if (jokerStat != null)
                {
                    // 가격 표시 (예: "$100")
                    Itemcost.text = "($" + jokerStat.price.ToString() + ")";
                    return;
                }
            }
        }
        
        // 드래그 중이 아니거나 유효한 오브젝트가 아닌 경우 가격 표시 초기화
        if (Itemcost != null)
        {
            Itemcost.text = "";
        }
    }
}