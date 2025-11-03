using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip[] soundEffects;
    public AudioClip[] bgmClips;

    void Awake()
    {
        // 싱글톤 패턴 적용
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
        }
        else
        {
            Destroy(gameObject); // 중복 제거
            return;
        }
    }

    // 배경음악 재생
    public void PlayBGM(int index, bool loop = true)
    {
        if (bgmSource == null || index >= bgmClips.Length) return;

        bgmSource.clip = bgmClips[index];
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    // 효과음 재생
    public void PlaySFX(int index, float volume = 1f)
    {
        if (sfxSource == null || index >= soundEffects.Length) return;
        sfxSource.PlayOneShot(soundEffects[index], volume);
    }

    // 모든 사운드 정지
    public void StopAll()
    {
        bgmSource.Stop();
        sfxSource.Stop();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 이름 또는 빌드 인덱스별로 BGM 인덱스 지정
        switch (scene.name)
        {
            case "MainMenu":      // 메인화면
                PlayBGM(0);
                break;
            case "InGame":      // 인게임
                PlayBGM(1);
                break;
            case "Stage":    // 스테이지
                PlayBGM(2);
                break;
            default:
                // 기본 BGM(없으면 미실행)
                // PlayBGM(0);
                break;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


}