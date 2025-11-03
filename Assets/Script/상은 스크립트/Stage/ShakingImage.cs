using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ShakingImage : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeStrength = 3f;  // 진동 세기
    [SerializeField] private float shakeDuration = 0.5f; // 한 번 흔들리는 시간
    [SerializeField] private float shakeInterval = 2f;   // 흔들림 사이의 간격
    [SerializeField] private int vibrato = 10;          // 진동 수 (높을수록 더 세밀한 진동)
    [SerializeField] private float randomness = 90f;     // 진동의 무작위성 (0-180)
    [SerializeField] private bool fadeOut = true;        // 점점 약해지는 효과 사용 여부

    private Vector3 originalPosition;
    private Sequence shakeSequence;

    private void Start()
    {
        originalPosition = transform.localPosition;
        StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        while (true)
        {
            // 흔들림 효과
            shakeSequence = DOTween.Sequence();
            
            // 현재 위치에서 약간 흔들리도록 설정
            shakeSequence.Append(transform.DOShakePosition(
                duration: shakeDuration,
                strength: shakeStrength,
                vibrato: vibrato,
                randomness: randomness,
                snapping: false,
                fadeOut: fadeOut
            ));

            // 원래 위치로 부드럽게 복귀
            shakeSequence.Append(transform.DOLocalMove(originalPosition, 0.3f));

            // 다음 흔들림까지 대기
            yield return new WaitForSeconds(shakeInterval);
        }
    }

    private void OnDestroy()
    {
        // 오브젝트가 파괴될 때 트윈 정리
        shakeSequence?.Kill();
    }
}
