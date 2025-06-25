using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public DeckManager deckManager;
    public int handcount;
    public int trashcount;
    public int money;
    public int ante;
    public int round;
    public int GoalPoint;
    public float gsumPoint;


    public TextMeshProUGUI handCountText;
    public TextMeshProUGUI trashCountText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI AnteText;
    public TextMeshProUGUI RoundText;
    public TextMeshProUGUI GoalPointText;

    private int[] goalPoints = { 10, 450, 600, 800, 1200, 1600, 2000, 3000, 4000, 5000, 7500, 10000, 11000, 16500, 22000, 20000, 27500, 35000, 35000, 52500, 70000, 50000, 75000, 100000, 110000, 165000, 220000, 560000, 840000, 2240000, 7200000, 10800000, 14400000, 300000000, 450000000, 600000000 };

    private void Start()
    {
        UpdateGoalPoint();
        UpdateUI();
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
        if (trashcount <= 0 || deckManager.GetSelectedCards().Count == 0) return;
        trashcount -= 1;
        UpdateUI();
        deckManager.TrashMove();
    }

    public void OnHandPlayButtonClick()
    {
        if (handcount <= 0 || deckManager.GetSelectedCards().Count == 0) return;
        handcount -= 1;
        UpdateUI();
        deckManager.HandPlay();
    }

    public void BuyItem(int price)
    {
        money -= price;
        UpdateUI();
    }

    public void SellItem(int price)
    {
        money += price;
        UpdateUI();
    }

    public void Reroll()
    {
        money -= 5;
        UpdateUI();
    }
    public void UpdateUI()
    {
        handCountText.text = handcount.ToString();
        trashCountText.text = trashcount.ToString();
        moneyText.text = "$" + money.ToString("N0");
        AnteText.text = ante + "/8";
        RoundText.text = round.ToString();
        GoalPointText.text = GoalPoint.ToString("N0");
    }

    private void UpdateGoalPoint()
    {
        if (round >= 1 && round <= goalPoints.Length)
        {
            GoalPoint = goalPoints[round - 1];
        }
        else
        {
            GoalPoint = 0;
        }
    }
}