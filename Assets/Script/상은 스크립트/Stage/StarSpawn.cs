using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

public class StarSpawn : MonoBehaviour
{
    [Header("Star Prefabs")]
    public GameObject[] starPrefabs;  // 서로 다른 별 프리팹들
    
    [Header("Spawn Settings")]
    [SerializeField] private int maxActiveStars = 30;  // 최대 활성화될 별 개수
    [SerializeField] private float minDisplayTime = 0.5f;  // 최소 표시 시간
    [SerializeField] private float maxDisplayTime = 2.5f;  // 최대 표시 시간
    [SerializeField] private float minSpawnDelay = 0.1f;   // 최소 생성 간격
    [SerializeField] private float maxSpawnDelay = 0.3f;   // 최대 생성 간격
    
    private const float MIN_X = -950f;
    private const float MAX_X = 950f;
    private const float MIN_Y = -530f;
    private const float MAX_Y = 530f;
    
    private List<GameObject> activeStars = new List<GameObject>();
    private Queue<GameObject> starPool = new Queue<GameObject>();
    private Transform starsParent;  // 생성된 별들을 정리할 부모 오브젝트

    private Canvas parentCanvas;
    private RectTransform canvasRect;
    private Dictionary<int, Vector3> starScales = new Dictionary<int, Vector3>(); // 프리팹 인덱스별 스케일 저장

    private void Start()
    {
        if (starPrefabs == null || starPrefabs.Length == 0)
        {
            Debug.LogError("No star prefabs assigned!");
            return;
        }
        
        // 캔버스 설정
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogError("StarSpawn must be a child of a Canvas!");
            return;
        }
        canvasRect = parentCanvas.GetComponent<RectTransform>();
        
        // 별들을 정리할 부모 오브젝트 생성 (UI용)
        GameObject starsParentObj = new GameObject("Stars", typeof(RectTransform));
        starsParent = starsParentObj.transform;
        starsParent.SetParent(transform, false);
        
        // 프리팹별 스케일 미리 설정
        for (int i = 0; i < starPrefabs.Length; i++)
        {
            if (starPrefabs[i] == null) continue;
            
            string name = starPrefabs[i].name;
            if (name.Contains("BlueStar") || name.Contains("YellowStar"))
            {
                starScales[i] = new Vector3(2f, 2f, 1f);
            }
            else if (name.Contains("Cornet"))
            {
                starScales[i] = new Vector3(6f, 6f, 1f);
            }
            else
            {
                starScales[i] = Vector3.one;
            }
        }
        
        // 별 생성 시작
        StartCoroutine(StarSpawningRoutine());
    }
    
    private GameObject GetStarFromPool()
    {
        // 풀에서 사용 가능한 별이 있으면 반환
        if (starPool.Count > 0)
        {
            return starPool.Dequeue();
        }
        
        // 풀이 비어있으면 새로 생성 (랜덤한 프리팹 선택)
        int randomIndex = Random.Range(0, starPrefabs.Length);
        var newStar = Instantiate(starPrefabs[randomIndex], starsParent, false);
        newStar.SetActive(false);
        
        // 프리팹 인덱스 저장 (스케일 정보 접근용)
        var info = newStar.AddComponent<StarInfo>();
        info.PrefabIndex = randomIndex;
        
        // UI 요소인 경우 RectTransform 설정
        var rectTransform = newStar.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale = Vector3.one;
        }
        
        return newStar;
    }
    
    private void ReturnStarToPool(GameObject star)
    {
        if (star == null) return;
        
        star.SetActive(false);
        star.transform.SetParent(starsParent);
        starPool.Enqueue(star);
    }

    private IEnumerator StarSpawningRoutine()
    {
        while (true)
        {
            // 현재 활성화된 별 개수 확인
            int currentActive = activeStars.Count(star => star != null && star.activeSelf);
            
            // 최대 개수보다 적으면 새로운 별 생성
            if (currentActive < maxActiveStars)
            {
                ShowStarWithTween();
            }
            
            // 다음 별 생성을 위한 랜덤 딜레이
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
        }
    }
    
    private void ShowStarWithTween()
    {
        // 풀에서 별 가져오기
        GameObject star = GetStarFromPool();
        if (star == null) return;
        
        // UI 위치 설정 (캔버스 내에서 랜덤 위치)
        if (star.TryGetComponent<RectTransform>(out var rectTransform) && canvasRect != null)
        {
            // 캔버스 크기에 맞게 위치 조정
            float xPos = Random.Range(-canvasRect.rect.width * 0.4f, canvasRect.rect.width * 0.4f);
            float yPos = Random.Range(-canvasRect.rect.height * 0.4f, canvasRect.rect.height * 0.4f);
            rectTransform.anchoredPosition = new Vector2(xPos, yPos);
        }
        
        // 초기 상태 설정
        star.transform.localScale = Vector3.zero;
        
        // 저장된 프리팹 인덱스로 스케일 가져오기
        var starInfo = star.GetComponent<StarInfo>();
        Vector3 targetScale = starInfo != null && starScales.TryGetValue(starInfo.PrefabIndex, out var scale) 
            ? scale 
            : Vector3.one; // 기본 스케일
        
        star.SetActive(true);
        
        // 별을 활성화 목록에 추가
        if (!activeStars.Contains(star))
        {
            activeStars.Add(star);
        }

        // 랜덤한 지속 시간 설정
        float duration = Random.Range(minDisplayTime, maxDisplayTime);
        
        // 1. 기본 등장/사라짐 애니메이션
        Sequence mainSequence = DOTween.Sequence();
        
        // 페이드 인
        mainSequence.Append(star.transform.DOScale(targetScale, 0.5f)
            .SetEase(Ease.OutBack));
        
        // 유지 시간 (전체 지속 시간에서 페이드 인/아웃 시간을 뺌)
        mainSequence.AppendInterval(duration - 1.0f);
        
        // 페이드 아웃
        mainSequence.Append(star.transform.DOScale(0f, 0.5f)
            .SetEase(Ease.InBack));
        
        // Cornet만 위 또는 아래로 이동하는 효과
        if (star.name.Contains("Cornet"))
        {
            float moveDistanceY = Random.Range(160f, 320f); // 세로 이동 거리
            float moveDistanceX = Random.Range(160f, 320f);  // 가로 이동 거리 (왼쪽으로)
            float moveDuration = Random.Range(2f, 3f);     // 이동 시간
            
            // 왼쪽 아래 대각선 방향으로 이동
            Vector2 startPos = rectTransform.anchoredPosition;
            Vector2 targetPos = new Vector2(
                startPos.x - moveDistanceX,  // 왼쪽으로
                startPos.y - moveDistanceY   // 아래로
            );
            
            // 이동 애니메이션
            rectTransform.DOAnchorPos(targetPos, moveDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo); // 왕복 반복
        }
        
        // 메인 시퀀스 완료 시 모든 애니메이션 정리
        mainSequence.OnComplete(() => {
            if (star != null)
            {
                // 모든 트윈 종료
                DOTween.Kill(star.transform);
                DOTween.Kill(rectTransform);
                
                activeStars.Remove(star);
                ReturnStarToPool(star);
            }
        });
        
        // 시퀀스 재생
        mainSequence.Play();
    }
}
