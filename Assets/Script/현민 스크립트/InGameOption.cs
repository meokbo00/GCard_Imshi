using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class OptionManagerController : MonoBehaviour
{
    public GameObject optionManager;       // OptionManager 오브젝트
    public GameObject inGameOptionPanel;   // OptionManager 하위 InGameOptionPanel
    public GameObject soundPanel;          // OptionManager 하위 SoundPanel
    public GameObject howToPlayPanel;      // OptionManager 하위 HowToPlayPanel
    public float moveDistance = 900f;      // 이동 거리
    public float duration = 1f;          // 애니메이션 시간

    private Vector3 originalPos;           // OptionManager 원래 위치

    // HowToPlay 내 도움말 매니저들
    public GameObject StageHelpManager;
    public GameObject HandPlayHelpManager;
    public GameObject TrashHelpManager;
    public GameObject HandArrayHelpManager;
    public GameObject JokerHelpManager;
    public GameObject RunInfoHelpManager;
    public GameObject OptionHelpManager;
    public GameObject ClearHelpManager;
    public GameObject GameOverHelpManager;
    public GameObject ShopHelpManager;

    // HowToPlay 내 커서(RectTransform)
    public RectTransform howToPlayCursor;
    private float howToPlayCursorDefaultY;
    private float originalAnchoredY;

    void Awake()
    {
        originalPos = optionManager.transform.localPosition; // 시작 위치 저장
        optionManager.SetActive(false);                      // 비활성화 상태로 시작
        var rtInit = optionManager.GetComponent<RectTransform>();
        originalAnchoredY = rtInit.anchoredPosition.y;       // anchoredPosition 기준 시작 Y 저장
        if (howToPlayCursor != null)
        {
            howToPlayCursorDefaultY = howToPlayCursor.anchoredPosition.y;
        }
    }

    // 옵션 버튼에 연결
    public void ShowOptionManager()
    {
        optionManager.SetActive(true);

        // 기본값: InGameOptionPanel만 활성화
        inGameOptionPanel.SetActive(true);
        soundPanel.SetActive(false);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);

        // anchoredPosition 기준: 초기 Y(예: 38)에서 900만큼 위로 올림
        var rt = optionManager.GetComponent<RectTransform>();
        rt.DOAnchorPosY(originalAnchoredY + 900f, 1f)
        .SetEase(Ease.OutBack)
        .OnComplete(() => optionManager.SetActive(true));

        Debug.Log("옵션 창 실행!");
    }

    // OptionBackBtn 버튼에 연결
    public void HideOptionManager()
    {
        // anchoredPosition 기준: 초기 Y로 복귀
        var rt = optionManager.GetComponent<RectTransform>();
        rt.DOAnchorPosY(originalAnchoredY, 1f)
        .SetEase(Ease.InBack)
        .OnComplete(() => optionManager.SetActive(false));

        Debug.Log("옵션 창 종료!");
    }

    // SoundBtn 버튼에 연결
    public void ShowSoundPanel()
    {
        inGameOptionPanel.SetActive(false);
        soundPanel.SetActive(true);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);

        Debug.Log("사운드 설정 창 실행!");
    }

    // OptionBackBtn 버튼에 연결
    public void ShowInGameOptionPanel()
    {
        inGameOptionPanel.SetActive(true);
        soundPanel.SetActive(false);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);

        Debug.Log("사운드 설정 창 종료! 옵션 창으로 이동");
    }

    // MainMenu 씬으로 이동
    public void GoMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // 씬 이름 정확히 입력

        Debug.Log("메인메뉴로 이동!");
    }

    // HowToPlayBtn 버튼에 연결: HowToPlayPanel 활성화, 나머지 비활성화
    public void ShowHowToPlayPanel()
    {
        inGameOptionPanel.SetActive(false);
        soundPanel.SetActive(false);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(true);

        // 기본값: StageHelpManager만 활성화
        SetHelpManagersActive(StageHelpManager);

        // 커서는 배치된 위치를 유지 (변경 없음)

        Debug.Log("HowToPlay 창 실행!");
    }

    // HowToPlay 내 BackBtn에 연결: HowToPlay 비활성화, InGameOptionPanel 활성화
    public void CloseHowToPlayToInGameOption()
    {
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
        inGameOptionPanel.SetActive(true);
        soundPanel.SetActive(false);

        Debug.Log("HowToPlay 종료 → 옵션 창으로 이동");
    }

    // 공통: 모든 HelpManager 비활성화 후 특정 하나만 활성화
    private void SetHelpManagersActive(GameObject active)
    {
        GameObject[] all = new GameObject[]
        {
            StageHelpManager,
            HandPlayHelpManager,
            TrashHelpManager,
            HandArrayHelpManager,
            JokerHelpManager,
            RunInfoHelpManager,
            OptionHelpManager,
            ClearHelpManager,
            GameOverHelpManager,
            ShopHelpManager
        };

        foreach (var go in all)
        {
            if (go != null) go.SetActive(false);
        }

        if (active != null) active.SetActive(true);
    }

    // 각 버튼에 연결: 해당 매니저만 활성화하고 커서 y 위치 설정
    public void ShowStageHelp()
    {
        SetHelpManagersActive(StageHelpManager);
        // 커서를 원래 배치되어 있던 y 위치로 복귀
        SetCursorY(howToPlayCursorDefaultY);
    }

    public void ShowHandPlayHelp()
    {
        SetHelpManagersActive(HandPlayHelpManager);
        SetCursorY(-692f);
    }

    public void ShowTrashHelp()
    {
        SetHelpManagersActive(TrashHelpManager);
        SetCursorY(-772f);
    }

    public void ShowHandArrayHelp()
    {
        SetHelpManagersActive(HandArrayHelpManager);
        SetCursorY(-852f);
    }

    public void ShowJokerHelp()
    {
        SetHelpManagersActive(JokerHelpManager);
        SetCursorY(-932f);
    }

    public void ShowRunInfoHelp()
    {
        SetHelpManagersActive(RunInfoHelpManager);
        SetCursorY(-1012f);
    }

    public void ShowOptionHelp()
    {
        SetHelpManagersActive(OptionHelpManager);
        SetCursorY(-1092f);
    }

    public void ShowClearHelp()
    {
        SetHelpManagersActive(ClearHelpManager);
        SetCursorY(-1172f);
    }

    public void ShowGameOverHelp()
    {
        SetHelpManagersActive(GameOverHelpManager);
        SetCursorY(-1252f);
    }

    public void ShowShopHelp()
    {
        SetHelpManagersActive(ShopHelpManager);
        SetCursorY(-1332f);
    }

    // 커서 y만 변경 (x는 유지)
    private void SetCursorY(float y)
    {
        if (howToPlayCursor == null) return;
        var pos = howToPlayCursor.anchoredPosition;
        howToPlayCursor.anchoredPosition = new Vector2(pos.x, y);
    }
}