using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtomSyncLink : MonoBehaviour
{
    public Button DownloadBtn;
    // Gán sự kiện cho nút bấm
    private void Start()
    {
        DownloadBtn.onClick.AddListener(OnCickButton);

    }
    
    private void OnDisable()
    {
        DownloadBtn.onClick.RemoveListener(OnCickButton);
    }

    // Phương thức để mở URL
    public void OnCickButton()
    {
        ButtonCTA();
    }
    
    public void ButtonCTA()
    {
        Luna.Unity.Playable.InstallFullGame();
        Debug.Log(nameof(ButtonCTA));
    }
}
