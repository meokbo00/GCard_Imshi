using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TextEffect : MonoBehaviour
{
    [Header("UI 참조")]
    public TextMeshProUGUI PlanetName;
    public TextMeshProUGUI PlanetExplain;

    [Header("타이핑 효과 설정")]
    [Tooltip("글자당 표시 시간 (초)")]
    public float charPerSecond = 0.05f;
    [Tooltip("이펙트 시작 전 대기 시간")]
    public float startDelay = 0.5f;
    [Tooltip("제목과 설명 사이의 대기 시간")]
    public float betweenTextDelay = 0.3f;

    private string targetNameText = "";
    private string targetExplainText = "";
    private Tween nameTween;
    private Tween explainTween;

    private void OnDestroy()
    {
        // 모든 트윈 정리
        nameTween?.Kill();
        explainTween?.Kill();
    }

    public void StartTypingEffect(string name, string explain)
    {
        // 기존 트윈 정리
        nameTween?.Kill();
        explainTween?.Kill();

        // 텍스트 초기화
        targetNameText = name;
        targetExplainText = explain;
        PlanetName.text = "";
        PlanetExplain.text = "";

        // 타이핑 효과 시작
        StartCoroutine(TypingRoutine());
    }

    private IEnumerator TypingRoutine()
    {
        // 시작 전 대기
        yield return new WaitForSeconds(startDelay);

        // 제목 타이핑 효과 (TextMeshProUGUI용 DOText 사용)
        nameTween = DOTween.To(
            () => "",
            x => PlanetName.text = x,
            targetNameText,
            targetNameText.Length * charPerSecond
        ).SetEase(Ease.Linear)
        .OnComplete(() => {
            // 제목 타이핑 완료 후 설명 타이핑 시작
            StartCoroutine(StartExplainTyping());
        });
    }

    private IEnumerator StartExplainTyping()
    {
        // 설명 타이핑 전 대기
        yield return new WaitForSeconds(betweenTextDelay);

        // 설명 타이핑 효과 (TextMeshProUGUI용 DOText 사용)
        explainTween = DOTween.To(
            () => "",
            x => PlanetExplain.text = x,
            targetExplainText,
            targetExplainText.Length * charPerSecond
        ).SetEase(Ease.Linear);
    }

    // 즉시 모든 텍스트 표시 (스킵용)
    public void ShowAllTextImmediately()
    {
        // 기존 트윈 정지
        nameTween?.Kill();
        explainTween?.Kill();

        // 즉시 모든 텍스트 표시
        PlanetName.text = targetNameText;
        PlanetExplain.text = targetExplainText;
    }
}
