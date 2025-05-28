using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
        // 랜덤하게 프리팹 선택
        if (availablePrefabs.Count > 0)
        {
            GameObject selectedPrefab = availablePrefabs[Random.Range(0, availablePrefabs.Count)];
            
            // 첫 번째 슬롯의 프리팹은 추적 목록에 추가
            if (!isSecondSlot && selectedPrefab != null && !usedJokerPrefabs.Contains(selectedPrefab))
            {
                usedJokerPrefabs.Add(selectedPrefab);
            }
            
            return selectedPrefab;
        }
        
        return null;
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


}
