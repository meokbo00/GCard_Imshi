using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class ShowSellBuyZone : MonoBehaviour
{
    public GameObject BuyZone;
    public GameObject SellZone;
    
    private Tween scaleTween;
    private Tween pulseTween;
    private const float ANIM_DURATION = 0.4f; // 기본 애니메이션 지속 시간 (늘림)
    private const float PULSE_DURATION = 1.2f; // 맥박 애니메이션 주기 (늘림)
    private bool isDraggingValidItem = false;
    
    // BuyZone을 활성화할 태그 목록
    private readonly HashSet<string> buyValidTags = new HashSet<string>
    {
        "Joker",
        "ItemPack",
        "Planet",
        "Taro",
        "Voucher"
    };
    
    private readonly HashSet<string> sellValidTags = new HashSet<string>
    {
        "BuyJoker",
        "BuyPlanet",
        "BuyTaro"
    };

    private void Awake()
    {
        if (BuyZone != null)
        {
            // 초기 상태 설정 (작은 크기로 시작)
            BuyZone.transform.localScale = Vector3.zero;
            BuyZone.SetActive(false);
            
            if (SellZone != null)
            {
                SellZone.transform.localScale = Vector3.zero;
                SellZone.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지를 위해 트윈 정리
        scaleTween?.Kill();
        pulseTween?.Kill();
    }

    void Update()
    {
        // 매 프레임 모든 DragItem 컴포넌트 찾기
        var dragItems = FindObjectsOfType<DragItem>();
        
        if (dragItems.Length == 0)
        {
            return;
        }

        // 모든 DragItem 중 유효한 태그를 가진 아이템이 드래그 중인지 확인
        var draggingItem = dragItems.FirstOrDefault(item => 
            item.isDrag && !string.IsNullOrEmpty(item.currentTag));
            
        if (draggingItem != null)
        {
            // BuyZone 또는 SellZone 중 어떤 것을 보여줄지 결정
            if (buyValidTags.Contains(draggingItem.currentTag) && !isDraggingValidItem)
            {
                // BuyZone 표시
                isDraggingValidItem = true;
                ShowBuyZone();
            }
            else if (sellValidTags.Contains(draggingItem.currentTag) && !isDraggingValidItem)
            {
                // SellZone 표시
                isDraggingValidItem = true;
                ShowSellZone();
            }
        }
        else if (isDraggingValidItem)
        {
            // 드래그 종료
            isDraggingValidItem = false;
            StopPulseAnimation();
            HideBuyZone();
            HideSellZone();
        }
    }

    private void ShowBuyZone()
    {
        if (BuyZone == null || BuyZone.activeSelf) return;

        // 기존 트윈 중지
        scaleTween?.Kill();
        
        // BuyZone 활성화 (크기는 0으로 시작)
        BuyZone.transform.localScale = Vector3.zero;
        BuyZone.SetActive(true);
        
        // 부드럽게 튀어나오는 효과 (OutQuad로 더 부드럽게)
        scaleTween = BuyZone.transform.DOScale(1f, ANIM_DURATION)
            .SetEase(Ease.OutQuad)
            .OnComplete(StartPulseAnimation);
    }

    private void StartPulseAnimation()
    {
        if (BuyZone == null || !BuyZone.activeSelf) return;
        
        // 기존 펄스 애니메이션 중지
        StopPulseAnimation();
        
        // 아주 미세하게 커졌다 작아지는 효과 반복 (크기 변화량을 1.01f로 줄임)
        pulseTween = BuyZone.transform.DOScale(1.01f, PULSE_DURATION * 0.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
    
    private void StopPulseAnimation()
    {
        pulseTween?.Kill();
        pulseTween = null;
    }

    private void HideBuyZone()
    {
        if (BuyZone == null || !BuyZone.activeSelf) return;

        scaleTween?.Kill();
        BuyZone.transform.DOScale(0f, ANIM_DURATION * 0.5f)
            .SetEase(Ease.InQuad)
            .OnComplete(() => BuyZone.SetActive(false));
    }
    
    private void HideSellZone()
    {
        if (SellZone == null || !SellZone.activeSelf) return;

        scaleTween?.Kill();
        SellZone.transform.DOScale(0f, ANIM_DURATION * 0.5f)
            .SetEase(Ease.InQuad)
            .OnComplete(() => SellZone.SetActive(false));
    }
    
    private void ShowSellZone()
    {
        if (SellZone == null || SellZone.activeSelf) return;

        // 기존 트윈 중지
        scaleTween?.Kill();
        
        // SellZone 활성화 (크기는 0으로 시작)
        SellZone.transform.localScale = Vector3.zero;
        SellZone.SetActive(true);
        
        // 부드럽게 튀어나오는 효과 (OutQuad로 더 부드럽게)
        scaleTween = SellZone.transform.DOScale(1f, ANIM_DURATION)
            .SetEase(Ease.OutQuad)
            .OnComplete(StartPulseAnimation);
    }
}