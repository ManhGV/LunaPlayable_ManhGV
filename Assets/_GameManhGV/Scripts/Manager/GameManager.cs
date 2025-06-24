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
    
    [Header("Snake Camera")]
    [SerializeField] private Transform shakeCam; // Biến để tham chiếu đến MainCamera
    [SerializeField] private float shakeCamMin;
    [SerializeField] private float shakeCamMax;
    
    [Header("End Game")] 
    public bool endGame;

    private void Start()
    {
        LunaLogStart();
        Time.timeScale = slowMotionTimeScale;
    }

    #region Pause and Resume
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
    #endregion
    
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
        SpawnBotManager.Instance.DespawnAllBot(2f);
        UIManager.Instance.GetUI<Canvas_GamePlay>().OpendEndGame(_isWin);
        soundCombatBoss.SetActive(false);
        soundBG.SetActive(false);
        endGame = true;
        gameState = GameConstants.GameState.EndGame;
        LunaEndGame();
    }

    public void ActiveSoundCombat()
    {
        soundCombatBoss.SetActive(true);
        soundBG.SetActive(false);
    }

    #region Slomotion
    public void SlomotionTimeScale()
    {
        Time.timeScale = slowMotionTimeScale;
    }
    
    public void DontSlomotionTimeScale()
    {
        Time.timeScale = 1f;
    }
    #endregion

    #region Luna Unity Life Cycle

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
    
    #endregion
    
    public void SnakeCamera(float duration, float magnitude)
    {
        if (shakeCam == null)
        {
            Debug.LogError("Shake camera transform is not assigned!");
            return;
        }
        
        StartCoroutine(ShakeCamera(duration, magnitude));
    }
    
    // Thêm hàm rung lắc camera
    protected IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Quaternion originalRot = shakeCam.localRotation;
        float elapsed = 0.0f;
    
        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(shakeCamMin, shakeCamMax) * magnitude;
            float y = UnityEngine.Random.Range(shakeCamMin, shakeCamMax) * magnitude;
    
            shakeCam.localRotation = originalRot * Quaternion.Euler(x, y, 0);
    
            elapsed += Time.deltaTime;
    
            yield return null;
        }
    
        shakeCam.localRotation = originalRot;
        EventManager.Invoke(EventName.OnCheckShakeCam, shakeCam.localEulerAngles);
    }
}