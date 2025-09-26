using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class AudioSettingsManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider BGMSlider;   // Min = 0, Max = 1
    public Slider SFXSlider;   // Min = 0, Max = 1

    public TMP_Text BGMValueText;
    public TMP_Text SFXValueText;

    private const string BGM_PARAM = "BGMVolume";
    private const string SFX_PARAM = "SFXVolume";

    void Start()
    {
        // 슬라이더 이벤트 연결
        BGMSlider.onValueChanged.AddListener(SetBGMVolume);
        SFXSlider.onValueChanged.AddListener(SetSFXVolume);

        // 저장된 값 불러오기 (없으면 0.5)
        float savedBGM = PlayerPrefs.GetFloat(BGM_PARAM, 0.5f);
        float savedSFX = PlayerPrefs.GetFloat(SFX_PARAM, 0.5f);

        BGMSlider.value = savedBGM;
        SFXSlider.value = savedSFX;

        SetBGMVolume(savedBGM);
        SetSFXVolume(savedSFX);
    }

    public void SetBGMVolume(float value)
    {
        SetVolume(BGM_PARAM, value);
        UpdateText(BGMValueText, value);
        PlayerPrefs.SetFloat(BGM_PARAM, value);
    }

    public void SetSFXVolume(float value)
    {
        SetVolume(SFX_PARAM, value);
        UpdateText(SFXValueText, value);
        PlayerPrefs.SetFloat(SFX_PARAM, value);
    }

    private void SetVolume(string parameterName, float sliderValue)
    {
        // 0~1 → -80 ~ 0dB 변환
        float normalized = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        float dB = Mathf.Log10(normalized) * 20f;
        audioMixer.SetFloat(parameterName, dB);
    }

    private void UpdateText(TMP_Text text, float sliderValue)
    {
        // 0~1 값을 0~100%로 변환
        text.text = $"{sliderValue * 100f:F0}%";
    }

    public void ResetVolume()
    {
        BGMSlider.value = 0.5f;
        SFXSlider.value = 0.5f;
    }

    public void MuteAll()
    {
        BGMSlider.value = 0f;
        SFXSlider.value = 0f;
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}