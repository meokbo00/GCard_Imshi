using UnityEngine;
using UnityEngine.UI;

public class BGEffect : MonoBehaviour
{
    public Image bgImage;
    
    [Header("일그러짐 설정")]
    [SerializeField] private float speed = 0.5f;          // 움직임 속도
    [SerializeField] private float intensity = 0.1f;      // 일그러짐 강도
    [SerializeField] private float noiseScale = 1.0f;     // 노이즈 스케일
    [SerializeField] private float noiseSpeed = 0.5f;     // 노이즈 변화 속도
    
    private Material material;
    private Vector2 noiseOffset;
    private float timeElapsed;

    void Start()
    {
        // 이미지의 머티리얼을 인스턴스화하여 사용
        material = new Material(bgImage.material);
        bgImage.material = material;
        
        // 초기 노이즈 오프셋 랜덤 설정
        noiseOffset = new Vector2(Random.Range(0f, 100f), Random.Range(0f, 100f));
    }

    void Update()
    {
        if (material == null) return;
        
        // 시간 업데이트
        timeElapsed += Time.deltaTime * noiseSpeed;
        
        // 노이즈 오프셋 업데이트
        noiseOffset.x += Time.deltaTime * speed;
        noiseOffset.y += Time.deltaTime * speed * 0.5f; // 대각선으로 움직이기 위해 y축 속도 다르게 설정
        
        // 머티리얼 프로퍼티 업데이트
        material.SetFloat("_NoiseScale", noiseScale);
        material.SetFloat("_Intensity", intensity);
        material.SetVector("_NoiseOffset", noiseOffset);
        material.SetFloat("_TimeValue", timeElapsed);
    }

    // 에디터에서 값이 변경될 때 호출
    private void OnValidate()
    {
        if (material != null)
        {
            material.SetFloat("_NoiseScale", noiseScale);
            material.SetFloat("_Intensity", intensity);
        }
    }
}
