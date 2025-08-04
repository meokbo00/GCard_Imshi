using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Btn : MonoBehaviour
{
    public Button PlayBtn1;
    public Button PlayBtn2;
    public Button PlayBtn3;

    GameSaveData gameSaveData;

    void Start()
    {
        gameSaveData = FindAnyObjectByType<GameSaveData>();
        if(gameSaveData.round % 3 == 1)
        {
            SetButtonColor(PlayBtn1, true);
            SetButtonColor(PlayBtn2, false);
            SetButtonColor(PlayBtn3, false);
        }
        else if(gameSaveData.round % 3 == 2)
        {
            SetButtonColor(PlayBtn1, false);
            SetButtonColor(PlayBtn2, true);
            SetButtonColor(PlayBtn3, false);
        }
        else if(gameSaveData.round % 3 == 0)
        {
            SetButtonColor(PlayBtn1, false);
            SetButtonColor(PlayBtn2, false);
            SetButtonColor(PlayBtn3, true);
        }
    }

    private void SetButtonColor(Button button, bool isInteractable)
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

    public void OnClickButton()
    {
        SceneManager.LoadScene("InGame");
    }
}