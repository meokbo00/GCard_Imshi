using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using System.Collections.Generic;

public class JsonTest : MonoBehaviour
{
    [Header("TextEffect 참조")]
    public TextEffect textEffect;  // TextEffect 컴포넌트 참조

    void Start()
    {
        // JSON 파일 로드
        TextAsset jsonFile = Resources.Load<TextAsset>("json/행성 설명 정리");
        
        if (jsonFile != null)
        {
            try
            {
                // Newtonsoft.Json을 사용하여 JSON 배열을 직접 파싱
                planetList = JsonConvert.DeserializeObject<List<PlanetInfo>>(jsonFile.text);
                
                if (planetList != null && planetList.Count > 0)
                {
                    // TextEffect를 통해 타이핑 효과로 텍스트 표시 (기본 동작 유지)
                    if (textEffect != null)
                    {
                        textEffect.StartTypingEffect(planetList[currentPlanetIndex].name, planetList[currentPlanetIndex].explain);
                    }
                    else
                    {
                        Debug.LogError("TextEffect 컴포넌트가 할당되지 않았습니다.");
                    }
                }
                else
                {
                    Debug.LogError("행성 데이터를 찾을 수 없습니다.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"JSON 파싱 오류: {e.Message}");
            }
        }
    }

    // 행성 정보를 담는 클래스
    private List<PlanetInfo> planetList;
    private int currentPlanetIndex = 5; // 기본값으로 5번 인덱스 사용 (기존과 동일하게 유지)

    // 현재 행성의 이름을 가져오는 메서드
    public string GetCurrentPlanetName()
    {
        if (planetList != null && planetList.Count > currentPlanetIndex)
        {
            return planetList[currentPlanetIndex].name;
        }
        return string.Empty;
    }

    // 현재 행성의 설명을 가져오는 메서드
    public string GetCurrentPlanetExplain()
    {
        if (planetList != null && planetList.Count > currentPlanetIndex)
        {
            return planetList[currentPlanetIndex].explain;
        }
        return string.Empty;
    }

    [System.Serializable]
    public class PlanetInfo
    {
        public int id;
        public string name;
        public string explain;
    }
}