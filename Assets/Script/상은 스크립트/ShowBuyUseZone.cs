using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class ShowBuyUseZone : MonoBehaviour
{
    
    public GameObject BuyZone;
    private Tween scaleTween;
    private Tween pulseTween;
    private const float ANIM_DURATION = 0.4f; // 기본 애니메이션 지속 시간 (늘림)
    private const float PULSE_DURATION = 1.2f; // 맥박 애니메이션 주기 (늘림)
    private bool isDraggingValidItem = false;
    
    // BuyZone을 활성화할 태그 목록
    private readonly HashSet<string> validTags = new HashSet<string>
    {
        "Joker",
        "ItemPack",
        "Planet",
        "Taro",
        "Voucher"
    };

    private void Awake()
    {
        if (BuyZone != null)
        {
            // 초기 상태 설정 (작은 크기로 시작)
            BuyZone.transform.localScale = Vector3.zero;
            BuyZone.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지를 위해 트윈 정리
        scaleTween?.Kill();
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
        bool anyValidDragging = dragItems.Any(item => 
            item.isDrag && 
            !string.IsNullOrEmpty(item.currentTag) && 
            validTags.Contains(item.currentTag));

        // 드래그 상태 변화 감지
        if (anyValidDragging && !isDraggingValidItem)
        {
            // 드래그 시작 (ShowBuyZone 내부에서 StartPulseAnimation이 호출됨)
            isDraggingValidItem = true;
            ShowBuyZone();
        }
        else if (!anyValidDragging && isDraggingValidItem)
        {
            // 드래그 종료
            isDraggingValidItem = false;
            StopPulseAnimation();
            HideBuyZone();
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

        // 기존 트윈 중지
        scaleTween?.Kill();
        
        // 부드럽게 들어가는 효과 후 비활성화 (InQuad로 더 부드럽게)
        scaleTween = BuyZone.transform.DOScale(0f, ANIM_DURATION * 0.7f)
            .SetEase(Ease.InQuad)
            .OnComplete(() => {
                if (BuyZone != null)
                {
                    BuyZone.SetActive(false);
                }
            });
    }
}