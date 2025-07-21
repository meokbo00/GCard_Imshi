using UnityEngine;

public class MoonMoving : MonoBehaviour
{
    [Header("공전 설정")]
    [SerializeField] private Transform target;           // 공전할 기준 오브젝트
    [SerializeField] private float orbitRadius = 5f;     // 공전 반경
    [SerializeField] private float orbitSpeed = 30f;     // 공전 속도 (도/초)
    
    private float currentAngle = 0f;
    private Vector3 offset;

    private void Start()
    {
        // 초기 각도 랜덤 설정
        currentAngle = Random.Range(0f, 360f);
        
        // 초기 위치 설정
        UpdatePosition();
    }

    private void Update()
    {
        if (target == null) return;
        
        // 각도 업데이트 (시계 방향으로 회전)
        currentAngle -= orbitSpeed * Time.deltaTime;
        
        // 위치 업데이트
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        // 라디안으로 변환
        float radian = currentAngle * Mathf.Deg2Rad;
        
        // 원형 궤도 계산 (XY 평면에서의 원운동)
        float x = Mathf.Cos(radian) * orbitRadius;
        float y = Mathf.Sin(radian) * orbitRadius;
        
        // 타겟의 위치를 기준으로 공전
        transform.position = new Vector3(
            target.position.x + x,
            target.position.y + y,
            target.position.z  // Z축은 타겟과 동일하게 유지
        );
    }
}
