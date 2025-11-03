using UnityEngine;
using DG.Tweening;

public class UIRotateShake : MonoBehaviour
{
    [SerializeField] float rotateAmount = 4f;   // 최대 회전 각도 (±rotateAmount)
    [SerializeField] float rotateDuration = 0.5f; // 한쪽으로 기울어지는 시간

    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // 먼저 시작 각도를 -rotateAmount로 맞춤
        rectTransform.localRotation = Quaternion.Euler(0, 0, -rotateAmount);

        // -rotateAmount → +rotateAmount 반복
        rectTransform.DOLocalRotate(
            new Vector3(0, 0, rotateAmount),
            rotateDuration,
            RotateMode.Fast
        )
        .SetEase(Ease.InOutSine)
        .SetLoops(-1, LoopType.Yoyo); // 무한 반복
    }
}