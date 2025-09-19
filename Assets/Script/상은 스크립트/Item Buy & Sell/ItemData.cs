using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;


[System.Serializable]
public class JokerData
{
    public string objectName;
    public int slot; // JokerZone 내에서의 슬롯 위치 (1부터 시작)
}

[System.Serializable]
public class BuyJokersData
{
    public List<JokerData> clickedObjects = new List<JokerData>();
}

public class ItemData : MonoBehaviour
{
    private BuyJokersData buyJokersData = new BuyJokersData();
    private string savePath = "./Saves/JokerZone";
    private const string PREFAB_PATH_PREFIX = "Prefabs/ShopItem/Joker/";
    
    private void Awake()
    {
        // 저장 디렉토리가 없으면 생성
        try
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
                Debug.Log($"[ItemData] 저장 디렉토리를 생성했습니다: {Path.GetFullPath(savePath)}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ItemData] 저장 디렉토리 생성 실패: {e.Message}");
        }
    }

    // 판매된 아이템을 저장된 데이터에서 제거
    public void RemoveItemFromSavedData(GameObject soldItem)
    {
        // 판매된 아이템의 이름 가져오기 (Clone 제거)
        string itemName = soldItem.name;
        if (itemName.Contains("(Clone)"))
        {
            itemName = itemName.Replace("(Clone)", "").Trim();
        }
        
        // 저장된 데이터에서 해당 아이템 찾기 (이름이 같은 첫 번째 아이템 제거)
        var itemToRemove = buyJokersData.clickedObjects.FirstOrDefault(item => item.objectName == itemName);
        if (itemToRemove != null)
        {
            buyJokersData.clickedObjects.Remove(itemToRemove);
            
            // 변경된 데이터 저장
            string jsonData = JsonUtility.ToJson(buyJokersData, true);
            string filePath = Path.Combine(savePath, "JokerZoneData.json");
            File.WriteAllText(filePath, jsonData);
            
            Debug.Log($"아이템이 저장 데이터에서 제거되었습니다: {itemName}");
        }
    }
    
    public void SaveObjectData(string objectName)
    {
        // JokerZone 찾기
        GameObject jokerZone = GameObject.FindGameObjectWithTag("JokerZone");
        if (jokerZone == null)
        {
            Debug.LogError("JokerZone을 찾을 수 없습니다!");
            return;
        }

        // 기존 데이터 초기화
        buyJokersData.clickedObjects.Clear();

        // JokerZone의 자식들을 순회하며 저장
        int index = 0;
        foreach (Transform child in jokerZone.transform)
        {
            if (child.CompareTag("BuyJoker"))
            {
                // 프리팹 이름에서 (Clone) 제거
                string prefabName = child.name;
                if (prefabName.Contains("(Clone)"))
                {
                    prefabName = prefabName.Replace("(Clone)", "").Trim();
                }

                buyJokersData.clickedObjects.Add(new JokerData
                {
                    objectName = prefabName,
                    slot = index++
                });
            }
        }

        // JSON으로 변환하여 저장
        string jsonData = JsonUtility.ToJson(buyJokersData, true);
        // savePath가 null이면 기본값 설정
        if (string.IsNullOrEmpty(savePath))
        {
            savePath = "./Saves/JokerZone";
            Debug.LogWarning($"[ItemData] 저장 경로가 비어있어 기본값으로 설정합니다: {savePath}");
        }
        
        string filePath = Path.Combine(savePath, "JokerZoneData.json");
        Debug.Log($"[ItemData] 데이터 파일 경로: {filePath}");
        File.WriteAllText(filePath, jsonData);
        
        Debug.Log("JokerZone 데이터 저장 완료");
    }

public void LoadAndPlaceJokerItems()
{
    // savePath가 null이면 기본값 설정
    if (string.IsNullOrEmpty(savePath))
    {
        savePath = "./Saves/JokerZone";
        Debug.LogWarning($"[ItemData] 저장 경로가 비어있어 기본값으로 설정합니다: {savePath}");
    }
    
    string filePath = Path.Combine(savePath, "JokerZoneData.json");
    Debug.Log($"[ItemData] 데이터 파일 경로: {filePath}");
    if (!File.Exists(filePath))
    {
        Debug.Log("저장된 JokerZone 데이터가 없습니다.");
        return;
    }

    // 저장된 데이터 로드
    string jsonData = File.ReadAllText(filePath);
    buyJokersData = JsonUtility.FromJson<BuyJokersData>(jsonData);

    // JokerZone 찾기
    GameObject jokerZone = GameObject.FindGameObjectWithTag("JokerZone");
    if (jokerZone == null)
    {
        Debug.LogError("JokerZone을 찾을 수 없습니다!");
        return;
    }

    // JokerZone의 BoxCollider 가져오기
    BoxCollider2D boxCollider = jokerZone.GetComponent<BoxCollider2D>();
    if (boxCollider == null)
    {
        Debug.LogError("JokerZone에 BoxCollider가 없습니다!");
        return;
    }

    // JokerZone의 기존 자식들 중 BuyJoker 태그가 있는 아이템들 제거
    List<GameObject> childrenToDestroy = new List<GameObject>();
    foreach (Transform child in jokerZone.transform)
    {
        if (child.CompareTag("BuyJoker"))
        {
            childrenToDestroy.Add(child.gameObject);
        }
    }
    foreach (GameObject child in childrenToDestroy)
    {
        DestroyImmediate(child);
    }

    // 저장된 아이템들을 순서대로 생성
    for (int i = 0; i < buyJokersData.clickedObjects.Count; i++)
    {
        var jokerData = buyJokersData.clickedObjects[i];
        
        // 프리팹 이름에서 (Clone) 제거
        string prefabName = jokerData.objectName;
        if (prefabName.Contains("(Clone)"))
        {
            prefabName = prefabName.Replace("(Clone)", "").Trim();
        }

        // 프리팹 로드
        string prefabPath = $"{PREFAB_PATH_PREFIX}{prefabName}";
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            Debug.LogError($"프리팹을 찾을 수 없습니다: {prefabPath}");
            continue;
        }

        // JokerZone의 BoxCollider 경계 가져오기
        Bounds bounds = boxCollider.bounds;
        float zoneWidth = bounds.size.x * 0.8f; // 80% 너비 사용 (여유 공간 확보)
        float startX = bounds.center.x - (zoneWidth / 2);
        
        // 아이템의 너비 (고정값 사용, 실제 아이템의 크기에 따라 조정 필요)
        float itemWidth = 1.0f; // 아이템의 대략적인 너비
        
        // 아이템 개수에 따른 간격 계산
        int itemCount = buyJokersData.clickedObjects.Count;
        float totalWidth = (itemCount - 1) * itemWidth;
        
        // 중앙 정렬을 위한 시작 위치 조정
        float startPos = bounds.center.x - (totalWidth / 2);
        
        // 생성 위치 설정 (BoxCollider2D 내에서 왼쪽에서 오른쪽으로 정렬)
        float xPos = startPos + (i * itemWidth);
        
        // JokerZone의 y 위치를 유지하면서 x 위치만 조정
        Vector3 spawnPosition = new Vector3(
            Mathf.Clamp(xPos, bounds.min.x + itemWidth/2, bounds.max.x - itemWidth/2),
            bounds.center.y,
            jokerZone.transform.position.z
        );
        
        // EdgeCollider2D가 있는 경우를 대비해 z 위치 조정
        spawnPosition.z = -0.1f * (i + 1); // 겹치지 않도록 약간씩 앞으로

        // JokerZone의 자식으로 프리팹 인스턴스 생성
        GameObject item = Instantiate(prefab, spawnPosition, Quaternion.identity, jokerZone.transform);
        item.transform.localScale = new Vector3(1.11f, 1.11f, 1f); // 스케일 설정
        item.name = prefabName;
        
        // PriceTag라는 이름을 가진 모든 자식 오브젝트 비활성화
        Transform[] allChildren = item.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (child.name == "PriceTag")
            {
                child.gameObject.SetActive(false);
            }
        }

        // DragItem 컴포넌트 초기화
        var dragItem = item.GetComponent<DragItem>();
        if (dragItem != null)
        {
            dragItem.isPurchased = true;
            dragItem.gameObject.tag = "BuyJoker";
            
            // SpriteRenderer의 sortingOrder 설정 (DragItem과 동일하게)
            var spriteRenderer = item.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = -(i + 1); // -1, -2, -3... 순서로 정렬
            }
        }
        
        Debug.Log($"아이템 생성 완료: {prefabName} -> 위치: {spawnPosition}");
    }

    // 자식들의 위치를 자동으로 정렬
    SortJokerZoneItems(jokerZone);
}

// JokerZone의 아이템들을 x 좌표 기준으로 정렬
private void SortJokerZoneItems(GameObject jokerZone)
{
    if (jokerZone == null) return;

    // JokerZone의 자식들 중 BuyJoker 태그가 있는 아이템만 가져오기
    List<Transform> items = new List<Transform>();
    foreach (Transform child in jokerZone.transform)
    {
        if (child.CompareTag("BuyJoker"))
        {
            items.Add(child);
        }
    }

    // x 좌표 기준으로 정렬
    items.Sort((a, b) => a.position.x.CompareTo(b.position.x));

    // 정렬된 순서대로 위치 조정
    for (int i = 0; i < items.Count; i++)
    {
        items[i].SetSiblingIndex(i);
    }
}
}