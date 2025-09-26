using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class OptionManagerController : MonoBehaviour
{
    public GameObject optionManager;       // OptionManager 오브젝트
    public GameObject inGameOptionPanel;   // OptionManager 하위 InGameOptionPanel
    public GameObject soundPanel;          // OptionManager 하위 SoundPanel
    public float moveDistance = 900f;      // 이동 거리
    public float duration = 0.5f;          // 애니메이션 시간

    private Vector3 originalPos;           // OptionManager 원래 위치

    void Awake()
    {
        originalPos = optionManager.transform.localPosition; // 시작 위치 저장
        optionManager.SetActive(false);                      // 비활성화 상태로 시작
    }

    // 옵션 버튼에 연결
    public void ShowOptionManager()
    {
        optionManager.SetActive(true);

        // 기본값: InGameOptionPanel만 활성화
        inGameOptionPanel.SetActive(true);
        soundPanel.SetActive(false);

        // Y값 900만큼 위로 이동
        Vector3 targetPos = originalPos + new Vector3(0, moveDistance, 0);
        optionManager.transform.DOLocalMove(targetPos, duration).SetEase(Ease.OutCubic);
    }

    // OptionBackBtn 버튼에 연결
    public void HideOptionManager()
    {
        // Y값 900만큼 아래로 이동
        Vector3 targetPos = originalPos;
        optionManager.transform.DOLocalMove(targetPos, duration).SetEase(Ease.InCubic)
                     .OnComplete(() => optionManager.SetActive(false));
    }

    // SoundBtn 버튼에 연결
    public void ShowSoundPanel()
    {
        inGameOptionPanel.SetActive(false);
        soundPanel.SetActive(true);
    }

    // OptionBackBtn 버튼에 연결
    public void ShowInGameOptionPanel()
    {
        inGameOptionPanel.SetActive(true);
        soundPanel.SetActive(false);
    }

    // MainMenu 씬으로 이동
    public void GoMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // 씬 이름 정확히 입력
    }
}