using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour

{
    public GameObject titleScreen;
    public GameObject HUDScreen;
    public GameObject winerScreen;
    public GameObject jailScreen;
    public GameObject hospitalScreen;
    public GameObject pauseScreen;
    public GameObject settingsScreen;
    public GameObject creditsScreen;
    public bool isGameActive;
    public Image barraAnsiedad;
    public Image barraMalViaje;
    public float barraCurrentData;
    public float barraTotalData;
    public bool policeColision;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isGameActive = false;
        barraCurrentData = 3;
        barraTotalData = 3;
        policeColision = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBarra();
        if (policeColision || barraCurrentData == 0) 
        {
            GameOver();
        }
    }

    public void StartGame () 
    { 
        isGameActive = true;
        titleScreen.SetActive(false);
        HUDScreen.SetActive(true);
    }

    void GameOver()
    {
        isGameActive = false;

        HUDScreen.SetActive(false);

        if (policeColision)
        {
            hospitalScreen.SetActive(true);
        }

        if (barraCurrentData == 0)
        {
            jailScreen.SetActive(true);
        }

    }

    void UpdateBarra () {
        barraAnsiedad.fillAmount = barraCurrentData / barraTotalData;
        barraMalViaje.fillAmount = barraCurrentData / barraTotalData;
    }

    public void RestartGame () {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SettingScreen ()
    {
        titleScreen.SetActive(false);
        settingsScreen.SetActive(true);
    }

    public void ReturnFromSettings()
    {
        settingsScreen.SetActive(false);
        titleScreen.SetActive(true);
    }

    public void CreditsScreen ()
    {
        titleScreen.SetActive(false);
        creditsScreen.SetActive(true);
    }

    public void ReturnFromCredits ()
    {
        titleScreen.SetActive(true);
        creditsScreen.SetActive(false);
    }
   
}
