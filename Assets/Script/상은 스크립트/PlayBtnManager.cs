using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBtnManager : MonoBehaviour
{
    public DeckManager deckManager;
    public GameManager gameManager;
    public int handcount;
    public int trashcount;

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
        if (trashcount <= 0 || deckManager.GetSelectedCards().Count == 0) return;
        trashcount -= 1;
        gameManager.UpdateUI();
        deckManager.TrashMove();
    }

    public void OnHandPlayButtonClick()
    {
        if (handcount <= 0 || deckManager.GetSelectedCards().Count == 0) return;
        handcount -= 1;
        gameManager.UpdateUI();
        deckManager.HandPlay();
    }
}
