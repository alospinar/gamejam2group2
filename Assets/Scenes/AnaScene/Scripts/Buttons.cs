using UnityEngine;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    private Button button;
    private GameManager gameManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(StartGame);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartGame () 
    {
        Debug.Log(gameObject.name + " was clicked");
        gameManager.StartGame();


    }

}

