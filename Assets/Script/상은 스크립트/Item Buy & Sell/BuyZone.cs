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
                // JokerStat, TaroStat, PlanetStat 컴포넌트 중에서 가격 정보 가져오기
                int price = 0;
                bool hasPrice = false;
                
                // JokerStat 확인
                var jokerStat = draggingItem.GetComponent<JokerStat>();
                if (jokerStat != null)
                {
                    price = jokerStat.price;
                    hasPrice = true;
                }
                
                // TaroStat 확인 (JokerStat 상속이므로 else if가 아닌 if로 각각 체크)
                var taroStat = draggingItem.GetComponent<TaroStat>();
                if (taroStat != null)
                {
                    price = taroStat.price;
                    hasPrice = true;
                }
                
                // PlanetStat 확인 (JokerStat 상속이므로 else if가 아닌 if로 각각 체크)
                var planetStat = draggingItem.GetComponent<PlanetStat>();
                if (planetStat != null)
                {
                    price = planetStat.price;
                    hasPrice = true;
                }
                
                // 가격이 있는 경우에만 표시
                if (hasPrice)
                {
                    Itemcost.text = "($" + price.ToString() + ")";
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 오브젝트가 드래그 중인지 확인
        var dragItem = other.GetComponent<DragItem>();
        if (dragItem != null && dragItem.isDrag)
        {
            // 드래그 중인 오브젝트의 태그가 유효한지 확인
            if (validTags.Contains(dragItem.tag))
            {
                // 구매 영역 안에 있음을 표시
                dragItem.isInBuyZone = true;
                // 구매 요청 메시지 출력
                Debug.Log($"({dragItem.tag}) 구매요청!");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 충돌이 끝난 오브젝트가 DragItem인지 확인
        var dragItem = other.GetComponent<DragItem>();
        if (dragItem != null)
        {
            // 구매 영역을 벗어났음을 표시
            dragItem.isInBuyZone = false;
        }
    }
}