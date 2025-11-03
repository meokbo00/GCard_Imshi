using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
//지금까지는 몇초를 기다리고 텍스트를 표시했지만 이제부터는 stageexplainbox가 활성화된 후에 텍스트를 표시할 예정
public class TextEffect : MonoBehaviour
{
    CameraMoving cameraMoving;
    PlayerData playerData;
    SaveManager saveManager;
    public GameObject StageExplainBox;
    [Header("UI 참조")]
    public TextMeshProUGUI PlanetName;
    public TextMeshProUGUI PlanetExplain;
    public TextMeshProUGUI Status;
    public int[] goalPoints = { 10, 20, 30, 800, 1200, 1600, 2000, 3000, 4000, 5000, 7500, 10000, 11000, 16500, 22000, 20000, 27500, 35000, 35000, 52500, 70000, 50000, 75000, 100000, 110000, 165000, 220000, 560000, 840000, 2240000, 7200000, 10800000, 14400000, 300000000, 450000000, 600000000 };


    [Header("타이핑 효과 설정")]
    [Tooltip("글자당 표시 시간 (초)")]
    public float charPerSecond = 0.05f;
    [Tooltip("이펙트 시작 전 대기 시간")]
    public float startDelay = 0.5f;
    [Tooltip("제목과 설명 사이의 대기 시간")]
    public float betweenTextDelay = 0.3f;

    private string targetNameText = "";
    private string targetExplainText = "";
    private string statusText = "";
    private Tween nameTween;
    private Tween explainTween;
    private Tween statusTween;

    private void Awake()
    {
        cameraMoving = FindObjectOfType<CameraMoving>();
    }
    private void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        if (saveManager != null)
        {
            playerData = saveManager.Load();
            if (playerData != null && goalPoints != null && goalPoints.Length > 0)
            {
                // 현재 라운드를 3으로 나눈 몫을 구해서 3을 곱하면 그룹의 시작 인덱스가 나옵니다.
                // 예: round가 1,2,3 -> groupStart = 0, round가 4,5,6 -> groupStart = 3
                int groupStart = ((playerData.round - 1) / 3) * 3;
                
                // 각 그룹의 시작 라운드 번호 계산
                int round1 = groupStart + 1;
                int round2 = groupStart + 2;
                int round3 = groupStart + 3;
                
                // 인덱스가 배열 범위를 벗어나지 않도록 보정
                int index1 = Mathf.Min(groupStart, goalPoints.Length - 1);
                int index2 = Mathf.Min(groupStart + 1, goalPoints.Length - 1);
                int index3 = Mathf.Min(groupStart + 2, goalPoints.Length - 1);
                
                // 상태 텍스트 설정
                statusText = $"1라운드 : {goalPoints[index1]:N0}";
                statusText += $"\n2라운드 : {goalPoints[index2]:N0}";
                statusText += $"\n3라운드 : {goalPoints[index3]:N0}";
            }
        }
        Status.text = "";
    }
    private void OnDestroy()
    {
        // 모든 트윈 정리
        nameTween?.Kill();
        explainTween?.Kill();
        statusTween?.Kill();
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

        // 카메라 이동이 완료되고 explain box가 활성화될 때까지 대기
        yield return new WaitUntil(() => cameraMoving != null && cameraMoving.isexplainboxenable);

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
        ).SetEase(Ease.Linear)
        .OnComplete(() => {
            // 설명 타이핑 완료 후 상태 텍스트 타이핑 시작
            StartCoroutine(StartStatusTyping());
        });
    }
    
    private IEnumerator StartStatusTyping()
    {
        // 상태 텍스트 타이핑 전 대기
        yield return new WaitForSeconds(betweenTextDelay);
        
        // 상태 텍스트 타이핑 효과
        if (Status != null && !string.IsNullOrEmpty(statusText))
        {
            Status.text = "";
            statusTween = DOTween.To(
                () => "",
                x => Status.text = x,
                statusText,
                statusText.Length * charPerSecond
            ).SetEase(Ease.Linear);
        }
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
