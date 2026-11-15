using UnityEngine;
using UnityEngine.UI; 

public class VolumeController : MonoBehaviour
{
    public Slider backgroundVolumeSlider;

    void Start()
    {
        // 1. Cargamos el volumen guardado (por defecto será 1.0, que es el 100%)
        float backgroundVolume = PlayerPrefs.GetFloat("VolumenMaster", 1f);
        
        // 2. Ajustamos el Slider y el audio maestro a ese valor inicial
        backgroundVolumeSlider.value = backgroundVolume;
        AudioListener.volume = backgroundVolume;

        // 3. Conectamos el Slider mediante código para evitar errores en el Inspector
        backgroundVolumeSlider.onValueChanged.AddListener(UpdateVolume);
    }

    // Esta función se activa automáticamente cada vez que el usuario arrastra el Handle
    public void UpdateVolume(float value)
    {
        AudioListener.volume = value; // El volumen va de 0.0 (silencio) a 1.0 (máximo)
        PlayerPrefs.SetFloat("VolumenMaster", value); // Guarda la preferencia para el siguiente nivel
    }
}
