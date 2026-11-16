using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SoundManager : MonoBehaviour
{

    //UI:
    public AudioMixer mainMixer;
    public Slider bgmSlider;
    public Slider sfxSlider;

    //Sountrack:

    public AudioSource menuMusic;
    public AudioSource soberMusic;
    public AudioSource trippedMusic;

    //Verificar si trippedMusic ha empezado antes.
    public bool hasTrippedMusicStarted = false;

    private GameManager gameManager;

    void Start()
    {
        InicializeVolume();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void InicializeVolume() 
    {
        float savedBGM = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);

        bgmSlider.value = savedBGM;
        sfxSlider.value = savedSFX;

        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetBGMVolume(float value)
    {
        mainMixer.SetFloat("BGMVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void PlaySoberMusic()
    {
        menuMusic.Stop();
        soberMusic.Play();
    }

    public void SwitchToTrippedWorld()
    {
        soberMusic.mute = true;
        if (!hasTrippedMusicStarted)
        {
            trippedMusic.Play();
            hasTrippedMusicStarted = true;
        }
        trippedMusic.mute = false;
    }

    public void SwitchToSoberWorld()
    {
        trippedMusic.mute = true;
        soberMusic.mute = false;
    }
    
}
