using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 이벤트 시스템 추가

// 클릭 가능한 오브젝트를 위한 인터페이스
public interface IClickable
{
    void OnClick();
}

public class InfoAndOptions : MonoBehaviour
{
    public GameObject[] RunInfo;
    public GameObject[] InfoExplain;
    public GameObject runInfoBox;
    public Button backButton;
    private Dictionary<GameObject, GameObject> infoToDetailMap; // Info와 Detail 매핑을 위한 딕셔너리

    private void Start()
    {
        // 게임 시작 시 런 정보창을 비활성화 (기본값)
        if (runInfoBox != null)
        {
            runInfoBox.SetActive(false);
        }
        
        // 뒤로가기 버튼을 눌렀을때 OnBackButtonClick 메소드 호출
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClick);
        }

        // Info와 Detail 매핑 설정
        InitializeInfoToDetailMap();
        
        // 각 Info 오브젝트에 클릭 이벤트 추가
        AddClickEventsToInfoObjects();
    }

    private void InitializeInfoToDetailMap()
    {
        infoToDetailMap = new Dictionary<GameObject, GameObject>();
        
        // Info와 Detail 매핑 설정
        if (RunInfo.Length == InfoExplain.Length)
        {
            for (int i = 0; i < RunInfo.Length; i++)
            {
                infoToDetailMap[RunInfo[i]] = InfoExplain[i];
            }
        }
    }

    private void AddClickEventsToInfoObjects()
    {
        foreach (GameObject info in RunInfo)
        {
            if (info != null)
            {
                // 이벤트 트리거 컴포넌트 추가
                EventTrigger trigger = info.GetComponent<EventTrigger>();
                if (trigger == null)
                {
                    trigger = info.AddComponent<EventTrigger>();
                }

                // 클릭 이벤트 추가
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((data) => OnInfoClick(info));
                trigger.triggers.Add(entry);
            }
        }
    }

    private void OnInfoClick(GameObject clickedInfo)
    {
        if (infoToDetailMap.TryGetValue(clickedInfo, out GameObject detail))
        {
            // 모든 Detail 비활성화
            foreach (GameObject d in InfoExplain)
            {
                if (d != null)
                {
                    d.SetActive(false);
                }
            }

            // 클릭된 Info에 해당하는 Detail 활성화
            if (detail != null)
            {
                detail.SetActive(true);
            }
        }
    }

    // 게임에서 런 정보 버튼을 눌렀을 때 실행되는 내용
    public void OnRunInfoButtonClick()
    {
        if (runInfoBox != null)
        {
            runInfoBox.SetActive(true);

            // 런 정보창을 인 게임 화면 위로 올림
            runInfoBox.transform.DOLocalMoveY(1030f, 0.5f).SetEase(Ease.OutQuad); 
        }
    }

    // 런 정보창 내 뒤로가기 버튼을 눌렀을 때 실행되는 내용
    private void OnBackButtonClick()
    {
        if (runInfoBox != null)
        {
            // 런 정보창을 인 게임 화면 밑으로 내림
            runInfoBox.transform.DOLocalMoveY(0f, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                // 런 정보창 비활성화
                runInfoBox.SetActive(false);
            });
        }

        // 뒤로가기 시 모든 Detail 비활성화
        foreach (GameObject detail in InfoExplain)
        {
            if (detail != null)
            {
                detail.SetActive(false);
            }
        }
    }
}