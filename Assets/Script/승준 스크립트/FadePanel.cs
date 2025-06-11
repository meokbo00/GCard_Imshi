using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadePanel : MonoBehaviour
{
    public float fadeDuration = 1f; // ���̵� ���� �ð�
    private Image panelImage;
    private float targetAlpha = 0f; // ��ǥ ���� ��

    void Start()
    {
        panelImage = GetComponent<Image>();
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        Color currentColor = panelImage.color;
        float startAlpha = currentColor.a;
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / fadeDuration);
            currentColor.a = alpha;
            panelImage.color = currentColor;
            yield return null;
        }
    }
}
