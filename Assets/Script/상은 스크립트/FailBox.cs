using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FailBox : MonoBehaviour
{
    GameManager gameManager;
    public TextMeshProUGUI FailText;
    PlayerData playerData;
    SaveManager saveManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        saveManager = FindObjectOfType<SaveManager>();
        playerData = saveManager.Load();
        
        // 모든 텍스트를 한 번에 할당
        FailText.text = $"앤티: {playerData.ante}\n라운드: {playerData.round}\n최고 기록: {playerData.bestscore}";
    }
    public void OnGotoMainMenuBtnClick()
    {
        gameManager.ResetData();
        SceneManager.LoadScene("MainMenu");
    }

    public void RetryBtnClick()
    {
        gameManager.ResetData();
        SceneManager.LoadScene("InGame");
    }

}
