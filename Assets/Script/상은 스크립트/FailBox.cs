using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FailBox : MonoBehaviour
{
    public void OnGotoMainMenuBtnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void RetryBtnClick()
    {
        SceneManager.LoadScene("InGame");
    }

}
