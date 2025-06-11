using UnityEngine;
using UnityEngine.UI; 

public class InfoChange : MonoBehaviour
{
    // 런 정보창에서 띄울 각각의 정보 창(게임 오브젝트)
    public GameObject PokerHandManager;
    public GameObject VoucherManager;
    public GameObject StakeManager;
    public GameObject BlindManager;

    // 위치를 변경할 UI Image (RectTransform 컴포넌트 필요)
    public RectTransform CurrentSelect;

    // 시작 시 PokerHandInfo를 기본값으로로 설정
    void Start()
    {
        ShowPokerHandInfo(); 
    }

    // 블라인드 정보 창을 띄우는 메서드
    public void ShowBlindInfo()
    {
        // 블라인드 정보 창을 띄우고 다른 정보 창은 비활성화
        PokerHandManager.SetActive(false);
        VoucherManager.SetActive(false);
        StakeManager.SetActive(false);
        BlindManager.SetActive(true);

        // 블라인드 정보 창을 띄웠다는 것을 나타내는 아이콘(역삼각형) 위치 변경
        Vector2 currentPos = CurrentSelect.anchoredPosition;
        CurrentSelect.anchoredPosition = new Vector2(14f, currentPos.y);
    }

    // 교환한 바우처 정보 창을 띄우는 메서드
    public void ShowVoucherInfo()
    {
        // 교환한 바우처 정보 창을 띄우고 다른 정보 창은 비활성화
        PokerHandManager.SetActive(false);
        BlindManager.SetActive(false);
        StakeManager.SetActive(false);
        VoucherManager.SetActive(true);

        // 교환한 바우처 정보 창을 띄웠다는 것을 나타내는 아이콘(역삼각형) 위치 변경
        Vector2 currentPos = CurrentSelect.anchoredPosition;
        CurrentSelect.anchoredPosition = new Vector2(259f, currentPos.y);
    }

    // 스테이크 정보 창을 띄우는 메서드
    public void ShowStakeInfo()
    {
        // 스테이크 정보 창을 띄우고 다른 정보 창은 비활성화
        PokerHandManager.SetActive(false);
        BlindManager.SetActive(false);
        VoucherManager.SetActive(false);
        StakeManager.SetActive(true);

        // 스테이크 정보 창을 띄웠다는 것을 나타내는 아이콘(역삼각형) 위치 변경
        Vector2 currentPos = CurrentSelect.anchoredPosition;
        CurrentSelect.anchoredPosition = new Vector2(500f, currentPos.y);
    }

    // 포커 핸드 정보 창을 띄우는 메서드
    public void ShowPokerHandInfo()
    {
        // 포커 핸드 정보 창을 띄우고 다른 정보 창은 비활성화
        BlindManager.SetActive(false);
        VoucherManager.SetActive(false);
        StakeManager.SetActive(false);
        PokerHandManager.SetActive(true);

        // 포커 핸드 정보 창을 띄웠다는 것을 나타내는 아이콘(역삼각형) 위치 변경
        Vector2 currentPos = CurrentSelect.anchoredPosition;
        CurrentSelect.anchoredPosition = new Vector2(-235f, currentPos.y);
    }
}
