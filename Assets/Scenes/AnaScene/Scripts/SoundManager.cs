using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

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

    //SFX
    public AudioSource agarrarTicket;
    public AudioSource arrastrarSilla;
    public AudioSource interaccion1;
    public AudioSource interaccion2;
    public AudioSource interaccion3;
    public AudioSource interaccion4;
    public AudioSource portal; // cuando cambie del mundo sobrio al viajado
    public AudioSource pasos; //debe acelerarse para que quede con el player
    public AudioSource saltoCorto; // salto por defecto
    public AudioSource saltoLargo; // salto con hongo
    public AudioSource susto; // cuando se topa con la sombra del policia o la sombra de la lámpara
    public AudioSource tragar; // cuando consume el hongo verde. 


    private GameManager gameManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            StopAllMusic();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        InicializeVolume();
        GameObject gameManagerObject = GameObject.Find("GameManager");
        if (gameManagerObject != null)
            gameManager = gameManagerObject.GetComponent<GameManager>();
    }

    private void InicializeVolume() 
    {
        float savedBGM = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (bgmSlider != null)
        {
            bgmSlider.value = savedBGM;
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = savedSFX;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
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
        PlayOnlyMusic(soberMusic);
    }

    public void SwitchToTrippedWorld()
    {
        PlayOnlyMusic(trippedMusic);
        if (trippedMusic != null)
            hasTrippedMusicStarted = true;
    }

    public void SwitchToSoberWorld()
    {
        PlayOnlyMusic(soberMusic);
    }

    private void PlayOnlyMusic(AudioSource musicToPlay)
    {
        AudioSource[] musicSources = { menuMusic, soberMusic, trippedMusic };

        foreach (AudioSource musicSource in musicSources)
        {
            if (musicSource == null || musicSource == musicToPlay)
                continue;

            musicSource.Stop();
        }

        if (musicToPlay != null && !musicToPlay.isPlaying)
            musicToPlay.Play();
    }

    private void StopAllMusic()
    {
        if (menuMusic != null)
            menuMusic.Stop();
        if (soberMusic != null)
            soberMusic.Stop();
        if (trippedMusic != null)
            trippedMusic.Stop();
    }

    public void PlayJump()
    {
        if (saltoCorto != null)
            saltoCorto.Play();
    }

    public void PlayMushroomPickup()
    {
        if (tragar != null)
            tragar.Play();
    }

    public void PlayTicketPickup()
    {
        if (agarrarTicket != null)
            agarrarTicket.Play();
    }

    public void PlayPortal()
    {
        if (portal != null)
            portal.Play();
    }

    public void PlayNpcInteraction()
    {
        AudioSource[] interactionSounds =
        {
            interaccion1,
            interaccion2,
            interaccion3,
            interaccion4
        };

        int startIndex = Random.Range(0, interactionSounds.Length);
        for (int offset = 0; offset < interactionSounds.Length; offset++)
        {
            AudioSource interactionSound =
                interactionSounds[(startIndex + offset) % interactionSounds.Length];

            if (interactionSound != null)
            {
                interactionSound.Play();
                return;
            }
        }
    }
    
}
