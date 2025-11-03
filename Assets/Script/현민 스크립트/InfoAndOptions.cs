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

                // 누르는 동안 Detail 표시: PointerDown
                EventTrigger.Entry down = new EventTrigger.Entry();
                down.eventID = EventTriggerType.PointerDown;
                down.callback.AddListener((data) => ShowDetail(info));
                trigger.triggers.Add(down);

                // 손을 뗄 때/마우스가 벗어날 때 Detail 숨김: PointerUp, PointerExit
                EventTrigger.Entry up = new EventTrigger.Entry();
                up.eventID = EventTriggerType.PointerUp;
                up.callback.AddListener((data) => HideAllDetails());
                trigger.triggers.Add(up);

                EventTrigger.Entry exit = new EventTrigger.Entry();
                exit.eventID = EventTriggerType.PointerExit;
                exit.callback.AddListener((data) => HideAllDetails());
                trigger.triggers.Add(exit);
            }
        }
    }

    // 길게 누르는 동안 Detail을 보여줌
    private void ShowDetail(GameObject info)
    {
        if (infoToDetailMap.TryGetValue(info, out GameObject detail))
        {
            foreach (GameObject d in InfoExplain)
            {
                if (d != null) d.SetActive(false);
            }
            if (detail != null) detail.SetActive(true);
        }
    }

    // 손을 떼거나 포인터가 벗어나면 Detail을 모두 숨김
    private void HideAllDetails()
    {
        foreach (GameObject d in InfoExplain)
        {
            if (d != null) d.SetActive(false);
        }
    }

    // 게임에서 런 정보 버튼을 눌렀을 때 실행되는 내용
    public void OnRunInfoButtonClick()
    {
        Debug.Log("런 정보창 실행!");

        if (runInfoBox != null)
        {
            runInfoBox.SetActive(true);

            // 런 정보창을 인 게임 화면 위로 올림
            runInfoBox.transform.DOLocalMoveY(1030f, 1f)
            .SetEase(Ease.OutBack); 
        }
    }

    // 런 정보창 내 뒤로가기 버튼을 눌렀을 때 실행되는 내용
    private void OnBackButtonClick()
    {
        if (runInfoBox != null)
        {
            Debug.Log("런 정보 창 종료!");
            
            // 런 정보창을 인 게임 화면 밑으로 내림
            runInfoBox.transform.DOLocalMoveY(0f, 1f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
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