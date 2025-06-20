#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FindMissingScripts : EditorWindow
{
    private List<GameObject> objectsWithMissingScripts = new List<GameObject>();
    private Vector2 scrollPos;

    private GameObject parentObject; // Parent object to search in
    private bool findActiveObjects = true; // Toggle for active/inactive objects

    private List<Component> componentsInScene = new List<Component>();
    private Type selectedComponentType; // Selected component type
    private string[] componentTypeNames; // Names of all component types
    private int selectedTypeIndex = 0; // Index of selected type

    [MenuItem("Tools/Find Missing Scripts In Scene")]
    public static void ShowWindow()
    {
        GetWindow(typeof(FindMissingScripts));
    }

    private void OnEnable()
    {
        EditorApplication.update += UpdateMissingScripts;

        // Populate component type names
        var allTypes = typeof(Component).Assembly.GetTypes();
        var componentTypes = new List<Type>();
        foreach (var type in allTypes)
        {
            if (type.IsSubclassOf(typeof(Component)) && !type.IsAbstract)
            {
                componentTypes.Add(type);
            }
        }

        componentTypeNames = new string[componentTypes.Count];
        for (int i = 0; i < componentTypes.Count; i++)
        {
            componentTypeNames[i] = componentTypes[i].Name;
        }
    }

    private void OnDisable()
    {
        EditorApplication.update -= UpdateMissingScripts;
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Find Missing Scripts in Scene"))
        {
            FindInCurrentScene();
        }

        GUILayout.Space(1);
        GUILayout.Label("Objects with Missing Scripts:", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));

        if (objectsWithMissingScripts.Count > 0)
        {
            foreach (GameObject go in objectsWithMissingScripts)
            {
                if (GUILayout.Button(go.name, GUILayout.ExpandWidth(true)))
                {
                    Selection.activeGameObject = go;
                    EditorGUIUtility.PingObject(go);
                }
            }
        }
        else
        {
            GUILayout.Label("No objects with missing scripts found.");
        }

        EditorGUILayout.EndScrollView();
        GUILayout.Space(1);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if (GUILayout.Button("Select All Objects with Missing Scripts"))
        {
            SelectObjectsWithMissingScripts();
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        Color originalColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.red;

        if (GUILayout.Button("Remove Missing Scripts from Selected Objects"))
        {
            RemoveMissingScriptsFromSelectedObjects();
            FindInCurrentScene();
        }

        GUI.backgroundColor = originalColor;

        GUILayout.Space(10);
        GUILayout.Label("Find Active/Inactive Child Objects", EditorStyles.boldLabel);

        parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(GameObject), true);
        findActiveObjects = EditorGUILayout.Toggle("Find Active Objects", findActiveObjects);

        if (GUILayout.Button("Find Child Objects"))
        {
            FindChildObjects();
        }

        GUILayout.Space(10);
        GUILayout.Label("Find Components in Scene", EditorStyles.boldLabel);

        selectedTypeIndex = EditorGUILayout.Popup("Component Type", selectedTypeIndex, componentTypeNames);
        selectedComponentType = Type.GetType(componentTypeNames[selectedTypeIndex]);

        if (GUILayout.Button("Find All Components in Scene"))
        {
            FindComponentsInScene();
        }

        GUILayout.Space(10);
        GUILayout.Label("Components Found:", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));

        if (componentsInScene.Count > 0)
        {
            foreach (var component in componentsInScene)
            {
                if (GUILayout.Button(component.gameObject.name, GUILayout.ExpandWidth(true)))
                {
                    Selection.activeGameObject = component.gameObject;
                    EditorGUIUtility.PingObject(component.gameObject);
                }
            }
        }
        else
        {
            GUILayout.Label("No components found.");
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (GUILayout.Button("Select All Found Components"))
        {
            SelectFoundComponents();
        }

        GUI.backgroundColor = Color.red;

        if (GUILayout.Button("Delete All Found Components"))
        {
            DeleteFoundComponents();
        }

        GUI.backgroundColor = originalColor;
    }

    private void UpdateMissingScripts()
    {
        objectsWithMissingScripts.RemoveAll(c => c == null);
        componentsInScene.RemoveAll(c => c == null);
    }

    private void FindInCurrentScene()
    {
        objectsWithMissingScripts.Clear();
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (!EditorUtility.IsPersistent(go.transform.root.gameObject) && go.hideFlags == HideFlags.None)
            {
                UnityEngine.Component[] components = go.GetComponents<UnityEngine.Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i] == null)
                    {
                        if (!objectsWithMissingScripts.Contains(go))
                        {
                            objectsWithMissingScripts.Add(go);
                        }
                        break;
                    }
                }
            }
        }
    }

    private void SelectObjectsWithMissingScripts()
    {
        if (objectsWithMissingScripts.Count > 0)
        {
            Selection.objects = objectsWithMissingScripts.ToArray();
        }
    }

    private void RemoveMissingScriptsFromSelectedObjects()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            return;
        }

        foreach (GameObject go in selectedObjects)
        {
            var components = go.GetComponents<UnityEngine.Component>();
            for (int i = components.Length - 1; i >= 0; i--)
            {
                if (components[i] == null)
                {
                    UnityEditor.Undo.RegisterCompleteObjectUndo(go, "Remove Missing Scripts");
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                }
            }
        }
    }

    private void FindChildObjects()
    {
        if (parentObject == null)
        {
            Debug.LogWarning("Please assign a parent object.");
            return;
        }

        List<GameObject> childObjects = new List<GameObject>();
        foreach (Transform child in parentObject.transform)
        {
            if (child.gameObject.activeSelf == findActiveObjects)
            {
                childObjects.Add(child.gameObject);
            }
        }

        if (childObjects.Count > 0)
        {
            Selection.objects = childObjects.ToArray();
            Debug.Log($"Found {childObjects.Count} child objects that are {(findActiveObjects ? "active" : "inactive")}.");
        }
        else
        {
            Debug.Log($"No child objects found that are {(findActiveObjects ? "active" : "inactive")}.");
        }
    }

    private void FindComponentsInScene()
    {
        componentsInScene.Clear();
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (!EditorUtility.IsPersistent(go.transform.root.gameObject) && go.hideFlags == HideFlags.None)
            {
                var colliders = go.GetComponents<Collider>();
                componentsInScene.AddRange(colliders);
            }
        }

        Debug.Log($"Found {componentsInScene.Count} Collider components in the scene.");
    
    }

    private void SelectFoundComponents()
    {
        if (componentsInScene.Count > 0)
        {
            Selection.objects = componentsInScene.ConvertAll(c => c.gameObject).ToArray();
        }
    }

    private void DeleteFoundComponents()
    {
        foreach (var component in componentsInScene)
        {
            Undo.DestroyObjectImmediate(component);
        }

        componentsInScene.Clear();
        Debug.Log($"Deleted all components of type {selectedComponentType.Name}.");
    }
}

#endif