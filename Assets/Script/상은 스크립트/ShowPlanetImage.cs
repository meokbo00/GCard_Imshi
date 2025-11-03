using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPlanetImage : MonoBehaviour
{
    PlayerData playerData;
    public GameObject[] planetImage;
    SaveManager saveManager;

    void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        playerData = saveManager.Load();
        int i = playerData.ante;

        if(i <= planetImage.Length)
        {
            planetImage[i-1].SetActive(true);
        }
    }
}
