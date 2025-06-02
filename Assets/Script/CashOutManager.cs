using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;


// 캐시 아웃창이 생성되고 나서 얻을 재화의 경로와 재화의 양을 보여주는 스크립트
public class CashOutManager : MonoBehaviour
{
    public GameObject CashOutBox;
    public GameObject CashOutBtn;
    public GameObject BG;
    public GameObject DollarPrefab;
    public GameObject MinusPrefab;

    [Header("캐시아웃 버튼 텍스트에 출력될 총 돈의 값")]
    public TextMeshProUGUI BtnMoneyTxt;

    [Header("캐시아웃에서 출력될 텍스트 장소")]

    public GameObject ClearTxt;
    public GameObject RemainHandTxt;
    public GameObject InterestTxt;
    public GameObject JokerSkillTxt;
    
    [Header("캐시아웃에서 출력될 돈 오브젝트 장소")]
    public GameObject ClearM;
    public GameObject RemainHandM;
    public GameObject InterestM;
    public GameObject JokerSkillM;
    
    GameManager gameManager;

    [Header("이자 관련 설정")]
    [Tooltip("이자 한계값 (기본값: 5, money 25 이상일 때 최대치)")]
    public int interestMaxLimit = 5;
    [Tooltip("이자 증가 단위 (money 5당 1 증가)")]
    public int interestIncrementUnit = 5;
    
    [Header("캐시아웃에서 받을 돈의 값이 저장되는 변수")]
    public int clearreward = 0;
    public int remainhand = 0;
    public int interestmoney = 0;
    public int isjokerskill = 0;
    public int totalmoney = 0;
    
    
    private bool hasStartedSequence = false;

    void Start()
    {
        CashOutBtn.SetActive(false);
        gameManager = FindAnyObjectByType<GameManager>();
    }

    IEnumerator ActivateTextsSequentially()
    {
        // 모든 텍스트를 배열로 관리
        GameObject[] textObjects = { ClearTxt, RemainHandTxt, InterestTxt, JokerSkillTxt };
        
        RectTransform bgRect = BG?.GetComponent<RectTransform>();
        
        if (bgRect != null)
        {
            bgRect.anchoredPosition = new Vector2(bgRect.anchoredPosition.x, 2.7f);
            bgRect.sizeDelta = new Vector2(bgRect.sizeDelta.x, 300f);
        }
        
        foreach (var textObj in textObjects)
        {
            if (textObj != null)
            {
                textObj.SetActive(false);
                textObj.transform.localScale = Vector3.zero;
            }
        }

        yield return null;

        // 각 텍스트에 대해 순차적으로 애니메이션 적용
        for (int i = 0; i < textObjects.Length; i++)
        {
            var textObj = textObjects[i];
            if (textObj == null) continue;
            
            textObj.SetActive(true);
            
            // 커지면서 나타나는 애니메이션
            textObj.transform.DOScale(Vector3.one * 1f, 0.2f)
                .SetEase(Ease.OutBack);
                
            yield return new WaitForSeconds(0.2f);
            
            // 원래 크기로 돌아오는 애니메이션
            textObj.transform.DOScale(Vector3.one, 0.2f);
            
            // BG 애니메이션 (두 번째 텍스트부터 적용)
            if (i < textObjects.Length - 1 && bgRect != null)
            {
                // 다음 BG 위치와 크기 설정
                float targetY = 0f;
                float targetHeight = 0f;
                
                switch (i + 1) // 다음 인덱스에 해당하는 값으로 설정 PosY 0.4 증가할때 Height은 80만큼 증가가
                {
                    case 1: // 두 번째 텍스트 (첫 번째 인덱스)
                        targetY = 2.3f;
                        targetHeight = 378.82f;
                        break;
                    case 2: // 세 번째 텍스트
                        targetY = 1.918f;
                        targetHeight = 456.219f;
                        break;
                    case 3: // 네 번째 텍스트
                        targetY = 1.41f;
                        targetHeight = 557.977f;
                        break;
                }
                
                // BG 애니메이션 적용
                bgRect.DOAnchorPosY(targetY, 0.3f).SetEase(Ease.OutQuad);
                bgRect.DOSizeDelta(new Vector2(bgRect.sizeDelta.x, targetHeight), 0.3f).SetEase(Ease.OutQuad);
            }
            
            // 다음 애니메이션까지 대기
            yield return new WaitForSeconds(0.5f);
        }

        CashOutBtn.SetActive(true);
    }



    void Update()
    {
        if (CashOutBox != null && CashOutBox.activeInHierarchy && !hasStartedSequence)
        {
            hasStartedSequence = true;
            ShowTotalMoney();
            StartCoroutine(DelayedStartSequence());
        }
    }
    
    public void ShowTotalMoney()
    {
        // money를 단위로 나누어 interest 계산 (5당 1 증가, 최대 interestMaxLimit)
        interestmoney = Mathf.Min(
            gameManager.money / interestIncrementUnit,  // money를 단위로 나눈 값
            interestMaxLimit                            // 최대값 제한
        );
        
        // 0 이하인 경우 0으로 설정 (옵션: 필요에 따라 제거 가능)
        interestmoney = Mathf.Max(0, interestmoney);

        remainhand = gameManager.handcount;
        totalmoney = clearreward + remainhand + interestmoney + isjokerskill;
        BtnMoneyTxt.GetComponent<TextMeshProUGUI>().text = "$" + totalmoney.ToString();
    }
    
    IEnumerator DelayedStartSequence()
    {
        // 1초 대기 후 시퀀스 시작
        yield return new WaitForSeconds(0.7f);
        StartCoroutine(ActivateTextsSequentially());
    }
}
