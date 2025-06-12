using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OptionMenuController : MonoBehaviour
{
    public GameObject OptionPanel;

    public void ShowOption()
    {
        OptionPanel.SetActive(true);

        RectTransform rectTransform = OptionPanel.GetComponent<RectTransform>();

        // 초기 위치를 살짝 아래로 설정
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);

        // Y축으로 500만큼 부드럽게 올라오는 애니메이션
        rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + 500, 0.5f).SetEase(Ease.OutCubic);
    }
    public void HideOption()
    {
        RectTransform rectTransform = OptionPanel.GetComponent<RectTransform>();

        rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y - 500, 0.5f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                OptionPanel.SetActive(false);
            });
    }


}
