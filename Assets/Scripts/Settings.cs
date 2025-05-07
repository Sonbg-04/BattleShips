using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Slider soundSlider, sfxSlider;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Button soundButton;
    [SerializeField] private Image soundButtonImage;
    [SerializeField] private Sprite soundOnSprite, soundOffSprite;

    private AudioManager audioManager;
    private float lastSoundVolume;
    private bool isSoundOn = true;

    private void Start()
    {
        GameObject audioObj = GameObject.FindGameObjectWithTag("Audio");

        if (audioObj != null)
        {
            audioManager = audioObj.GetComponent<AudioManager>();
        }
        else
        {
            Debug.Log("AudioManager not found!");
        }

        if (PlayerPrefs.HasKey("SoundGame"))
        {
            LoadVolumeSettings();
            isSoundOn = (soundSlider.value > 0.0001f);
            lastSoundVolume = soundSlider.value;
            UpdateSoundButtonImage();
        }
        else
        {
            SoundSlider();
            SFXSlider();
            lastSoundVolume = 1f;
        }
    }

    public void SoundSlider()
    {
        float volume = soundSlider.value;
        if (volume <= 0.0001f)
        {
            audioMixer.SetFloat("SoundGame", -80f);
            isSoundOn = false;
        }
        else
        {
            audioMixer.SetFloat("SoundGame", Mathf.Log10(volume) * 20);
            isSoundOn = true;
            lastSoundVolume = volume; 
        }
        PlayerPrefs.SetFloat("SoundGame", volume);
        UpdateSoundButtonImage();
    }

    public void SFXSlider()
    {
        float volume = sfxSlider.value;
        if (volume <= 0.0001f)
            audioMixer.SetFloat("SFXGame", -80f);
        else
            audioMixer.SetFloat("SFXGame", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXGame", volume);
    }

    private void LoadVolumeSettings()
    {
        soundSlider.value = PlayerPrefs.GetFloat("SoundGame");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXGame");
        SoundSlider();
        SFXSlider();
    }

    public void BtnSound()
    {
        audioManager.PlaySFX();

        if (isSoundOn)
        {
            lastSoundVolume = soundSlider.value;
            soundSlider.value = 0.0001f;
        }
        else
        {
            soundSlider.value = lastSoundVolume;
        }

        SoundSlider(); 
    }

    private void UpdateSoundButtonImage()
    {
        if (soundButtonImage != null)
            soundButtonImage.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
    }
}