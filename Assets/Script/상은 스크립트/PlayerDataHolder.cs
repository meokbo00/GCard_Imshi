using UnityEngine;
using System;

[Serializable]
public class PlayerData
{
    public int handcount;
    public int trashcount;
    public int money;
    public int ante;
    public int round;
}

public class PlayerDataHolder : MonoBehaviour
{
    public PlayerData playerData = new PlayerData(); // 인스펙터에서 보기 가능 (Serialize됨)

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않음
    }
}
