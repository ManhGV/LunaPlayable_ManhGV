#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathmanagerEditorVer2 : EditorWindow
{
    private PathManager pathManager;
    private GameObject pathWayPoint;
    
    [MenuItem("Tools/Path Manager Tool Ver2")]
    public static void ShowWindow()
    {
        GetWindow<PathManagerTool>("Path Manager Tool Ver2");
    }
    
    private void OnEnable()
    {
        pathManager = FindObjectOfType<PathManager>();
    }
}
#endif