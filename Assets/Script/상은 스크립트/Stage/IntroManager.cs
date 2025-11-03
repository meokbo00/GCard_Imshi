using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public Image IntroImage;
    
    private void Start()
    {
        IntroImage.gameObject.SetActive(true);
        Color imageColor = IntroImage.color;
        imageColor.a = 0f;
        IntroImage.color = imageColor;

        // 페이드 인, 대기, 페이드 아웃 시퀀스
        Sequence introSequence = DOTween.Sequence();
        
        // 1. 페이드 인 (1초)
        introSequence.Append(IntroImage.DOFade(1f, 2f).SetEase(Ease.InOutQuad));
        
        // 2. 2초 대기
        introSequence.AppendInterval(2f);
        
        // 3. 페이드 아웃 (1초)
        introSequence.Append(IntroImage.DOFade(0f, 2f).SetEase(Ease.InOutQuad));
        
        // 4. 완료 후 비활성화
        introSequence.OnComplete(() => {
            IntroImage.gameObject.SetActive(false);
            SceneManager.LoadScene("MainMenu");
        });
    }
}