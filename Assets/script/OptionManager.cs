// OptionManager.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class OptionManager : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider seSlider;
    public GameObject optionCanvas;
    public GameObject mainMenuCanvas;
    public GameObject initialButtons;
    public GameObject secondaryButtons;

    public AudioMixer audioMixer;
    public AudioSource sePreviewSource;
    public AudioClip sePreviewClip;

    private float sePreviewCooldown = 0f;
    private bool hasStarted = false;

    private const string BGM_VOLUME_KEY = "BGM_VOLUME";
    private const string SE_VOLUME_KEY = "SE_VOLUME";

    void Start()
    {
        // �ۑ�����Ă���l��ǂݍ��ށB������΃f�t�H���g�l 0.5f
        float bgmValue = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 0.5f);
        float seValue = PlayerPrefs.GetFloat(SE_VOLUME_KEY, 0.5f);

        // �X���C�_�[�̒l�ɔ��f�iLog�ϊ��O�j
        bgmSlider.value = bgmValue;
        seSlider.value = seValue;

        // AudioMixer �ɂ����f�iLog�ϊ���j
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Clamp(bgmValue, 0.0001f, 1f)) * 20f);
        audioMixer.SetFloat("SEVolume", Mathf.Log10(Mathf.Clamp(seValue, 0.0001f, 1f)) * 20f);

        hasStarted = true;

        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        seSlider.onValueChanged.AddListener(SetSEVolume);
    }


    public void SetBGMVolume(float value)
    {
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, value);
        PlayerPrefs.Save();
    }

    public void SetSEVolume(float value)
    {
        audioMixer.SetFloat("SEVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);

        PlayerPrefs.SetFloat(SE_VOLUME_KEY, value);
        PlayerPrefs.Save();

        // �v���r���[�Đ��i0.2�b�����Ɂj
        if (Time.time >= sePreviewCooldown && sePreviewSource != null && sePreviewClip != null)
        {
            sePreviewSource.PlayOneShot(sePreviewClip);
            sePreviewCooldown = Time.time + 0.2f;
        }
    }
}
