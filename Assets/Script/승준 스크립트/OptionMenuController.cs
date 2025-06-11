using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OptionMenuController : MonoBehaviour
{
    public RectTransform optionPanel;
    public Vector2 targetPosition;
    public float duration = 0.5f;

    private Vector2 startPosition;

    private void Start()
    {
        startPosition = new Vector2(0, -Screen.height);
        optionPanel.anchoredPosition = startPosition;
        optionPanel.gameObject.SetActive(false);
    }

    public void ShowOption()
    {
        optionPanel.gameObject.SetActive(true);
        optionPanel.DOAnchorPos(targetPosition, duration).SetEase(Ease.OutCubic);
    }

    public void HideOption()
    {
        optionPanel.DOAnchorPos(startPosition, duration).SetEase(Ease.InCubic)
            .OnComplete(() => optionPanel.gameObject.SetActive(false));
    }
}