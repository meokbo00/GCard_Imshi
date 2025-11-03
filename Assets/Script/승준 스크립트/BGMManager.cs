using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    public AudioSource bgmSource;
    public AudioClip[] bgmClips;

    [System.Serializable]
    public struct SceneBGM
    {
        public string sceneName;
        public int bgmIndex;
    }

    public SceneBGM[] sceneBGMs;

    public bool useFade = false;
    [Range(0.5f, 5f)] public float fadeTime = 1.5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var sb in sceneBGMs)
        {
            if (sb.sceneName == scene.name)
            {
                if (useFade)
                    StartCoroutine(FadeToBGM(sb.bgmIndex));
                else
                    PlayBGM(sb.bgmIndex);
                return;
            }
        }
        StopBGM();
    }

    public void PlayBGM(int index, bool loop = true)
    {
        if (bgmSource == null || index >= bgmClips.Length) return;

        if (bgmSource.clip == bgmClips[index] && bgmSource.isPlaying) return;

        bgmSource.clip = bgmClips[index];
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource == null) return;
        bgmSource.Stop();
    }

    IEnumerator FadeToBGM(int newIndex)
    {
        if (bgmSource == null) yield break;

        float startVolume = bgmSource.volume;

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeTime);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.volume = startVolume;
        PlayBGM(newIndex);
    }
}