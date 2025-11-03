using UnityEngine;
using System.Collections;

public class SoundManager2 : MonoBehaviour
{
    public static SoundManager2 Instance { get; private set; }
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Sound Effects")]
    // 효과음 3개를 인스펙터에서 할당할 수 있게 배열로 선언
    public AudioClip BlueChipSound;    // 버튼 클릭 효과음
    public AudioClip JokerBlueChipSound;
    public AudioClip RedChipSound;        // 성공 효과음
    public AudioClip CardSound;          // 에러 효과음
    public AudioClip CardSound2;
    public AudioClip CoinSound;
    public AudioClip FailSound;
    public AudioClip ItemPackSound;
    public AudioClip MultipleSound;
    public AudioClip CashOutSound;
    
    private void Awake()
    {
        // 싱글톤 패턴 적용
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
    
    // 효과음 재생 메서드
    public void PlaySFX(AudioClip clip, float volume = 1.0f)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }
    
    // 각 효과음별로 편리하게 호출할 수 있는 메서드들
    public void PlayBlueChipSound() => PlaySFX(BlueChipSound);
    public void PlayJokerBlueChipSound() => PlaySFX(JokerBlueChipSound);
    public void PlayRedChipSound() => PlaySFX(RedChipSound);    
    public void PlayCardSound() => PlaySFX(CardSound);
    public void PlayCardSound2() => PlaySFX(CardSound2);
    public void PlayCoinSound() => PlaySFX(CoinSound);
    public void PlayFailSound() => PlaySFX(FailSound);
    public void PlayItemPackSound() => PlaySFX(ItemPackSound);  
    public void PlayMultipleSound() => PlaySFX(MultipleSound);
    public void PlayCashOutSound() => PlaySFX(CashOutSound);
}