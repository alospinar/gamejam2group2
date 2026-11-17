using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreenUI : MonoBehaviour
{
    [SerializeField] private string firstLevelSceneName = "Level 1";
    [SerializeField] private string startSceneName = "Pantalla Inicial";

    public void PlayAgain()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
            return;
        }

        Time.timeScale = 1f;
        if (LevelManager.Instance != null)
            LevelManager.Instance.ResetSession();
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void ReturnToMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMenu();
            return;
        }

        Time.timeScale = 1f;
        if (LevelManager.Instance != null)
            LevelManager.Instance.ResetSession();
        SceneManager.LoadScene(startSceneName);
    }
}
