using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    
    public class BoxColliderMeshTool : EditorWindow
    {
        public List<GameObject> objectsToProcess = new List<GameObject>();
    
        [MenuItem("Tools/Box Collider Mesh Tool")]
        public static void ShowWindow()
        {
            GetWindow<BoxColliderMeshTool>("Box Collider Mesh Tool");
        }
    
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Add Mesh Filter + Mesh Renderer (Cube) matching BoxCollider size", EditorStyles.boldLabel);
            SerializedObject so = new SerializedObject(this);
            SerializedProperty listProperty = so.FindProperty("objectsToProcess");
    
            EditorGUILayout.PropertyField(listProperty, true);
            so.ApplyModifiedProperties();
    
            if (GUILayout.Button("Apply to All"))
            {
                ApplyMeshToBoxCollider();
            }
        }
    
        private void ApplyMeshToBoxCollider()
        {
            foreach (GameObject obj in objectsToProcess)
            {
                if (obj == null) continue;
        
                Undo.RegisterFullObjectHierarchyUndo(obj, "Add Mesh Components");
        
                // Add MeshFilter if missing
                MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                if (meshFilter == null)
                    meshFilter = obj.AddComponent<MeshFilter>();
        
                // Assign Unity's default cube mesh
                GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Mesh cubeMesh = tempCube.GetComponent<MeshFilter>().sharedMesh;
                DestroyImmediate(tempCube);
        
                meshFilter.sharedMesh = cubeMesh;
        
                // Add MeshRenderer if missing
                if (obj.GetComponent<MeshRenderer>() == null)
                    obj.AddComponent<MeshRenderer>();
        
                // Adjust MeshRenderer scale to match BoxCollider size
                if (obj.TryGetComponent<BoxCollider>(out BoxCollider boxCollider))
                {
                    Vector3 boxSize = boxCollider.size; // BoxCollider size
                    Vector3 boxCenter = boxCollider.center; // BoxCollider center
        
                    // Create a child object for the MeshRenderer
                    GameObject meshRendererObject = new GameObject("MeshRenderer");
                    meshRendererObject.transform.SetParent(obj.transform, false);
        
                    // Set the local position and scale of the MeshRenderer object
                    meshRendererObject.transform.localPosition = boxCenter;
                    meshRendererObject.transform.localScale = Vector3.Scale(boxSize, obj.transform.lossyScale);
        
                    // Add MeshFilter and MeshRenderer to the child object
                    MeshFilter childMeshFilter = meshRendererObject.AddComponent<MeshFilter>();
                    childMeshFilter.sharedMesh = cubeMesh;
        
                    MeshRenderer childMeshRenderer = meshRendererObject.AddComponent<MeshRenderer>();
                }
            }
        
            Debug.Log("✅ Mesh Filter and Renderer added with sizes matching BoxCollider.");
        }
    }