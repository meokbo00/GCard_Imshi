using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StartSceneManager : MonoBehaviour
{
    public Image BlackImage;
    public GameObject title;
    public GameObject BGBox;
    public GameObject playButton;
    public GameObject optionButton;
    public GameObject exitButton;
    public GameObject CollectionButton;
    public GameObject CreditButton;

private void Start()
{
    // 시작 시 타이틀을 안보이게 설정
    if (title != null)
    {
        // 타이틀 초기화
        title.transform.localScale = Vector3.zero;
        
        // BGBox 초기화 (비활성화)
        if (BGBox != null)
        {
            BGBox.SetActive(false);
            var bgImage = BGBox.GetComponent<Image>();
            if (bgImage != null)
            {
                // 원래 색상 저장
                Color originalColor = bgImage.color;
                bgImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
            }
        }
        
        // BlackImage 초기화 및 페이드 아웃
        if (BlackImage != null)
        {
            BlackImage.gameObject.SetActive(true);
            // 알파값을 1로 설정 (완전 불투명)
            Color color = BlackImage.color;
            color.a = 1f;
            BlackImage.color = color;
            
            // 페이드 아웃 후 ShowTitleWithDelay 실행
            BlackImage.DOFade(0f, 3f)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => {
                    BlackImage.gameObject.SetActive(false);
                    StartCoroutine(ShowTitleWithDelay(2f));
                });
        }
    }
}
    
    private IEnumerator ShowTitleWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // 타이틀 애니메이션
        title.transform.DOScale(1.4f, 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => {
                title.transform.DOScale(1.35f, 0.2f)
                    .OnComplete(ShowBGBox);
            });
    }
    
    private void ShowBGBox()
    {
        if (BGBox != null)
        {
            BGBox.SetActive(true); // BGBox 활성화
            var bgImage = BGBox.GetComponent<Image>();
            if (bgImage != null)
            {
                // 0.8초 동안 페이드 인
                bgImage.DOFade(1f, 0.8f)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(ShowButtons); // 버튼 표시 시작
            }
        }
    }
    
    private void ShowButtons()
    {
        // 버튼들을 순차적으로 표시
        StartCoroutine(ShowButtonWithDelay(playButton, 0f));
        StartCoroutine(ShowButtonWithDelay(CollectionButton, 0.2f));
        StartCoroutine(ShowButtonWithDelay(optionButton, 0.4f));
        StartCoroutine(ShowButtonWithDelay(exitButton, 0.6f));
        StartCoroutine(ShowButtonWithDelay(CreditButton, 0.8f));
    }
    
    private IEnumerator ShowButtonWithDelay(GameObject button, float delay)
    {
        if (button == null) yield break;
        
        button.SetActive(true);
        var canvasGroup = button.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = button.AddComponent<CanvasGroup>();
        }
        
        // 초기 투명도 설정
        canvasGroup.alpha = 0f;
        
        // 딜레이 대기
        yield return new WaitForSeconds(delay);
        
        // 페이드 인 애니메이션
        canvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutQuad);
    }

}