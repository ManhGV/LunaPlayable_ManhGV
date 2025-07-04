using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    Dictionary<System.Type, UICanvas> canvasActives = new Dictionary<System.Type, UICanvas>();
    Dictionary<System.Type, UICanvas> canvasPrefabs = new Dictionary<System.Type, UICanvas>();
    [SerializeField] List<UICanvas> canvasListInit = new List<UICanvas>();
    [SerializeField] Transform parent;
    protected override void Awake()
    {
        for (int i = 1; i < canvasListInit.Count; i++)
        {
            canvasListInit[i].gameObject.SetActive(false);
            canvasActives.Add(canvasListInit[i].GetType(), canvasListInit[i]);
            canvasPrefabs.Add(canvasListInit[i].GetType(), canvasListInit[i]);
        }
        canvasListInit[0].gameObject.SetActive(true);
    }

    //mo canvas
    public T OpenUI<T>() where T : UICanvas
    {
        T canvas = GetUI<T>();

        canvas.Setup();
        canvas.Open();

        return canvas;
    }

    //dong canvas sau time
    public void CloseUI<T>(float _time) where T : UICanvas
    {
        if (IsLoaded<T>())
        {
            canvasActives[typeof(T)].Close(_time);
        }
    }

    //dong canvas truc tiep
    public void CloseUIDirectly<T>() where T : UICanvas
    {
        if (IsLoaded<T>())
        {
            canvasActives[typeof(T)].CloseDirectly();
        }
    }

    //kiem tra canvas da duoc tao chua
    public bool IsLoaded<T>() where T : UICanvas
    {
        return canvasActives.ContainsKey(typeof(T)) && canvasActives[typeof(T)] != null;
    }

    //kiem tra canvas da duoc active chua
    public bool IsOpened<T>() where T : UICanvas
    {
        return IsLoaded<T>() && canvasActives[typeof(T)].gameObject.activeSelf;
    }

    //lay canvas active
    public T GetUI<T>() where T : UICanvas
    {
        if (!IsLoaded<T>())
        {
            T prefab = GetUIPrefab<T>();
            T canvas = Instantiate(prefab, parent);

            canvasActives[typeof(T)] = canvas;
        }

        return canvasActives[typeof(T)] as T;
    }

    //get prefab
    private T GetUIPrefab<T>() where T : UICanvas
    {
        return canvasPrefabs[typeof(T)] as T;
    }

    //dong tat ca
    public void CloseAll()
    {
        foreach (var canvas in canvasActives)
        {
            if (canvas.Value != null && canvas.Value.gameObject.activeSelf)
            {
                canvas.Value.Close(0);
            }
        }
    }
}
