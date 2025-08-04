using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OverOrClear : MonoBehaviour
{
    public GameObject CashOutBox;
    public GameObject FailBox;
    public GameObject ShopBox;

    public GameObject PlayBtn;
    public GameObject CardZone;
    
    HandRanking handRanking;
    GameManager gameManager;
    GameSaveData gameSaveData;
    CashOutManager cashOutManager;
    void Start()
    {
        gameSaveData = FindObjectOfType<GameSaveData>();
        cashOutManager = FindObjectOfType<CashOutManager>();
        gameManager = FindObjectOfType<GameManager>();
        handRanking = FindObjectOfType<HandRanking>();
        CashOutBox.SetActive(false);
        FailBox.SetActive(false);
    }
    public void IsClearOrFail()
    {
        Debug.Log("게임 클리어/오버 체크");
        if (handRanking.sumPoint >= gameManager.goalPoints[gameManager.playerData.round - 1])
        {
            Debug.Log("스테이지 클리어!");
            StartCoroutine(HideUIElementsAndShowBox(CashOutBox));
        }
        else if (gameManager.playerData.handcount == 0 && (handRanking.sumPoint < gameManager.goalPoints[gameManager.playerData.round - 1]))
        {
            Debug.Log("스테이지 오버!");
            StartCoroutine(HideUIElementsAndShowBox(FailBox));
        }
    }
    
    private IEnumerator HideUIElementsAndShowBox(GameObject boxToShow)
    {
        // 모든 UI 요소들 모음
        List<GameObject> uiElements = new List<GameObject>();
        
        // PlayBtn과 CardZone 추가
        if (PlayBtn != null && PlayBtn.activeSelf) uiElements.Add(PlayBtn);
        if (CardZone != null && CardZone.activeSelf) uiElements.Add(CardZone);
        
        // 활성화된 카드들 추가
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");
        foreach (var card in cards)
        {
            if (card != null && card.activeSelf)
            {
                uiElements.Add(card);
            }
        }
        
        // 모든 UI 요소들을 아래로 내리는 애니메이션
        int completedTweens = 0;
        int totalTweens = uiElements.Count;
        
        // 애니메이션이 모두 완료되었는지 확인하는 플래그
        bool allAnimationsCompleted = false;
        
        foreach (var element in uiElements)
        {
            // PlayBtn은 170유닛, 나머지는 10유닛 내리기
            float moveDistance = (element == PlayBtn) ? 170f : 10f;
            Vector3 targetPos = element.transform.position - new Vector3(0, moveDistance, 0);
            
            element.transform.DOMoveY(targetPos.y, 0.5f)
                .SetEase(Ease.InQuad)
                .SetUpdate(UpdateType.Normal, true)
                .OnComplete(() => 
                {
                    element.SetActive(false);
                    completedTweens++;
                    
                    // 모든 트윈이 완료되었는지 확인
                    if (completedTweens >= totalTweens)
                    {
                        allAnimationsCompleted = true;
                    }
                });
        }
        
        // 모든 애니메이션이 완료될 때까지 대기
        yield return new WaitUntil(() => allAnimationsCompleted || uiElements.Count == 0);
        
        // 결과 박스 표시
        StartCoroutine(ShowBoxWithAnimation(boxToShow));
    }

    private IEnumerator ShowBoxWithAnimation(GameObject box)
    {
        // Rigidbody2D 캐싱
        Rigidbody2D rb = box.GetComponent<Rigidbody2D>();
        
        // 애니메이션 전 물리 설정 저장
        RigidbodyConstraints2D originalConstraints = rb?.constraints ?? RigidbodyConstraints2D.None;
        bool originalGravity = false;
        float originalDrag = 0f;
        
        if (rb != null)
        {
            originalGravity = rb.gravityScale > 0;
            originalDrag = rb.drag;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.gravityScale = 0f;
            rb.drag = 0f;
        }
        
        // 박스 활성화
        box.SetActive(true);
        
        // 현재 위치에서 7유닛 위로 이동할 타겟 위치 계산
        Vector3 currentPos = box.transform.position;
        Vector3 targetPos = currentPos + new Vector3(0, 7f, 0);
        
        // DOTween 애니메이션 설정
        float duration = 0.7f; // 애니메이션 지속 시간을 약간 늘림
        
        // 부드러운 움직임을 위한 Ease 함수 사용 (OutCubic이 더 부드럽게 느껴질 수 있음)
        box.transform.DOMoveY(targetPos.y, duration)
            .SetEase(Ease.OutCubic)
            .SetUpdate(UpdateType.Normal, true) // TimeScale 영향 받지 않도록 설정
            .SetLink(box); // 오브젝트가 파괴되면 자동으로 트윈 정리
            
        // 애니메이션 완료 대기
        yield return new WaitForSeconds(duration);
        
        // 원래 물리 설정 복원
        if (rb != null)
        {
            rb.constraints = originalConstraints;
            rb.gravityScale = originalGravity ? 1f : 0f;
            rb.drag = originalDrag;
        }
    }

    // 머니박스와 실패박스에 달린 버튼을 눌렀을 때 실행되는 메서드
    public void OnCashOutButton()
    {
        gameManager.playerData.money += cashOutManager.totalmoney;
        gameManager.UpdateUI();
        StartCoroutine(HideCashOutAndShowShop());
    }
    
    private IEnumerator HideCashOutAndShowShop()
    {
        // CashOutBox를 아래로 내리기
        Vector3 cashOutStartPos = CashOutBox.transform.position;
        Vector3 cashOutEndPos = cashOutStartPos - new Vector3(0, 7f, 0);
        
        // 아래로 부드럽게 이동
        yield return CashOutBox.transform.DOMoveY(cashOutEndPos.y, 0.5f)
            .SetEase(Ease.InCubic)
            .SetUpdate(UpdateType.Normal, true)
            .WaitForCompletion();
            
        // CashOutBox 비활성화
        CashOutBox.SetActive(false);
        
        // ShopBox 활성화 및 7유닛 위로 이동
        ShopBox.SetActive(true);
        Vector3 shopBoxStartPos = ShopBox.transform.position;
        Vector3 shopBoxTargetPos = shopBoxStartPos + new Vector3(0, 7f, 0);
        
        // ShopBox를 7유닛 위로 부드럽게 이동
        yield return ShopBox.transform.DOMoveY(shopBoxTargetPos.y, 0.7f)
            .SetEase(Ease.OutCubic)
            .SetUpdate(UpdateType.Normal, true)
            .WaitForCompletion();
    }
    
    // 이전 DisableAllCards 메서드는 HideUIElementsAndShowBox로 대체되어 제거됨
    }