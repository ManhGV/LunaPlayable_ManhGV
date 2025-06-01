#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class PathmanagerEditorVer2 : EditorWindow
{
    private static PathManager pathManager;

    private List<ListPoin> listPoint = new List<ListPoin>();
    private WayPoint wayPoint = new WayPoint();

    private List<int> indexPointDuocRandomTheoThuTu = new List<int>();

    int countOnePointZomMove;
    int countPointRandom;
    private Vector2 scrollPos;

    private GUIStyle _centeredBoldStyle;
    [MenuItem("Tools/Path Manager Tool Ver2")]
    public static void ShowWindow()
    {
        GetWindow<PathmanagerEditorVer2>("Path Manager Tool Ver2");
    }


    private void OnEnable()
    {

    }

    private void OnGUI()
    {
        _centeredBoldStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter
        };

        GUILayout.Label("Path Manager Tool Ver 2", _centeredBoldStyle);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        pathManager = (PathManager)EditorGUILayout.ObjectField(pathManager, typeof(PathManager), true, GUILayout.Width(400));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        #region ------------------- List --------------------------

        EditorGUILayout.BeginHorizontal("box");
        for (int i = 0; i < listPoint.Count; i++)
        {
            ListPoin listPoin = listPoint[i];

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"ListPoin {i + 1}", _centeredBoldStyle);

            if (listPoin.transforms == null)
                listPoin.transforms = new List<Transform>();

            for (int j = 0; j < listPoin.transforms.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();
                listPoin.transforms[j] = (Transform)EditorGUILayout.ObjectField($"Point {j + 1}", listPoin.transforms[j], typeof(Transform), true);
                if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                    listPoin.transforms.Remove(listPoin.transforms[j]);
                EditorGUILayout.EndHorizontal();
            }

            DrawDropArea("Drop Tranform Random", (objs) => AddTranform(objs, i), Color.cyan, _centeredBoldStyle);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Remove List Index"))
                listPoint.RemoveAt(i);

            listPoint[i].CanRandom = EditorGUILayout.Toggle("Can Random", listPoint[i].CanRandom,GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndHorizontal();

        #endregion
        EditorGUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add ListPoint"))
            listPoint.Add(new ListPoin());
        
        if (GUILayout.Button("Clear ListPoint"))
            listPoint.Clear();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(20);



        EditorGUILayout.LabelField("List point random done", _centeredBoldStyle);
        countPointRandom = EditorGUILayout.IntField("Count Point Random", countPointRandom, GUILayout.Width(250));
        EditorGUILayout.Space(20);

        int index = 0;
        EditorGUILayout.BeginVertical();
        if (countOnePointZomMove != 0)
            for (int i = 0; i < wayPoint.WayPoints.Count / countOnePointZomMove; i++)
            {
                EditorGUILayout.BeginHorizontal("Box");
                EditorGUILayout.LabelField($"Point {i+1}", _centeredBoldStyle,GUILayout.Width(60));
                for (int j = 0; j < countOnePointZomMove; j++)
                {
                    if (index < wayPoint.WayPoints.Count)
                        wayPoint.WayPoints[index] = (Transform)EditorGUILayout.ObjectField(wayPoint.WayPoints[index], typeof(Transform), true);
                    index++;
                }
                EditorGUILayout.EndHorizontal();
            }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("RandomPoint"))
        {
            index = 0;
            RandomPoint();
        }

        if (GUILayout.Button("Add Point To ParthManager"))
            AddPoinToParthManager();
        EditorGUILayout.EndHorizontal();

    }

    private void AddPoinToParthManager()
    {
        int index = 0;
        WayPointlist _wayPointList = new WayPointlist();
        for (int i = 0; i < countPointRandom; i++)
        {
            WayPoint _wayPoint = new WayPoint();
            for (int j = 0; j < countOnePointZomMove; j++)
            {
                if (index < wayPoint.WayPoints.Count)
                    _wayPoint.WayPoints.Add(wayPoint.WayPoints[index]);
                index++;
            }
            _wayPointList._wayPointlist.Add(_wayPoint);
        }
        pathManager.Listwaypoint.Add(_wayPointList);
    }

    private void RandomPoint()
    {
        wayPoint.WayPoints.Clear();
        countOnePointZomMove = 0;
        for (int i = 0; i < listPoint.Count; i++)
            if (listPoint[i].CanRandom)
                countOnePointZomMove++;


        for (int j = 0; j < countPointRandom; j++)
            for (int i = 0; i < listPoint.Count; i++)
                if (listPoint[i].CanRandom)
                    wayPoint.WayPoints.Add(listPoint[i].transforms[UnityEngine.Random.Range(0, listPoint[i].transforms.Count)]);
                else
                    continue;
    }

    private void AddTranform(object[] objs, int _indexList)
    {
        foreach (var obj in objs)
            if (obj is GameObject go)
                listPoint[_indexList].transforms.Add(go.transform);

        Repaint();
    }

    private void DrawDropArea(string label, Action<System.Object[]> onDrop, Color bgColor, GUIStyle _GUIStyle)
    {
        Color prevColor = GUI.color;
        GUI.color = bgColor;

        GUILayout.BeginVertical("box", GUILayout.Height(50));
        GUI.color = prevColor;

        GUILayout.FlexibleSpace();
        GUILayout.Label(label, _GUIStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();

        Rect dropArea = GUILayoutUtility.GetLastRect();
        Event evt = Event.current;

        if ((evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform) && dropArea.Contains(evt.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (evt.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                onDrop?.Invoke(DragAndDrop.objectReferences);
                Event.current.Use();
            }
        }
    }
}

public class ListPoin
{
    public List<Transform> transforms;
    public bool CanRandom;
}
#endif