using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GenarateItem : MonoBehaviour
{
    [Header("슬롯 참조")]
    public GameObject ZokerSlot1;
    public GameObject ZokerSlot2;
    public GameObject ItemPackSlot1;
    public GameObject ItemPackSlot2;
    public GameObject VoucherSlot1;

    [Header("프리팹 배열")]
    public GameObject[] ZokerPrefabs;
    public GameObject[] TaroPrefabs;
    public GameObject[] PlanetPrefabs;
    public GameObject[] ItemPackPrefabs;
    public GameObject[] VoucherPrefabs;

    private List<GameObject> instantiatedPrefabs = new List<GameObject>();
    private List<GameObject> usedJokerPrefabs = new List<GameObject>();

    GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
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
        // 랜덤 값 생성 (0~99)
        float randomValue = Random.Range(0f, 100f);
        
        // 사용 가능한 프리팹 목록
        List<GameObject> availablePrefabs = new List<GameObject>();
        
        // 확률에 따라 프리팹 배열 선택
        // 조커: 80% (0~79.999...)
        // 타로: 10% (80~89.999...)
        // 행성: 10% (90~99.999...)
        
        GameObject[] targetPrefabs = null;
        
        if (randomValue < 80f && ZokerPrefabs.Length > 0)
        {
            targetPrefabs = ZokerPrefabs;
        }
        else if (randomValue < 90f && TaroPrefabs.Length > 0)
        {
            targetPrefabs = TaroPrefabs;
        }
        else if (PlanetPrefabs.Length > 0)
        {
            targetPrefabs = PlanetPrefabs;
        }
        else
        {
            // 기본적으로 조커 프리팹 사용
            targetPrefabs = ZokerPrefabs;
        }
        
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
                instantiatedPrefabs.Add(instance);
            }
        }
        
        if (ItemPackSlot2 != null && ItemPackPrefabs.Length > 0)
        {
            GameObject prefabToSpawn = GetRandomPrefab(ItemPackPrefabs);
            if (prefabToSpawn != null)
            {
                GameObject instance = Instantiate(prefabToSpawn, ItemPackSlot2.transform.position, Quaternion.identity, ItemPackSlot2.transform);
                // 로컬 포지션 설정
                Vector3 localPos = instance.transform.localPosition;
                localPos.z = -1f;
                instance.transform.localPosition = localPos;
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

    // ZokerSlot1과 ZokerSlot2의 프리팹을 새로운 랜덤 프리팹으로 교체 (서로 다른 프리팹이 생성됨)
    public void OnRerollBtnClick()
    {

        gameManager.Reroll();
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
        
        // 확률에 따라 프리팹 배열 선택 (조커 80%, 타로 10%, 행성 10%)
        GameObject[] GetRandomPrefabArray()
        {
            float randomValue = Random.Range(0f, 100f);
            Debug.Log("randomValue : " + randomValue);
            if (randomValue < 80f && ZokerPrefabs.Length > 0)
                return ZokerPrefabs;
            else if (randomValue < 90f && TaroPrefabs.Length > 0)
                return TaroPrefabs;
            else if (PlanetPrefabs.Length > 0)
                return PlanetPrefabs;
            else
                return ZokerPrefabs; // 기본값으로 조커 프리팹 반환
        }
        
        // 첫 번째 슬롯에 새로운 프리팹 생성
        if (ZokerSlot1 != null)
        {
            // 확률에 따라 프리팹 배열 선택
            GameObject[] targetPrefabs = GetRandomPrefabArray();
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
                    instantiatedPrefabs.Add(instance);
                    
                    // 두 번째 슬롯을 위해 첫 번째 슬롯의 프리팹 저장
                    previousJokers.Add(instance);
                }
            }
        }
        
        // 두 번째 슬롯에 새로운 프리팹 생성 (첫 번째 슬롯과 다른 프리팹이어야 함)
        if (ZokerSlot2 != null)
        {
            // 확률에 따라 프리팹 배열 선택
            GameObject[] targetPrefabs = GetRandomPrefabArray();
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
                    instantiatedPrefabs.Add(instance);
                }
            }
        }
    }

}
