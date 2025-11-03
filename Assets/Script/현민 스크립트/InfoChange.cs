using UnityEngine;
using UnityEngine.UI; 

public class InfoChange : MonoBehaviour
{
    // 런 정보창에서 띄울 각각의 정보 창(게임 오브젝트)
    public GameObject PokerHandManager;
    public GameObject StageManager;
    public GameObject RoundManager;
    
    // 포커 핸드 정보 창의 세부 설명 이미지 배열 (InfoAndOptions 컴포넌트에서 가져옴)
    public GameObject[] DetailImages;

    // InfoAndOptions 컴포넌트를 가져오기 위한 변수 선언
    public InfoAndOptions infoAndOptions;

    // 위치를 변경할 UI Image (RectTransform 컴포넌트 필요)
    public RectTransform CurrentSelect;

    void Start()
    {
        // InfoAndOptions 인스턴스를 통해 InfoExplain 배열 가져오기
        if (infoAndOptions != null)
        {
            DetailImages = infoAndOptions.InfoExplain;
        }
        else
        {
            Debug.LogError("InfoAndOptions 인스턴스 초기화 오류");
        }

        // 시작 시 PokerHandInfo를 기본값으로 설정
        ShowPokerHandInfo(); 
    }

    // 라운드 정보 창을 띄우는 메서드
    public void ShowRoundInfo()
    {
        // 모든 Detail 이미지를 비활성화
        foreach (GameObject detail in DetailImages)
        {
            if (detail != null)
            {
                detail.SetActive(false);
            }
        }
        
        // 라운드 정보 창을 띄우고 다른 정보 창은 비활성화
        PokerHandManager.SetActive(false);
        StageManager.SetActive(false);
        RoundManager.SetActive(true);

        // 라운드 정보 창을 띄웠다는 것을 나타내는 아이콘(역삼각형) 위치 변경
        Vector2 currentPos = CurrentSelect.anchoredPosition;
        CurrentSelect.anchoredPosition = new Vector2(136f, currentPos.y);
    }

    // 스테이지 정보 창을 띄우는 메서드
    public void ShowStageInfo()
    {
        // 모든 Detail 이미지를 비활성화
        foreach (GameObject detail in DetailImages)
        {
            if (detail != null)
            {
                detail.SetActive(false);
            }
        }
        
        // 스테이지 정보 창을 띄우고 다른 정보 창은 비활성화
        PokerHandManager.SetActive(false);
        RoundManager.SetActive(false);
        StageManager.SetActive(true);

        // 스테이지 정보 창을 띄웠다는 것을 나타내는 아이콘(역삼각형) 위치 변경
        Vector2 currentPos = CurrentSelect.anchoredPosition;
        CurrentSelect.anchoredPosition = new Vector2(467f, currentPos.y);
    }

    // 포커 핸드 정보 창을 띄우는 메서드
    public void ShowPokerHandInfo()
    {
        Debug.Log("포커 핸드 정보창 실행!");

        // 모든 Detail 이미지를 비활성화
        foreach (GameObject detail in DetailImages)
        {
            if (detail != null)
            {
                detail.SetActive(false);
            }
        }
        
        // 포커 핸드 정보 창을 띄우고 다른 정보 창은 비활성화
        RoundManager.SetActive(false);
        StageManager.SetActive(false);
        PokerHandManager.SetActive(true);

        // 포커 핸드 정보 창을 띄웠다는 것을 나타내는 아이콘(역삼각형) 위치 변경
        Vector2 currentPos = CurrentSelect.anchoredPosition;
        CurrentSelect.anchoredPosition = new Vector2(-193f, currentPos.y);
    }
}
