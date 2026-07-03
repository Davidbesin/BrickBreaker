using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver
    }

    public GameState CurrentState;
    public GameObject Canvas;
    public GameObject Menu;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
         
        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //SetState(GameState.Menu);
    }
    public void PlayState()
    {
        SetState(GameState.Playing);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;

        switch (CurrentState)
        {
            case GameState.Menu:
                Time.timeScale = 0f;
                Canvas.SetActive(false);
                 Menu.SetActive(true);
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                Canvas.SetActive(false);
                 Menu.SetActive(false);
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                Canvas.SetActive(true);
                 Menu.SetActive(false);
                break;

            case GameState.GameOver:
                Time.timeScale = 0f;
                Canvas.SetActive(true);
                 Menu.SetActive(false);
                break;
        }

        Debug.Log($"Game State: {CurrentState}");
    }
    public void TogglePause()
    {
        if (CurrentState == GameState.Playing)
        {
            SetState(GameState.Paused);
        }
        else if (CurrentState == GameState.Paused)
        {
            SetState(GameState.Playing);
        }
    }
    public bool IsPlaying => CurrentState == GameState.Playing;
    public bool IsPaused => CurrentState == GameState.Paused;
    public bool IsGameOver => CurrentState == GameState.GameOver;
    public bool IsMenu => CurrentState == GameState.Menu;
}