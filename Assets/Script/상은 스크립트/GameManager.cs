using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public DeckManager deckManager;
    public GameSaveData gameSaveData;
    public SaveManager saveManager;

    public PlayerData playerData;

    public TextMeshProUGUI handCountText;
    public TextMeshProUGUI trashCountText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI AnteText;
    public TextMeshProUGUI RoundText;
    public TextMeshProUGUI GoalPointText;
    public int[] goalPoints = { 10, 20, 30, 800, 1200, 1600, 2000, 3000, 4000, 5000, 7500, 10000, 11000, 16500, 22000, 20000, 27500, 35000, 35000, 52500, 70000, 50000, 75000, 100000, 110000, 165000, 220000, 560000, 840000, 2240000, 7200000, 10800000, 14400000, 300000000, 450000000, 600000000 };

    public int currentTrashCount;
    public int currentHandCount;
    public float gsumPoint;


    private void Start()
    {

        Debug.Log("goalPoint[0] : " + goalPoints[0]);
        Debug.Log("goalPoint[1] : " + goalPoints[1]);
        Debug.Log("goalPoint[2] : " + goalPoints[2]);

        gameSaveData = FindAnyObjectByType<GameSaveData>();
        saveManager = FindAnyObjectByType<SaveManager>();

        // 데이터 리셋은 여기서!!!!
        //ResetData();


        playerData = saveManager.Load();
        currentTrashCount = playerData.trashcount;
        currentHandCount = playerData.handcount;
        UpdateUI();
    }

    public void ResetData()
    {
        playerData = new PlayerData();
        playerData.money = 200;
        playerData.handcount = 4;
        playerData.trashcount = 4;
        playerData.round = 1;
        playerData.ante = 1;
        saveManager.Save(playerData);
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
        if (currentTrashCount <= 0 || deckManager.GetSelectedCards().Count == 0) return;
        currentTrashCount -= 1;
        UpdateUI();
        deckManager.TrashMove();
    }

    public void OnHandPlayButtonClick()
    {
        if (currentHandCount <= 0 || deckManager.GetSelectedCards().Count == 0) return;
        currentHandCount -= 1;
        UpdateUI();
        deckManager.HandPlay();
    }

    public void BuyItem(int price)
    {
        //gameSaveData.money -= price;
        playerData.money -= price;
        saveManager.Save(playerData);
        UpdateUI();
    }

    public void SellItem(int price)
    {
        //gameSaveData.money += price;
        playerData.money += price;
        saveManager.Save(playerData);
        UpdateUI();
    }

    public void Reroll(int RerollCost)
    {
        //gameSaveData.money -= RerollCost;
        playerData.money -= RerollCost;
        saveManager.Save(playerData);
        UpdateUI();
    }

    public void PlusRound()
    {
        playerData.round += 1;
        if(playerData.round % 3 == 1)
        {
            playerData.ante += 1;
        }
        saveManager.Save(playerData);
        UpdateUI();
    }
    public void UpdateUI()
    {
        // UI 업데이트
        handCountText.text = currentHandCount.ToString();
        trashCountText.text = currentTrashCount.ToString();
        moneyText.text = "$" + playerData.money.ToString("N0");
        AnteText.text = playerData.ante + "/8";
        RoundText.text = playerData.round.ToString();
        GoalPointText.text = goalPoints[playerData.round - 1].ToString("N0");

        // 게임 데이터 저장
        //PlayerPrefs.SetInt("HandCount", gameSaveData.handcount);
        //PlayerPrefs.SetInt("TrashCount", gameSaveData.trashcount);
        //PlayerPrefs.SetInt("Money", gameSaveData.money);
        //PlayerPrefs.SetInt("Ante", gameSaveData.ante);
        //PlayerPrefs.SetInt("Round", gameSaveData.round);
        //PlayerPrefs.SetInt("GoalPoint", gameSaveData.GoalPoint);
        //PlayerPrefs.SetFloat("GSumPoint", gameSaveData.gsumPoint);
        //PlayerPrefs.Save(); // 변경사항을 디스크에 저장
    }
}