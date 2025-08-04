using UnityEngine;
using DG.Tweening;

public class CameraMoving : MonoBehaviour
{
    public GameObject PlanetExplainBox;
    public GameObject[] stars;

    [Header("맨 처음 카메라와 확대할 사이즈")]
    [SerializeField] private float initialSize = 5000f;
    [SerializeField] private float targetSize = 70f;
    [Header("첫 카메라 확대 대기 시간, 대기 후 확대 시간")]
    [SerializeField] private float startDelay = 10f;
    [SerializeField] private float zoomDuration = 5f;
    [Header("플레이할 행성 차례")]
    public int index = 0;



    private Camera mainCamera;
    private Tween zoomTween;
    private Transform targetObject;  // 추적할 타겟 오브젝트
    private Vector3 velocity = Vector3.zero;  // 부드러운 이동을 위한 벡터
    private float smoothTime = 0.3f;  // 부드러운 이동에 걸리는 시간
    
    // 인덱스별 카메라 사이즈 배열
    private readonly float[] cameraSizes = { 0.6f, 1.35f, 1.5f, 1.2f, 0.7f, 7.7f, 8f, 6f, 4.5f, 70f };

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        //InitializeCamera();
        //StartZoomSequence();
        MovingStarToStar(index, index + 1);
    }

    // 게임 맨 처음 시작시 시작할 메서드. 맨 끝에서 줌인한다
    private void InitializeCamera()
    {
        // 카메라 초기화
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = initialSize;
    }

    private void StartZoomSequence()
    {
        // 시작 대기 후 줌 인 시작
        DOVirtual.DelayedCall(startDelay, () =>
        {
            zoomTween = mainCamera.DOOrthoSize(
                targetSize,
                zoomDuration
            )
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => {
                Debug.Log("Camera zoom completed!");
                // 줌 인이 완료된 후 5초 대기 후에 별을 따라가기 시작
                DOVirtual.DelayedCall(1f, () => {
                    FollowStarAt(index);
                });
            });
        });
    }

    private bool isMovingToTarget = false;
    private Vector3 moveStartPosition;
    private float moveStartTime;
    private float moveDuration = 2f; // 이동에 걸리는 시간 (초)

    private void LateUpdate()
    {
        // 타겟 오브젝트가 있으면 카메라 위치 업데이트
        if (targetObject != null)
        {
            Vector3 targetPosition = targetObject.position;
            targetPosition.z = mainCamera.transform.position.z;  // z값은 유지
            
            if (isMovingToTarget)
            {
                // 이동 애니메이션 중일 때
                float timeSinceStarted = Time.time - moveStartTime;
                float percentageComplete = timeSinceStarted / moveDuration;
                
                // EaseInOut 효과 적용
                percentageComplete = Mathf.SmoothStep(0, 1, percentageComplete);
                
                // 부드러운 이동
                mainCamera.transform.position = Vector3.Lerp(moveStartPosition, targetPosition, percentageComplete);
                
                // 이동 완료 체크
                if (percentageComplete >= 1.0f)
                {
                    isMovingToTarget = false;
                }
            }
            else
            {
                // 이동이 완료된 후에는 타겟을 정확히 추적
                mainCamera.transform.position = targetPosition;
            }
        }
    }

    /// 지정된 인덱스의 별을 카메라가 따라가도록 설정합니다.
    public void FollowStarAt(int index)
    {
        if (index >= 0 && index < stars.Length && stars[index] != null)
        {
            // 카메라 사이즈 설정 (인덱스가 배열 범위를 벗어나면 마지막 값 사용)
            float targetSize = index < cameraSizes.Length ? cameraSizes[index] : cameraSizes[^1];
            
            // 1. 현재 위치에서 타겟으로 부드럽게 이동 후 콜백 실행
            SetCameraTarget(stars[index].transform, () => {
                // 2. 이동이 완료된 후에만 줌인 애니메이션 실행
                mainCamera.DOOrthoSize(targetSize, 4f)
                    .SetEase(Ease.InOutQuad);
            });
        }
        else
        {
            Debug.LogWarning($"유효하지 않은 별 인덱스입니다: {index}");
        }
    }

    /// 카메라가 따라갈 타겟 오브젝트를 설정합니다.
    /// <param name="onComplete">이동이 완료된 후 실행할 액션</param>
    private void SetCameraTarget(Transform target, System.Action onComplete = null)
    {
        if (target != null)
        {
            // 이동 시작 위치 저장
            moveStartPosition = mainCamera.transform.position;
            moveStartTime = Time.time;
            isMovingToTarget = true;
            
            // 타겟 오브젝트 설정
            targetObject = target;
            
            // 이동 완료 후 콜백을 위해 코루틴 시작
            StartCoroutine(WaitForMoveComplete(onComplete));
        }
    }

    private void MovingStarToStar(int currentindex, int afterindex)
    {
        if (currentindex < 0 || currentindex >= stars.Length || stars[currentindex] == null)
        {
            Debug.LogError($"유효하지 않은 시작 인덱스입니다: {currentindex}");
            return;
        }

        // 현재 카메라의 모든 트윈 중지
        mainCamera.DOKill();
        zoomTween?.Kill();
        
        // 타겟 오브젝트 설정
        targetObject = stars[currentindex].transform;
        
        // 카메라 위치 즉시 설정 (현재 별 정중앙으로)
        Vector3 targetPosition = targetObject.position;
        targetPosition.z = mainCamera.transform.position.z;
        mainCamera.transform.position = targetPosition;
        
        // 카메라 사이즈 즉시 설정 (인덱스에 해당하는 사이즈로)
        if (currentindex < cameraSizes.Length)
        {
            mainCamera.orthographicSize = cameraSizes[currentindex];
        }
        else
        {
            mainCamera.orthographicSize = cameraSizes[^1]; // 배열 범위를 벗어나면 마지막 값 사용
        }
        
        // 이동 상태 초기화
        isMovingToTarget = false;
        
        Debug.Log($"별 {currentindex} 위치에서 게임을 시작합니다. 카메라 사이즈: {mainCamera.orthographicSize}");
        
        // 3초 후 afterindex로 부드럽게 이동
        if (afterindex >= 0 && afterindex < stars.Length && stars[afterindex] != null)
        {
            DOVirtual.DelayedCall(3f, () => {
                // 이동 애니메이션 시작
                isMovingToTarget = true;
                moveStartPosition = mainCamera.transform.position;
                moveStartTime = Time.time;
                
                // 타겟 오브젝트 업데이트
                targetObject = stars[afterindex].transform;
                
                // 이동 시간 설정 (5초)
                moveDuration = 5f;
                
                // 2.5초 동안 카메라 사이즈를 10으로 조정
                mainCamera.DOOrthoSize(10f, 2.5f)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() => {
                        // 나머지 2.5초 동안 목표 사이즈로 조정
                        float targetSize = afterindex < cameraSizes.Length ? cameraSizes[afterindex] : cameraSizes[^1];
                        mainCamera.DOOrthoSize(targetSize, 2.5f)
                            .SetEase(Ease.InOutQuad);
                    });
                
                // 이동 완료 후 실행할 액션
                System.Action onMoveComplete = () => {
                    isMovingToTarget = false;
                    Debug.Log($"별 {afterindex} 위치로 이동 완료. 카메라 사이즈: {mainCamera.orthographicSize}");
                };
                
                // 이동 완료를 감지하기 위한 코루틴 시작
                StartCoroutine(WaitForMoveComplete(onMoveComplete));
            });
        }
    }
    
    // 이동이 완료될 때까지 대기한 후 콜백 실행
    private System.Collections.IEnumerator WaitForMoveComplete(System.Action onComplete)
    {
        // 이동이 완료될 때까지 대기
        while (isMovingToTarget)
        {
            yield return null;
        }
        
        // 이동이 완료된 후 콜백 실행
        onComplete?.Invoke();
        PlanetExplainBox.SetActive(true);
    }
    
    private void OnDestroy()
    {
        zoomTween?.Kill();
    }
}
