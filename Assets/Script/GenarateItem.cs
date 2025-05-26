using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    private Dictionary<GameObject, Sprite> prefabSpriteMap = new Dictionary<GameObject, Sprite>();

    private void OnEnable()
    {
        // ShopBox가 활성화되면 프리팹 생성
        InstantiatePrefabs();
    }

    private Sprite LoadRandomSprite(string folderName)
    {
        // Resources 폴더에서 해당 폴더의 모든 스프라이트 로드
        Sprite[] sprites = Resources.LoadAll<Sprite>($"ShopItem/{folderName}");
        
        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogWarning($"No sprites found in folder: {folderName}");
            return null;
        }
        
        // 랜덤한 스프라이트 반환
        return sprites[Random.Range(0, sprites.Length)];
    }
    
    private void ApplyRandomSprite(GameObject prefab, string folderName)
    {
        if (prefab == null) return;
        
        // 이미 해당 프리팹에 대한 스프라이트가 로드되어 있지 않은 경우에만 로드
        if (!prefabSpriteMap.ContainsKey(prefab))
        {
            Sprite randomSprite = LoadRandomSprite(folderName);
            if (randomSprite != null)
            {
                prefabSpriteMap[prefab] = randomSprite;
            }
        }
        
        // 스프라이트 적용
        if (prefabSpriteMap.TryGetValue(prefab, out Sprite sprite))
        {
            SpriteRenderer renderer = prefab.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sprite = sprite;
            }
        }
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
            {
                GameObject instance = Instantiate(prefabToSpawn, ZokerSlot1.transform.position, Quaternion.identity, ZokerSlot1.transform);
                instantiatedPrefabs.Add(instance);
                
                // 프리팹 타입에 따라 적절한 폴더에서 스프라이트 로드 및 적용
                if (prefabToSpawn == ZokerPrefab)
                    ApplyRandomSprite(instance, "Joker");
                else if (prefabToSpawn == TaroPrefab)
                    ApplyRandomSprite(instance, "Taro");
                else if (prefabToSpawn == PlanetPrefab)
                    ApplyRandomSprite(instance, "Planet");
            }
        }
        
        // ZokerSlot2에 랜덤 프리팹 생성 (ZokerSlot1과 다른 프리팹이 나올 수 있음)
        if (ZokerSlot2 != null)
        {
            GameObject prefabToSpawn = GetRandomPrefab();
            if (prefabToSpawn != null)
            {
                GameObject instance = Instantiate(prefabToSpawn, ZokerSlot2.transform.position, Quaternion.identity, ZokerSlot2.transform);
                instantiatedPrefabs.Add(instance);
                
                // 프리팹 타입에 따라 적절한 폴더에서 스프라이트 로드 및 적용
                if (prefabToSpawn == ZokerPrefab)
                    ApplyRandomSprite(instance, "Joker");
                else if (prefabToSpawn == TaroPrefab)
                    ApplyRandomSprite(instance, "Taro");
                else if (prefabToSpawn == PlanetPrefab)
                    ApplyRandomSprite(instance, "Planet");
            }
        }
        
        // Instantiate ItemPack prefabs
        if (ItemPackSlot1 != null && ItemPackPrefab != null)
        {
            GameObject instance = Instantiate(ItemPackPrefab, ItemPackSlot1.transform.position, Quaternion.identity, ItemPackSlot1.transform);
            instantiatedPrefabs.Add(instance);
            ApplyRandomSprite(instance, "ItemPack");
        }
        
        if (ItemPackSlot2 != null && ItemPackPrefab != null)
        {
            GameObject instance = Instantiate(ItemPackPrefab, ItemPackSlot2.transform.position, Quaternion.identity, ItemPackSlot2.transform);
            instantiatedPrefabs.Add(instance);
            ApplyRandomSprite(instance, "ItemPack");
        }
        
        // Instantiate Voucher prefab
        if (VoucherSlot1 != null && VoucherPrefab != null)
        {
            GameObject instance = Instantiate(VoucherPrefab, VoucherSlot1.transform.position, Quaternion.identity, VoucherSlot1.transform);
            instantiatedPrefabs.Add(instance);
            ApplyRandomSprite(instance, "Voucher");
        }
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
        prefabSpriteMap.Clear();
    }


}
