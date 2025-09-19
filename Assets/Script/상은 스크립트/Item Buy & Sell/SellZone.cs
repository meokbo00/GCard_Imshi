using UnityEngine;
using TMPro;
using System.Linq;

public class SellZone : MonoBehaviour
{
    public TextMeshProUGUI SellText;
    public TextMeshProUGUI Itemcost;
    
    // 유효한 태그 목록
    private readonly string[] validTags = 
    {
        "BuyJoker",
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
                // JokerStat 컴포넌트 중에서 가격 정보 가져오기
                int price = 0;
                bool hasPrice = false;
                
                // JokerStat 확인
                var jokerStat = draggingItem.GetComponent<JokerStat>();
                if (jokerStat != null)
                {
                    price = jokerStat.price;
                    hasPrice = true;
                }
                
                // 가격이 있는 경우에만 표시 (판매 가격은 원가의 절반, 홀수일 경우 1을 뺌)
                if (hasPrice)
                {
                    int sellPrice = price / 2;
                    if (price % 2 != 0) // 홀수인 경우
                    {
                        sellPrice = (price - 1) / 2;
                    }
                    Itemcost.text = "($" + sellPrice.ToString() + ")";
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
                // 판매 영역 안에 있음을 표시
                dragItem.isInSellZone = true;
                SellText.gameObject.SetActive(false);
                Itemcost.gameObject.SetActive(false);
                // 판매 요청 메시지 출력
                Debug.Log($"({dragItem.tag}) 판매요청!");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 충돌이 끝난 오브젝트가 DragItem인지 확인
        var dragItem = other.GetComponent<DragItem>();
        if (dragItem != null)
        {
            // 판매 영역을 벗어났음을 표시
            SellText.gameObject.SetActive(true);
            Itemcost.gameObject.SetActive(true);
            dragItem.isInSellZone = false;
        }
    }

    // 판매 처리 메서드 (DragItem에서 호출)
    public void ProcessSell(GameObject soldItem)
    {
        // 판매 가격 계산
        int price = 0;
        bool hasPrice = false;
        
        // JokerStat 중에서 가격 정보 가져오기
        var jokerStat = soldItem.GetComponent<JokerStat>();
        
        if (jokerStat != null)
        {
            price = jokerStat.price;
            hasPrice = true;
        }
        
        if (hasPrice)
        {
            // 판매 가격 계산 (원가의 절반, 홀수일 경우 1을 뺌)
            int sellPrice = price / 2;
            if (price % 2 != 0) // 홀수인 경우
            {
                sellPrice = (price - 1) / 2;
            }
            
            // GameManager를 통해 판매 처리
            var gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                Debug.Log($"판매성공! 판매가격: {sellPrice}");
                gameManager.SellItem(sellPrice);
            }
            
            // 저장된 데이터에서 아이템 제거
            var itemData = FindObjectOfType<ItemData>();
            if (itemData != null)
            {
                itemData.RemoveItemFromSavedData(soldItem);
            }
            
            // 아이템 제거
            Destroy(soldItem);
        }
    }
}
