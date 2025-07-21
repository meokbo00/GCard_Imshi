using UnityEngine;

public class Moving : MonoBehaviour
{
    [Header("공전 설정")]
    [SerializeField] private float orbitRadius = 5f;     // 공전 반경
    [SerializeField] private float orbitSpeed = 30f;     // 공전 속도 (도/초)
    
    private Vector3 orbitCenter = new Vector3(0, 0, 0); // 고정된 중심점
    private float currentAngle = 0f;

    private void Start()
    {
        // 게임 시작 시 랜덤한 각도(0~360도)에서 시작
        currentAngle = Random.Range(0f, 360f);
        
        // 초기 위치 설정
        UpdatePosition();
    }

    private void Update()
    {
        // 각도 업데이트 (시계 방향으로 회전)
        currentAngle -= orbitSpeed * Time.deltaTime;
        
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        // 라디안으로 변환
        float radian = currentAngle * Mathf.Deg2Rad;
        
        // 원형 궤도 계산 (XY 평면에서의 원운동)
        float x = Mathf.Cos(radian) * orbitRadius;
        float y = Mathf.Sin(radian) * orbitRadius;
        
        // 위치 업데이트 (Z축은 유지)
        transform.position = new Vector3(
            orbitCenter.x + x,
            orbitCenter.y + y,
            orbitCenter.z
        );
    }
}
