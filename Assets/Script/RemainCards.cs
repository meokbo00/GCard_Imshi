using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RemainCards : MonoBehaviour
{
    public TextMeshProUGUI remainText; // 남은 카드 수를 표시할 텍스트
    private DeckManager deckManager;
    private int totalCards = 52; // 전체 카드 수
    private int initialCards = 8; // 초기에 활성화되는 카드 수

    // Start is called before the first frame update
    void Start()
    {
        deckManager = FindObjectOfType<DeckManager>();
        UpdateRemainText();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRemainText();
    }

    private void UpdateRemainText()
    {
        // 현재 활성화된 카드 수 계산
        int activeCards = 0;
        foreach (Card card in deckManager.GetHand())
        {
            if (card.gameObject.activeSelf)
            {
                activeCards++;
            }
        }

        // 덱에 남은 카드 수 계산
        int deckCards = deckManager.GetDeckCount();

        // 남은 카드 수 계산 (덱에 남은 카드 수 + 활성화된 카드 수)
        int remainCards = deckCards + activeCards;
        remainText.text = $"({remainCards}/{totalCards})";
    }
}
