using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSaveData : MonoBehaviour
{
    public int handcount;
    public int trashcount;
    public int money;
    public int ante;
    public int round;
    public int GoalPoint;
    public float gsumPoint;



    public static GameSaveData Instance { get; private set; }

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 이미 인스턴스가 존재하면 새로 생성된 인스턴스 파괴
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //ResetData();
    }

    public void ResetData()
    {
        // 기본값 설정
        handcount = 4;
        trashcount = 4;
        money = 200;
        ante = 1;
        round = 1;
        
        // PlayerPrefs에 저장
        PlayerPrefs.SetInt("HandCount", handcount);
        PlayerPrefs.SetInt("TrashCount", trashcount);
        PlayerPrefs.SetInt("Money", money);
        PlayerPrefs.SetInt("Ante", ante);
        PlayerPrefs.SetInt("Round", round);
        PlayerPrefs.Save();
    }
}
