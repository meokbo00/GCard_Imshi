using UnityEngine;
using UnityEngine.UI;

public class PanelSwitcher : MonoBehaviour
{
    public GameObject newPanel;  // 열고 싶은 새 패널

    // 버튼 클릭 시 호출할 함수
    public void OnButtonClicked()
    {
        // 이 버튼의 부모 패널(부모 오브젝트) 찾기
        GameObject parentPanel = transform.parent.gameObject;

        // 부모 패널 닫기 (비활성화)
        parentPanel.SetActive(false);

        // 새 패널 열기 (활성화)
        if (newPanel != null)
            newPanel.SetActive(true);
    }
}
