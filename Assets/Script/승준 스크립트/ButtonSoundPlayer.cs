using UnityEngine;

public class ButtonSoundPlayer : MonoBehaviour
{
    public AudioClip[] clips;     // 효과음 배열
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSource 자동 생성
    }

    // 버튼에서 호출
    public void Play(int index)
    {
        if (clips != null && index >= 0 && index < clips.Length)
            audioSource.PlayOneShot(clips[index]);
    }
}