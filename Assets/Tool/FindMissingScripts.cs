#if UNITY_EDITOR
        
        using System.Collections.Generic;
        using UnityEngine;
        using UnityEditor;
        
        public class FindMissingScripts : EditorWindow
        {
            private List<GameObject> objectsWithMissingScripts = new List<GameObject>();
            private Vector2 scrollPos;
        
            private GameObject parentObject; // Parent object to search in
            private bool findActiveObjects = true; // Toggle for active/inactive objects
        
            private Component selectedComponent; // Selected component to find or remove
        
            [MenuItem("Tools/Find Missing Scripts In Scene")]
            public static void ShowWindow()
            {
                GetWindow(typeof(FindMissingScripts));
            }
        
            private void OnEnable()
            {
                EditorApplication.update += UpdateMissingScripts;
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
                GUILayout.Label("Find/Remove Specific Component", EditorStyles.boldLabel);
        
                selectedComponent = (Component)EditorGUILayout.ObjectField("Component", selectedComponent, typeof(Component), true);
        
                if (GUILayout.Button("Find Component"))
                {
                    FindComponentInParent();
                }
        
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Remove Component"))
                {
                    RemoveComponentFromParent();
                }
                GUI.backgroundColor = originalColor;
            }
        
            private void UpdateMissingScripts()
            {
                for (int i = objectsWithMissingScripts.Count - 1; i >= 0; i--)
                {
                    if (objectsWithMissingScripts[i] == null)
                    {
                        objectsWithMissingScripts.RemoveAt(i);
                    }
                }
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
        
            private void FindComponentInParent()
            {
                if (parentObject == null || selectedComponent == null)
                {
                    Debug.LogWarning("Please assign a parent object and a component.");
                    return;
                }
        
                var components = parentObject.GetComponentsInChildren(selectedComponent.GetType(), true);
                if (components.Length > 0)
                {
                    Selection.objects = components;
                    Debug.Log($"Found {components.Length} components of type {selectedComponent.GetType().Name}.");
                }
                else
                {
                    Debug.Log($"No components of type {selectedComponent.GetType().Name} found.");
                }
            }
        
            private void RemoveComponentFromParent()
            {
                if (parentObject == null || selectedComponent == null)
                {
                    Debug.LogWarning("Please assign a parent object and a component.");
                    return;
                }
        
                var components = parentObject.GetComponentsInChildren(selectedComponent.GetType(), true);
                foreach (var component in components)
                {
                    Undo.DestroyObjectImmediate(component);
                }
        
                Debug.Log($"Removed {components.Length} components of type {selectedComponent.GetType().Name}.");
            }
        }
        
        #endif