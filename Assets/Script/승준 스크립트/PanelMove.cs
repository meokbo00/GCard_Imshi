using UnityEngine;
using UnityEngine.UI;

public class PanelSwitcher : MonoBehaviour
{
    public GameObject newPanel;  // ���� ���� �� �г�

    // ��ư Ŭ�� �� ȣ���� �Լ�
    public void OnButtonClicked()
    {
        // �� ��ư�� �θ� �г�(�θ� ������Ʈ) ã��
        GameObject parentPanel = transform.parent.gameObject;

        // �θ� �г� �ݱ� (��Ȱ��ȭ)
        parentPanel.SetActive(false);

        // �� �г� ���� (Ȱ��ȭ)
        if (newPanel != null)
            newPanel.SetActive(true);
    }
}
