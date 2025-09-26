using UnityEngine;

public class AudioTestController : MonoBehaviour
{
    public AudioSource BGMsource;
    public AudioSource SFXsource;
    public AudioClip testSFXClip; // 테스트용 효과음

    void Start()
    {
        // 씬 시작 시 BGM 자동 재생
        if (BGMsource != null)
            BGMsource.Play();
    }

    // SFX 재생 / 정지
    public void PlaySFX()
    {
        if (SFXsource != null && testSFXClip != null)
            SFXsource.PlayOneShot(testSFXClip, SFXsource.volume);
    }
}