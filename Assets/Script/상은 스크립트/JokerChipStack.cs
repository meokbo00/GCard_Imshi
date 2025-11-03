using UnityEngine;

public class JokerChipStack : MonoBehaviour
{
    private const string REROLL_RED_CHIP_KEY = "RerollRedChip";
    private const string ITEM_SELL_BLUE_CHIP_KEY = "ItemSellBlueChip";
    private const string ITEM_BUY_RED_CHIP_KEY = "ItemBuyRedChip";

    public int RerollRedChip { get; private set; }
    public int ItemSellBlueChip { get; private set; }
    public int ItemBuyRedChip { get; private set; }

    public enum ChipType
    {
        RerollRed,
        SellBlue,
        BuyRed
    }

    private static JokerChipStack _instance;
    public static JokerChipStack Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<JokerChipStack>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("JokerChipStack");
                    _instance = obj.AddComponent<JokerChipStack>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllChips();            
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void AddChips(ChipType chipType, int amount)
    {
        switch (chipType)
        {
            case ChipType.RerollRed:
                RerollRedChip += amount;
                break;
            case ChipType.SellBlue:
                ItemSellBlueChip += amount;
                break;
            case ChipType.BuyRed:
                ItemBuyRedChip += amount;
                break;
        }
        SaveChips(chipType);
    }

    public int GetChips(ChipType chipType)
    {
        return chipType switch
        {
            ChipType.RerollRed => RerollRedChip,
            ChipType.SellBlue => ItemSellBlueChip,
            ChipType.BuyRed => ItemBuyRedChip,
            _ => 0
        };
    }

    private void SaveChips(ChipType chipType)
    {
        switch (chipType)
        {
            case ChipType.RerollRed:
                PlayerPrefs.SetInt(REROLL_RED_CHIP_KEY, RerollRedChip);
                break;
            case ChipType.SellBlue:
                PlayerPrefs.SetInt(ITEM_SELL_BLUE_CHIP_KEY, ItemSellBlueChip);
                break;
            case ChipType.BuyRed:
                PlayerPrefs.SetInt(ITEM_BUY_RED_CHIP_KEY, ItemBuyRedChip);
                break;
        }
        PlayerPrefs.Save();
        Debug.Log($"{chipType} 저장 완료: {GetChips(chipType)}");
    }

    private void LoadAllChips()
    {
        RerollRedChip = PlayerPrefs.GetInt(REROLL_RED_CHIP_KEY, 0);
        ItemSellBlueChip = PlayerPrefs.GetInt(ITEM_SELL_BLUE_CHIP_KEY, 0);
        ItemBuyRedChip = PlayerPrefs.GetInt(ITEM_BUY_RED_CHIP_KEY, 0);
        Debug.Log("모든 칩 값 불러오기 완료");
    }

    // 모든 칩 데이터 초기화 (필요시 사용)
    public void ResetAllChips()
    {
        RerollRedChip = 0;
        ItemSellBlueChip = 0;
        ItemBuyRedChip = 0;
        
        PlayerPrefs.DeleteKey(REROLL_RED_CHIP_KEY);
        PlayerPrefs.DeleteKey(ITEM_SELL_BLUE_CHIP_KEY);
        PlayerPrefs.DeleteKey(ITEM_BUY_RED_CHIP_KEY);
        PlayerPrefs.Save();

        Debug.Log(RerollRedChip);
    }
}