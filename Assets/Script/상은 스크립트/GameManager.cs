using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public DeckManager deckManager;
    public GameSaveData gameSaveData;
    public SaveManager saveManager;
    public SoundManager2 soundManager2;
    public JokerChipStack jokerChipStack;

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
    public int sortgear = 0;

    // 현민 추가, 원래 제 폴더에 만들려 했는데 데이터 참조가 잘 안되는 듯 해서 여기다가 추가합니다.
    public Button R1Btn;  // 1라운드 : 점수
    public Button R2Btn;  // 2라운드 : 점수
    public Button R3Btn;  // 3라운드 : 점수

    public TextMeshProUGUI stageNameText;  // 현재 스테이지(행성) 이름
    public TextMeshProUGUI goalPointText1;  // 현재 스테이지의 1라운드 목표 점수
    public TextMeshProUGUI goalPointText2;  // 현재 스테이지의 2라운드 목표 점수
    public TextMeshProUGUI goalPointText3;  // 현재 스테이지의 3라운드 목표 점수

    private void Start()
    {
        gameSaveData = FindAnyObjectByType<GameSaveData>();
        saveManager = FindAnyObjectByType<SaveManager>();
        soundManager2 = FindAnyObjectByType<SoundManager2>();
        jokerChipStack = FindAnyObjectByType<JokerChipStack>();

        // 데이터 리셋은 여기서!!!!
        //ResetData();

        var itemData = FindObjectOfType<ItemData>();
        if (itemData != null)
        {
            itemData.LoadAndPlaceJokerItems();
        }

        playerData = saveManager.Load();
        currentTrashCount = playerData.trashcount;
        currentHandCount = playerData.handcount;
        UpdateUI();

        // 현민 추가, 원래 제 폴더에 만들려 했는데 데이터 참조가 잘 안되는 듯 해서 여기다가 추가합니다.
        UpdateStageInfo();

        if(playerData.round % 3 == 1)
        {
            SetButtonColor(R1Btn, true);
            SetButtonColor(R2Btn, false);
            SetButtonColor(R3Btn, false);
        }
        else if(playerData.round % 3 == 2)
        {
            SetButtonColor(R1Btn, false);
            SetButtonColor(R2Btn, true);
            SetButtonColor(R3Btn, false);
        }
        else if(playerData.round % 3 == 0)
        {
            SetButtonColor(R1Btn, false);
            SetButtonColor(R2Btn, false);
            SetButtonColor(R3Btn, true);
        }
    }

    public void ResetData()
    {
        // 기존 플레이어 데이터 초기화
        playerData = new PlayerData();
        playerData.money = 1000;
        playerData.handcount = 4;
        playerData.trashcount = 4;
        playerData.round = 1;
        playerData.ante = 1;
        playerData.bestscore = 0;
        playerData.moneyLimit = 5;
        saveManager.Save(playerData);

        // 조커 아이템 데이터 초기화
        ResetJokerItems();
        JokerChipStack.Instance.ResetAllChips();
    }

    private void ResetJokerItems()
    {
        try
        {
            string savePath = "./Saves/JokerZone";
            string filePath = Path.Combine(savePath, "JokerZoneData.json");
            
            // 파일이 존재하면 삭제
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"[GameManager] 조커 아이템 데이터가 초기화되었습니다: {filePath}");
            }
            
            // 현재 씬의 조커 아이템 오브젝트도 제거
            var jokerZone = GameObject.FindGameObjectWithTag("JokerZone");
            if (jokerZone != null)
            {
                // JokerZone의 모든 자식 오브젝트 제거
                foreach (Transform child in jokerZone.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            
            // ItemData 컴포넌트의 데이터도 초기화
            var itemData = FindObjectOfType<ItemData>();
            if (itemData != null)
            {
                // 리플렉션을 사용하여 private 필드 초기화
                var field = typeof(ItemData).GetField("buyJokersData", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(itemData, new BuyJokersData());
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[GameManager] 조커 아이템 초기화 중 오류 발생: {e.Message}");
        }
    }

    public void OnSuitButtonClick()
    {
        deckManager.Suit();
        sortgear = 1;
    }

    public void OnRankButtonClick()
    {
        deckManager.Rank();
        sortgear = 2;
    }

    public void OnTrashButtonClick()
    {
        Debug.Log("버리기 메서드 실행");
        if (currentTrashCount <= 0 || deckManager.GetSelectedCards().Count == 0) return;
        currentTrashCount -= 1;
        UpdateUI();
        deckManager.TrashMove();
    }

    public void OnHandPlayButtonClick()
    {
        UseJokerSkill useJokerSkill = FindObjectOfType<UseJokerSkill>();
        useJokerSkill.SetJokerSlot();
        Debug.Log("핸드플레이 메서드 실행");
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
        soundManager2.PlayCashOutSound();
        playerData.money += price;
        saveManager.Save(playerData);
        UpdateUI();
    }

    public void Reroll(int RerollCost)
    {
        //gameSaveData.money -= RerollCost;
        soundManager2.PlayCoinSound();
        playerData.money -= RerollCost;
        JokerChipStack.Instance.AddChips(JokerChipStack.ChipType.RerollRed, 3);
        saveManager.Save(playerData);
        UpdateUI();
    }

    public void PlusMoneyBtn(int plusmoney)
    {
        ShopBoxUpAndDown shopBoxUpAndDown = FindObjectOfType<ShopBoxUpAndDown>();
        shopBoxUpAndDown.MoveShopBoxUp();
        shopBoxUpAndDown.MoneyPackZoneDown();
        playerData.money += plusmoney;
        saveManager.Save(playerData);
        UpdateUI();
    }

    public void MultiplyMoneyBtn(int multiplymoney)
    {
        ShopBoxUpAndDown shopBoxUpAndDown = FindObjectOfType<ShopBoxUpAndDown>();
        shopBoxUpAndDown.MoveShopBoxUp();
        shopBoxUpAndDown.MoneyPackZoneDown();
        playerData.money *= multiplymoney;
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

    public void NewBestScore(float newBestScore)
    {
        playerData.bestscore = newBestScore;
        saveManager.Save(playerData);
        Debug.Log("최고점수 갱신! : " + playerData.bestscore);
    }
    public void UpdateUI()
    {
        // UI 업데이트
        handCountText.text = currentHandCount.ToString();
        trashCountText.text = currentTrashCount.ToString();
        moneyText.text = "$" + playerData.money.ToString("N0");
        AnteText.text = playerData.ante + "/10";
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

    // 현민 추가, 원래 제 폴더에 만들려 했는데 데이터 참조가 잘 안되는 듯 해서 여기다가 추가합니다.
    public void UpdateStageInfo() // 현재 스테이지 행성 이름과 라운드 점수들을 표시합니다.
    {
        if(playerData.ante == 1)   
        {
            stageNameText.text = "수성";
            goalPointText1.text = $"1라운드 : {goalPoints[0].ToString("N0")}";
            goalPointText2.text = $"2라운드 : {goalPoints[1].ToString("N0")}";
            goalPointText3.text = $"3라운드 : {goalPoints[2].ToString("N0")}";
        }
        else if(playerData.ante == 2)
        {
            stageNameText.text = "금성";
            goalPointText1.text = $"1라운드 : {goalPoints[3].ToString("N0")}";
            goalPointText2.text = $"2라운드 : {goalPoints[4].ToString("N0")}";
            goalPointText3.text = $"3라운드 : {goalPoints[5].ToString("N0")}";
        }
        else if(playerData.ante == 3)
        {
            stageNameText.text = "지구";
            goalPointText1.text = $"1라운드 : {goalPoints[6].ToString("N0")}";
            goalPointText2.text = $"2라운드 : {goalPoints[7].ToString("N0")}";
            goalPointText3.text = $"3라운드 : {goalPoints[8].ToString("N0")}";
        }
        else if(playerData.ante == 4)
        {
            stageNameText.text = "달";
            goalPointText1.text = $"1라운드 : {goalPoints[9].ToString("N0")}";
            goalPointText2.text = $"2라운드 : {goalPoints[10].ToString("N0")}";
            goalPointText3.text = $"3라운드 : {goalPoints[11].ToString("N0")}";
        }
        else if(playerData.ante == 5)
        {
            stageNameText.text = "화성";
            goalPointText1.text = $"1라운드 : {goalPoints[12].ToString("N0")}";
            goalPointText2.text = $"2라운드 : {goalPoints[13].ToString("N0")}";
            goalPointText3.text = $"3라운드 : {goalPoints[14].ToString("N0")}";
        }
        else if(playerData.ante == 6)
        {
            stageNameText.text = "목성";
            goalPointText1.text = $"1라운드 : {goalPoints[15].ToString("N0")}";
            goalPointText2.text = $"2라운드 : {goalPoints[16].ToString("N0")}";
            goalPointText3.text = $"3라운드 : {goalPoints[17].ToString("N0")}";
        }
        else if(playerData.ante == 7)
        {
            stageNameText.text = "토성";
            goalPointText1.text = $"1라운드 : {goalPoints[18].ToString("N0")}";
            goalPointText2.text = $"2라운드 : {goalPoints[19].ToString("N0")}";
            goalPointText3.text = $"3라운드 : {goalPoints[20].ToString("N0")}";
        }
        else if(playerData.ante == 8)
        {
            stageNameText.text = "천왕성";
            goalPointText1.text = $"1라운드 : {goalPoints[21].ToString("N0")}";
            goalPointText2.text = $"2라운드 : {goalPoints[22].ToString("N0")}";
            goalPointText3.text = $"3라운드 : {goalPoints[23].ToString("N0")}";
        }
        else if(playerData.ante == 9)
        {
            stageNameText.text = "해왕성";
            goalPointText1.text = $"1라운드 : {goalPoints[24].ToString("N0")}";
            goalPointText2.text = $"2라운드 : {goalPoints[25].ToString("N0")}";
            goalPointText3.text = $"3라운드 : {goalPoints[26].ToString("N0")}";
        }
        else if(playerData.ante == 10)
        {
            stageNameText.text = "태양";
            goalPointText1.text = $"1라운드 : {goalPoints[27].ToString("N0")}";
            goalPointText2.text = $"2라운드 : {goalPoints[28].ToString("N0")}";
            goalPointText3.text = $"3라운드 : {goalPoints[29].ToString("N0")}";
        }
    }    

    private void SetButtonColor(Button button, bool isInteractable) // 현재 라운드만 강조 (출처 : 상은 스크립트/Stage/Btn.cs)
    {
        button.interactable = isInteractable;
        var colors = button.colors;
        colors.normalColor = isInteractable ? Color.white : new Color(0.2745f, 0.2745f, 0.2745f); // #FFFFFF or #464646
        colors.highlightedColor = colors.normalColor;
        colors.pressedColor = colors.normalColor;
        colors.selectedColor = colors.normalColor;
        colors.disabledColor = colors.normalColor;
        button.colors = colors;
    }
}