using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    private string savePath;

    private void Awake()
    {
        // 저장 파일 경로 (플랫폼에 맞게 자동으로 지정됨)
        savePath = Path.Combine(Application.persistentDataPath, "save.json");
    }

    // 저장
    public void Save(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true); // true = 보기 좋은 형식
        File.WriteAllText(savePath, json);
        Debug.Log("저장 완료: " + savePath);
    }

    // 불러오기
    public PlayerData Load()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("저장 파일이 없음. 기본값 반환.");
            return new PlayerData(); // 기본값
        }

        string json = File.ReadAllText(savePath);
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);
        Debug.Log("불러오기 완료!");
        return data;
    }
}