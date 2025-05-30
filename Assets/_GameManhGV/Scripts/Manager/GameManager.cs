using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameConstants.GameState gameState;

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
}