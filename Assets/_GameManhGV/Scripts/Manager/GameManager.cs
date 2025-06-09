using System;
using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject soundBG;
    [SerializeField] private GameConstants.GameState gameState;
    [SerializeField] private float slowMotionTimeScale = 0.5f;
    
    [Header("CutScene")]
    [SerializeField] CanvasGroup _canvasGameplay;
    [SerializeField] private GameObject _mainCamera;
    [SerializeField] private GameObject _cutSceneCamera;
    
    [Header("End Game")] 
    public bool endGame;

    private void Start()
    {
        LunaLogStart();
    }

    public void StartCutScene()
    {
        soundBG.SetActive(false);
        _canvasGameplay.alpha = 0;
        _mainCamera.SetActive(false);
        _cutSceneCamera.SetActive(true);
        gameState = GameConstants.GameState.CutScene;
    }
    
    public void EndCutScene()
    {
        _canvasGameplay.alpha = 1;
        _mainCamera.SetActive(true);
        _cutSceneCamera.SetActive(false);
        gameState = GameConstants.GameState.Playing;
    }
    
    public void PauseGame()
    {
        Time.timeScale = 0f;
        gameState = GameConstants.GameState.Paused;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        gameState = GameConstants.GameState.Playing;
    }

    public GameConstants.GameState GetGameState() => gameState;

    public Transform GetMainCameraTransform()
    {
        if (Camera.main != null)
        {
            return Camera.main.transform;
        }
        else
        {
            Debug.LogError("Main camera not found!");
            return null;
        }
    }

    public void EndGame()
    {
        soundBG.SetActive(false);
        endGame = true;
        UIManager.Instance.GetUI<Canvas_GamePlay>().OpendEndGame();
        gameState = GameConstants.GameState.EndGame;
        LunaEndGame();
    }
    
    public void SlomotionTimeScale()
    {
        Time.timeScale = slowMotionTimeScale;
    }
    
    public void DontSlomotionTimeScale()
    {
        Time.timeScale = 1f;
    }

    public void LunaEndGame()
    {
        Luna.Unity.LifeCycle.GameEnded();
        Luna.Unity.Analytics.LogEvent(Luna.Unity.Analytics.EventType.EndCardShown);
    }
    
    public void LunaLogStart()
    {
        Luna.Unity.LifeCycle.GameStarted();
        Luna.Unity.Analytics.LogEvent(Luna.Unity.Analytics.EventType.TutorialComplete);
        Debug.Log($"{nameof(LunaLogStart)}");
    }
}