using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class GenarateItem : MonoBehaviour
{
    [Header("슬롯 참조")]
    public GameObject ZokerSlot1;
    public GameObject ZokerSlot2;
    public GameObject ItemPackSlot1;
    public GameObject ItemPackSlot2;
    public GameObject VoucherSlot1;

    [Header("아이템팩 전용 슬롯")]
    public GameObject SelectSlot1;
    public GameObject SelectSlot2;
    public GameObject SelectSlot3;
    public GameObject SelectSlot4;
    public GameObject SkipBtn;

    [Header("머니팩 전용 슬롯")]
    public GameObject MoneyPackSlot1;
    public GameObject MoneyPackSlot2;
    public GameObject MoneyPackSlot3;
    public GameObject MoneyPackSlot4;

    [Header("프리팹 배열")]
    public GameObject[] ZokerPrefabs;
    public GameObject[] ItemPackPrefabs;
    public GameObject[] VoucherPrefabs;
    public GameObject[] MoneyPrefabs;
    public GameObject[] ItemPackJoker;
    public GameObject[] ItemPackMoney;
    
    // 더 이상 사용하지 않는 프리팹 배열들
    private GameObject[] TaroPrefabs = new GameObject[0];
    private GameObject[] PlanetPrefabs = new GameObject[0];

    private List<GameObject> instantiatedPrefabs = new List<GameObject>();
    private List<GameObject> usedJokerPrefabs = new List<GameObject>();

    GameManager gameManager;
    public TextMeshProUGUI RerollCostText;
    public int RerollCost = 5;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    // 타입에 따라 랜덤한 프리팹을 지정된 개수의 슬롯에 생성하는 메서드
    // type: 1=ZokerPrefabs, 2=MoneyPrefabs
    // slotCount: 사용할 슬롯의 개수 (1~4)
    // itemPackToRemove: 제거할 특정 ItemPack 프리팹 (선택사항)
    // isMoneyPack: 머니팩 여부 (true면 MoneyPack 전용 슬롯 사용)
    public void GenerateRandomPrefabs(int type, int slotCount, GameObject itemPackToRemove = null, bool isMoneyPack = false)
    {
        // 특정 ItemPack 프리팹이 지정된 경우 해당 프리팹만 제거
        if (itemPackToRemove != null)
        {
            if (instantiatedPrefabs.Contains(itemPackToRemove))
            {
                instantiatedPrefabs.Remove(itemPackToRemove);
                if (itemPackToRemove != null)
                    Destroy(itemPackToRemove);
            }
        }
        else
        {
            // 기존 로직: 모든 프리팹 제거
            foreach (var prefab in instantiatedPrefabs)
            {
                if (prefab != null)
                    Destroy(prefab);
            }
            instantiatedPrefabs.Clear();
        }

        // 타입과 isMoneyPack에 따라 사용할 프리팹 배열 선택
        GameObject[] prefabsToUse;
        if (isMoneyPack)
        {
            prefabsToUse = type == 1 ? ItemPackMoney : MoneyPrefabs;
        }
        else
        {
            prefabsToUse = type == 1 ? ItemPackJoker : MoneyPrefabs;
        }
        if (prefabsToUse == null || prefabsToUse.Length == 0)
        {
            Debug.LogError("No prefabs available for the specified type: " + type);
            return;
        }

        // 슬롯 개수 유효성 검사
        slotCount = Mathf.Clamp(slotCount, 1, 4);
        
        // 사용할 슬롯 배열 생성
        GameObject[] slots;
        List<int> slotIndices = new List<int>();
        
        // MoneyPack인 경우 MoneyPack 전용 슬롯 사용
        if (isMoneyPack)
        {
            slots = new GameObject[4] { MoneyPackSlot1, MoneyPackSlot2, MoneyPackSlot3, MoneyPackSlot4 };
            // 랜덤하게 슬롯 선택 (중복 없이)
            slotIndices = new List<int> { 0, 1, 2, 3 };
            for (int i = 0; i < 4 - slotCount; i++)
            {
                int removeIndex = Random.Range(0, slotIndices.Count);
                slotIndices.RemoveAt(removeIndex);
            }
        }
        // slotCount가 2인 경우 SelectSlot2와 SelectSlot3에만 생성
        else if (slotCount == 2)
        {
            SkipBtn.SetActive(true);
            slots = new GameObject[2] { SelectSlot2, SelectSlot3 };
            slotIndices.Add(0); // SelectSlot2
            slotIndices.Add(1); // SelectSlot3
        }
        else
        {
            SkipBtn.SetActive(true);
            // 그 외의 경우 기존 로직 유지
            slots = new GameObject[4] { SelectSlot1, SelectSlot2, SelectSlot3, SelectSlot4 };
            // 랜덤하게 슬롯 선택 (중복 없이)
            slotIndices = new List<int> { 0, 1, 2, 3 };
            for (int i = 0; i < 4 - slotCount; i++)
            {
                int removeIndex = Random.Range(0, slotIndices.Count);
                slotIndices.RemoveAt(removeIndex);
            }
        }

        // 선택된 슬롯에 프리팹 생성
        foreach (int slotIndex in slotIndices)
        {
            if (slots[slotIndex] != null)
            {
                // 랜덤한 프리팹 선택 (MoneyPack의 경우 가중치 적용)
                GameObject randomPrefab;
                if (isMoneyPack && type == 1) // MoneyPack이고 ItemPackMoney 배열을 사용하는 경우
                {
                    // 1~100 사이의 랜덤 값 생성
                    int randomValue = Random.Range(1, 101);
                    int selectedIndex;
                    
                    // 지정된 범위에 따라 인덱스 결정
                    if (randomValue <= 45) // 45% 확률 (1-45)
                        selectedIndex = 0;
                    else if (randomValue <= 65) // 25% 확률 (46-70)
                        selectedIndex = 1;
                    else if (randomValue <= 85) // 15% 확률 (71-85)
                        selectedIndex = 2;
                    else if (randomValue <= 92) // 9% 확률 (86-94)
                        selectedIndex = 3;
                    else // 6% 확률 (95-100)
                        selectedIndex = 4;
                    
                    // 선택된 인덱스가 배열 범위를 벗어나지 않도록 조정
                    selectedIndex = Mathf.Clamp(selectedIndex, 0, prefabsToUse.Length - 1);
                    randomPrefab = prefabsToUse[selectedIndex];
                }
                else
                {
                    // 기존 랜덤 선택 로직
                    randomPrefab = prefabsToUse[Random.Range(0, prefabsToUse.Length)];
                }
                
                if (randomPrefab != null)
                {
                    GameObject newPrefab = Instantiate(randomPrefab, slots[slotIndex].transform);
                    newPrefab.transform.localPosition = Vector3.zero;
                    instantiatedPrefabs.Add(newPrefab);
                }
            }
        }
    }

    private void OnEnable()
    {
        // ShopBox가 활성화되면 프리팹 생성
        InstantiatePrefabs();
    }
    
    private GameObject GetRandomPrefab(GameObject[] prefabArray)
    {
        if (prefabArray == null || prefabArray.Length == 0)
            return null;
            
        return prefabArray[Random.Range(0, prefabArray.Length)];
    }
    
    // 프리팹 가중치를 추적하기 위한 딕셔너리
    private Dictionary<GameObject, int> prefabWeights = new Dictionary<GameObject, int>();
    
    private GameObject GetRandomJokerTypePrefab(bool isSecondSlot = false)
    {
        // 사용 가능한 프리팹 목록
        List<GameObject> availablePrefabs = new List<GameObject>();
        
        // 항상 조커 프리팹 사용
        GameObject[] targetPrefabs = ZokerPrefabs;

        if (targetPrefabs == null || targetPrefabs.Length == 0)
            return null;
        
        // 사용 가능한 프리팹 필터링 (두 번째 슬롯인 경우 이미 사용된 프리팹 제외)
        if (isSecondSlot && usedJokerPrefabs.Count > 0)
        {
            foreach (var prefab in targetPrefabs)
            {
                if (prefab != null && !usedJokerPrefabs.Contains(prefab))
                {
                    availablePrefabs.Add(prefab);
                }
            }
            
            // 사용 가능한 프리팹이 없으면 원본 배열에서 랜덤 선택
            if (availablePrefabs.Count == 0)
            {
                availablePrefabs.AddRange(targetPrefabs);
            }
        }
        else
        {
            availablePrefabs.AddRange(targetPrefabs);
        }
        
        // 사용 가능한 프리팹이 없으면 null 반환
        if (availablePrefabs.Count == 0)
            return null;
            
        // 가중치 초기화 (없는 프리팹에 대해서는 기본값 10 부여)
        foreach (var prefab in availablePrefabs)
        {
            if (!prefabWeights.ContainsKey(prefab))
            {
                prefabWeights[prefab] = 10; // 기본 가중치
            }
        }
        
        // 가중치에 기반한 랜덤 선택
        int totalWeight = 0;
        foreach (var prefab in availablePrefabs)
        {
            totalWeight += prefabWeights[prefab];
        }
        
        int randomWeight = Random.Range(0, totalWeight);
        int weightSum = 0;
        GameObject selectedPrefab = null;
        
        foreach (var prefab in availablePrefabs)
        {
            weightSum += prefabWeights[prefab];
            if (randomWeight < weightSum)
            {
                selectedPrefab = prefab;
                break;
            }
        }
        
        // 선택된 프리팹이 없으면 첫 번째 프리팹 사용 (에러 방지)
        if (selectedPrefab == null)
        {
            selectedPrefab = availablePrefabs[0];
        }
        
        // 선택된 프리팹의 가중치 감소 (다음에 선택될 확률이 줄어듦)
        prefabWeights[selectedPrefab] = Mathf.Max(1, prefabWeights[selectedPrefab] - 2);
        
        // 다른 프리팹들의 가중치 증가 (선택되지 않은 프리팹들이 다음에 선택될 확률이 높아짐)
        foreach (var prefab in availablePrefabs)
        {
            if (prefab != selectedPrefab)
            {
                prefabWeights[prefab] = Mathf.Min(20, prefabWeights[prefab] + 1);
            }
        }
        
        // 첫 번째 슬롯의 프리팹은 추적 목록에 추가
        if (!isSecondSlot && selectedPrefab != null && !usedJokerPrefabs.Contains(selectedPrefab))
        {
            usedJokerPrefabs.Add(selectedPrefab);
        }
        
        return selectedPrefab;
    }

    private void OnDisable()
    {
        // ShopBox가 비활성화되면 프리팹 제거
        ClearPrefabs();
        usedJokerPrefabs.Clear();
    }

    private void InstantiatePrefabs()
    {
        ClearPrefabs();

        // ZokerSlot1에 랜덤 프리팹 생성
        if (ZokerSlot1 != null)
        {
            GameObject prefabToSpawn = GetRandomJokerTypePrefab(false);
            if (prefabToSpawn != null)
            {
                GameObject instance = Instantiate(prefabToSpawn, ZokerSlot1.transform.position, Quaternion.identity, ZokerSlot1.transform);
                // 로컬 포지션 설정
                Vector3 localPos = instance.transform.localPosition;
                localPos.z = -1f;
                instance.transform.localPosition = localPos;
                // sortingOrder 설정
                SpriteRenderer renderer = instance.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sortingOrder = -7;
                }
                instantiatedPrefabs.Add(instance);
            }
        }
        
        // ZokerSlot2에 랜덤 프리팹 생성 (ZokerSlot1과 다른 프리팹이 나오도록 보장)
        if (ZokerSlot2 != null)
        {
            GameObject prefabToSpawn = GetRandomJokerTypePrefab(true);
            if (prefabToSpawn != null)
            {
                GameObject instance = Instantiate(prefabToSpawn, ZokerSlot2.transform.position, Quaternion.identity, ZokerSlot2.transform);
                // 로컬 포지션 설정
                Vector3 localPos = instance.transform.localPosition;
                localPos.z = -1f;
                instance.transform.localPosition = localPos;
                // sortingOrder 설정
                SpriteRenderer renderer = instance.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sortingOrder = -7;
                }
                instantiatedPrefabs.Add(instance);
            }
        }
        
        // ItemPack 슬롯에 프리팹 생성
        if (ItemPackSlot1 != null && ItemPackPrefabs.Length > 0)
        {
            GameObject prefabToSpawn = GetRandomPrefab(ItemPackPrefabs);
            if (prefabToSpawn != null)
            {
                GameObject instance = Instantiate(prefabToSpawn, ItemPackSlot1.transform.position, Quaternion.identity, ItemPackSlot1.transform);
                // 로컬 포지션 설정
                Vector3 localPos = instance.transform.localPosition;
                localPos.z = -1f;
                instance.transform.localPosition = localPos;
                // sortingOrder 설정
                SpriteRenderer renderer = instance.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sortingOrder = -7;
                }
                instantiatedPrefabs.Add(instance);
            }
        }
        
        if (ItemPackSlot2 != null && MoneyPrefabs.Length > 0)
        {
            GameObject prefabToSpawn = GetRandomPrefab(MoneyPrefabs);
            if (prefabToSpawn != null)
            {
                GameObject instance = Instantiate(prefabToSpawn, ItemPackSlot2.transform.position, Quaternion.identity, ItemPackSlot2.transform);
                // 로컬 포지션 설정
                Vector3 localPos = instance.transform.localPosition;
                localPos.z = -1f;
                instance.transform.localPosition = localPos;
                // sortingOrder 설정
                SpriteRenderer renderer = instance.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sortingOrder = -7;
                }
                instantiatedPrefabs.Add(instance);
            }
        }
        
        // Voucher 슬롯에 프리팹 생성
        if (VoucherSlot1 != null && VoucherPrefabs.Length > 0)
        {
            GameObject prefabToSpawn = GetRandomPrefab(VoucherPrefabs);
            if (prefabToSpawn != null)
            {
                GameObject instance = Instantiate(prefabToSpawn, VoucherSlot1.transform.position, Quaternion.identity, VoucherSlot1.transform);
                // 로컬 포지션 설정
                Vector3 localPos = instance.transform.localPosition;
                localPos.z = -1f;
                instance.transform.localPosition = localPos;
                // sortingOrder 설정
                SpriteRenderer renderer = instance.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sortingOrder = -7;
                }
                instantiatedPrefabs.Add(instance);
            }
        }
    }

    private void ClearPrefabs()
    {
        // 생성된 모든 프리팹 제거
        foreach (var prefab in instantiatedPrefabs)
        {
            if (prefab != null)
            {
                Destroy(prefab);
            }
        }
        instantiatedPrefabs.Clear();
    }

    private void UpdateUI()
    {
        RerollCostText.text = "$" + RerollCost.ToString("N0");
    }

    public void OnSkipBtnClick()
    {
        ShopBoxUpAndDown shopBoxUpAndDown = FindObjectOfType<ShopBoxUpAndDown>();
        if (shopBoxUpAndDown.isShopBoxDown)
        {
            shopBoxUpAndDown.MoveShopBoxUp();
            shopBoxUpAndDown.ItemSelectZoneDown();
        }
        SkipBtn.SetActive(false);
    }
    public void NoSkipSelectJoker()
    {
        SkipBtn.SetActive(false);
    }

    public void OnNextBtnClick()
    {
        gameManager.PlusRound();
        Debug.Log("엔티 : " + gameManager.playerData.ante + "\n");
        Debug.Log("라운드 : " + gameManager.playerData.round + "\n");
        Debug.Log("보유한 돈 : $" + gameManager.playerData.money.ToString("N0") + "\n");
        SceneManager.LoadScene("Stage");
    }
    

    // ZokerSlot1과 ZokerSlot2의 프리팹을 새로운 랜덤 프리팹으로 교체 (서로 다른 프리팹이 생성됨)
    public void OnRerollBtnClick()
    {
        Debug.Log("리롤 버튼 클릭!");

        gameManager.Reroll(RerollCost);
        RerollCost += 1;
        UpdateUI();
        // 이전에 사용된 조커 프리팹 저장
        List<GameObject> previousJokers = new List<GameObject>();

        // 현재 ZokerSlot1과 ZokerSlot2의 자식 프리팹 찾기
        if (ZokerSlot1 != null && ZokerSlot1.transform.childCount > 0)
        {
            var child = ZokerSlot1.transform.GetChild(0);
            if (child != null)
            {
                previousJokers.Add(child.gameObject);
                instantiatedPrefabs.Remove(child.gameObject);
                Destroy(child.gameObject);
            }
        }
        
        if (ZokerSlot2 != null && ZokerSlot2.transform.childCount > 0)
        {
            var child = ZokerSlot2.transform.GetChild(0);
            if (child != null)
            {
                previousJokers.Add(child.gameObject);
                instantiatedPrefabs.Remove(child.gameObject);
                Destroy(child.gameObject);
            }
        }
        
        // 첫 번째 슬롯에 새로운 프리팹 생성
        if (ZokerSlot1 != null)
        {
            // 항상 조커 프리팹 사용
            GameObject[] targetPrefabs = ZokerPrefabs;
            if (targetPrefabs != null && targetPrefabs.Length > 0)
            {
                // 사용 가능한 프리팹 필터링
                var availablePrefabs = targetPrefabs.Where(p => p != null).ToList();
                
                // 이전에 사용된 프리팹 제외
                if (previousJokers.Count > 0)
                {
                    availablePrefabs = availablePrefabs
                        .Where(p => !previousJokers.Exists(j => j != null && j.name.Replace("(Clone)", "") == p.name))
                        .ToList();
                    
                    // 사용 가능한 프리팹이 없으면 필터링 없이 다시 시도
                    if (availablePrefabs.Count == 0)
                        availablePrefabs = targetPrefabs.Where(p => p != null).ToList();
                }
                
                if (availablePrefabs.Count > 0)
                {
                    // 가중치에 따라 랜덤 선택
                    int index = Random.Range(0, availablePrefabs.Count);
                    GameObject selectedPrefab = availablePrefabs[index];
                    
                    // 프리팹 생성
                    GameObject instance = Instantiate(selectedPrefab, ZokerSlot1.transform.position, Quaternion.identity, ZokerSlot1.transform);
                    Vector3 localPos = instance.transform.localPosition;
                    localPos.z = -1f;
                    instance.transform.localPosition = localPos;
                    SpriteRenderer renderer = instance.GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {
                        renderer.sortingOrder = -7;
                    }
                    instantiatedPrefabs.Add(instance);
                    
                    // 두 번째 슬롯을 위해 첫 번째 슬롯의 프리팹 저장
                    previousJokers.Add(instance);
                }
            }
        }
        
        // 두 번째 슬롯에 새로운 프리팹 생성 (첫 번째 슬롯과 다른 프리팹이어야 함)
        if (ZokerSlot2 != null)
        {
            // 항상 조커 프리팹 사용
            GameObject[] targetPrefabs = ZokerPrefabs;
            if (targetPrefabs != null && targetPrefabs.Length > 0)
            {
                // 사용 가능한 프리팹 필터링
                var availablePrefabs = targetPrefabs.Where(p => p != null).ToList();
                
                // 첫 번째 슬롯의 프리팹과 다른 프리팹만 선택
                if (previousJokers.Count > 0)
                {
                    availablePrefabs = availablePrefabs
                        .Where(p => !previousJokers.Exists(j => j != null && j.name.Replace("(Clone)", "") == p.name))
                        .ToList();
                    
                    // 사용 가능한 프리팹이 없으면 필터링 없이 다시 시도 (다른 카테고리에서 선택)
                    if (availablePrefabs.Count == 0)
                    {
                        // 다른 카테고리에서 선택 (조커가 아닌 다른 카테고리)
                        GameObject[] altPrefabs = targetPrefabs == ZokerPrefabs ? 
                            (TaroPrefabs.Length > 0 ? TaroPrefabs : PlanetPrefabs) :
                            ZokerPrefabs;
                            
                        if (altPrefabs != null && altPrefabs.Length > 0)
                            availablePrefabs = altPrefabs.Where(p => p != null).ToList();
                    }
                }
                
                if (availablePrefabs.Count > 0)
                {
                    // 가중치에 따라 랜덤 선택
                    int index = Random.Range(0, availablePrefabs.Count);
                    GameObject selectedPrefab = availablePrefabs[index];
                    
                    // 프리팹 생성
                    GameObject instance = Instantiate(selectedPrefab, ZokerSlot2.transform.position, Quaternion.identity, ZokerSlot2.transform);
                    Vector3 localPos = instance.transform.localPosition;
                    localPos.z = -1f;
                    instance.transform.localPosition = localPos;
                    SpriteRenderer renderer = instance.GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {
                        renderer.sortingOrder = -7;
                    }
                    instantiatedPrefabs.Add(instance);
                }
            }
        }
    }

}
