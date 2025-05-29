using UnityEngine;

public static class ParameterManagers
{
    public static bool IsIngameGUI = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnActiveInGameGUI()
    {
        IsIngameGUI = true;
    }
}