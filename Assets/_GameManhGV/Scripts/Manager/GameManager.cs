using System;
using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject soundBG;
    [SerializeField] private GameObject soundCombatBoss;
    [SerializeField] private GameConstants.GameState gameState;
    [SerializeField] private float slowMotionTimeScale = 0.5f;
    
    [Header("CutScene")]
    [SerializeField] CanvasGroup _canvasGameplay;
    [SerializeField] private GameObject _mainCamera;
    [SerializeField] private GameObject _cutSceneCamera;
    
    [Header("End Game")] 
    public bool endGame;
    
    [Header("Swat Move Audio Zomcam")]
    private Coroutine _soundBotMoveCoroutine;
    [SerializeField] private AudioClip _swatmoveclip;
    private void Start()
    {
        LunaLogStart();
    }

    public void StartCutScene()
    {
        StartCoroutine(IESoundSwatMove(4));
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

    public void EndGame(bool _isWin)
    {
        if(gameState == GameConstants.GameState.EndGame)
            return;
        UIManager.Instance.GetUI<Canvas_GamePlay>().OpendEndGame(_isWin);
        soundCombatBoss.SetActive(false);
        soundBG.SetActive(false);
        endGame = true;
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

    private IEnumerator IESoundSwatMove(float _countPlay)
    {
            AudioManager audioManager = AudioManager.Instance;
        for (int i = 0; i < _countPlay; i++)
        {
            audioManager.PlaySound(_swatmoveclip,1);
            if( i%2 == 0)
                yield return new WaitForSeconds(.65f);
            else
                yield return new WaitForSeconds(.4f);
        }
    }
    
}