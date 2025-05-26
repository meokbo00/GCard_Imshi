using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenarateItem : MonoBehaviour
{
    public GameObject ZokerSlot1;
    public GameObject ZokerSlot2;
    public GameObject ItemPackSlot1;
    public GameObject ItemPackSlot2;
    public GameObject VoucherSlot1;

    public GameObject ZokerPrefab;
    public GameObject TaroPrefab;
    public GameObject PlanetPrefab;
    public GameObject ItemPackPrefab;
    public GameObject VoucherPrefab;


    private List<GameObject> instantiatedPrefabs = new List<GameObject>();

    private void OnEnable()
    {
        // ShopBox가 활성화되면 프리팹 생성
        InstantiatePrefabs();
    }

    private void OnDisable()
    {
        // ShopBox가 비활성화되면 프리팹 제거
        ClearPrefabs();
    }

    private GameObject GetRandomPrefab()
    {
        // 사용 가능한 프리팹이 없으면 null 반환
        if (ZokerPrefab == null && TaroPrefab == null && PlanetPrefab == null)
            return null;
        
        // 랜덤 값 생성 (0~99)
        float randomValue = Random.Range(0f, 100f);
        
        // 확률에 따라 프리팹 선택
        // ZokerPrefab: 80% (0~79.999...)
        // TaroPrefab: 10% (80~89.999...)
        // PlanetPrefab: 10% (90~99.999...)
        
        if (randomValue < 80f && ZokerPrefab != null)
        {
            return ZokerPrefab;
        }
        else if (randomValue < 90f && TaroPrefab != null)
        {
            return TaroPrefab;
        }
        else if (PlanetPrefab != null)
        {
            return PlanetPrefab;
        }
        
        // 만약 선택된 프리팹이 null이면 ZokerPrefab을 반환 (null 방지)
        return ZokerPrefab ?? TaroPrefab ?? PlanetPrefab;
    }

    private void InstantiatePrefabs()
    {
        ClearPrefabs();

        // ZokerSlot1에 랜덤 프리팹 생성
        if (ZokerSlot1 != null)
        {
            GameObject prefabToSpawn = GetRandomPrefab();
            if (prefabToSpawn != null)
                instantiatedPrefabs.Add(Instantiate(prefabToSpawn, ZokerSlot1.transform.position, Quaternion.identity, ZokerSlot1.transform));
        }
        
        // ZokerSlot2에 랜덤 프리팹 생성 (ZokerSlot1과 다른 프리팹이 나올 수 있음)
        if (ZokerSlot2 != null)
        {
            GameObject prefabToSpawn = GetRandomPrefab();
            if (prefabToSpawn != null)
                instantiatedPrefabs.Add(Instantiate(prefabToSpawn, ZokerSlot2.transform.position, Quaternion.identity, ZokerSlot2.transform));
        }
        
        // Instantiate ItemPack prefabs
        if (ItemPackSlot1 != null && ItemPackPrefab != null)
            instantiatedPrefabs.Add(Instantiate(ItemPackPrefab, ItemPackSlot1.transform.position, Quaternion.identity, ItemPackSlot1.transform));
        
        if (ItemPackSlot2 != null && ItemPackPrefab != null)
            instantiatedPrefabs.Add(Instantiate(ItemPackPrefab, ItemPackSlot2.transform.position, Quaternion.identity, ItemPackSlot2.transform));
        
        // Instantiate Voucher prefab
        if (VoucherSlot1 != null && VoucherPrefab != null)
            instantiatedPrefabs.Add(Instantiate(VoucherPrefab, VoucherSlot1.transform.position, Quaternion.identity, VoucherSlot1.transform));
    }

    private void ClearPrefabs()
    {
        // Destroy all instantiated prefabs
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
