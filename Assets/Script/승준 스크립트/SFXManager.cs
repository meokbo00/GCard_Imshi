using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    public AudioSource sfxSource;
    public AudioClip[] soundEffects;

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
        }
    }

    public void PlaySFX(int index, float volume = 1f)
    {
        if (sfxSource == null || index >= soundEffects.Length) return;
        sfxSource.PlayOneShot(soundEffects[index], volume);
    }
}