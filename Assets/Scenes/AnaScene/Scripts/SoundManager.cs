using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        StopAllMusic();
    }

    void Start()
    {
        InicializeVolume();
        gameManager = GameManager.Instance;
    }

    private void InicializeVolume() 
    {
        float savedBGM = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);

        ApplyBGMVolume(savedBGM);
        ApplySFXVolume(savedSFX);
        BindSliders(bgmSlider, sfxSlider);
    }

    public void BindSliders(Slider bgm, Slider sfx)
    {
        float savedBGM = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (bgmSlider != null && bgmSlider != bgm)
            bgmSlider.onValueChanged.RemoveListener(SetBGMVolume);
        if (sfxSlider != null && sfxSlider != sfx)
            sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);

        bgmSlider = bgm;
        sfxSlider = sfx;

        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.RemoveListener(SetBGMVolume);
            bgmSlider.SetValueWithoutNotify(savedBGM);
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
            sfxSlider.SetValueWithoutNotify(savedSFX);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    public void SetBGMVolume(float value)
    {
        ApplyBGMVolume(value);
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        ApplySFXVolume(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    private void ApplyBGMVolume(float value)
    {
        if (mainMixer != null)
            mainMixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f);
    }

    private void ApplySFXVolume(float value)
    {
        if (mainMixer != null)
            mainMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f);
    }

    public void PlayMenuMusic()
    {
        PlayOnlyMusic(menuMusic);
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
