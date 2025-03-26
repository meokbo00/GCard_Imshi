using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DeckManager deckManager;

    public void OnDrawCardButtonClick()
    {
        if (deckManager.GetDeckCount() > 0)
        {
            Card drawnCard = deckManager.DrawCard();
            if (drawnCard != null)
            {
                // 카드를 화면에 표시
                drawnCard.gameObject.SetActive(true);
                drawnCard.transform.position = new Vector3(0, 0, 0);
            }
        }
    }

    public void OnSuitButtonClick()
    {
        deckManager.Suit();
    }

    public void OnRankButtonClick()
    {
        deckManager.Rank();
    }

    public void OnTrashButtonClick()
    {
        deckManager.TrashMove();
    }

    public void OnHandPlayButtonClick()
    {
        deckManager.HandPlay();
    }
}