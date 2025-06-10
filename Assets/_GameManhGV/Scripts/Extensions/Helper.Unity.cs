using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;


public static partial class Helper
{
    public static void PlayPreview(this AudioClip clip)
    {
        var go = new GameObject("Preview Sound");
        go.hideFlags = HideFlags.HideAndDontSave;
        var audioSource = go.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();
    }

    public static void PlayPreview(this AudioClip clip, Vector3 position)
    {
        var go = new GameObject("Preview Sound");
        go.hideFlags = HideFlags.HideAndDontSave;
        go.transform.position = position;
        var audioSource = go.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1;
        audioSource.clip = clip;
        audioSource.Play();
    }
    
    public static void PlayPreview(this AudioClip clip, Vector3 position, float volume, float pitch, float spatialBlend)
    {
        var go = new GameObject("Preview Sound");
        go.hideFlags = HideFlags.HideAndDontSave;
        go.transform.position = position;
        var audioSource = go.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.spatialBlend = spatialBlend;
        audioSource.Play();
    }

    public static Transform FindInChildren(this Transform root, string name)
    {
        foreach (Transform child in root)
        {
            if (child.name == name)
                return child;

            var resultInChild = child.FindInChildren(name);
            if (resultInChild != null)
                return resultInChild;
        }

        return null;
    }

    public static Transform FindAChildByNameContains(this Transform root, string name)
    {
        foreach (Transform child in root)
        {
            if (child.name.Contains(name))
                return child;

            var resultInChild = child.FindAChildByNameContains(name);
            if (resultInChild != null)
                return resultInChild;
        }
        return null;
    }
    
    public static Transform FindChildByNameContainsIgnoreCase(this Transform root, string name)
    {
        foreach (Transform child in root)
        {
            if (child.name.IndexOf(name, System.StringComparison.OrdinalIgnoreCase) >= 0)
                return child;

            var resultInChild = child.FindChildByNameContainsIgnoreCase(name);
            if (resultInChild != null)
                return resultInChild;
        }
        return null;
    }
    
    public static Transform FindChildByNameStartWithIgnoreCase(this Transform root, string name)
    {
        foreach (Transform child in root)
        {
            if (child.name.StartsWith(name, System.StringComparison.OrdinalIgnoreCase))
                return child;

            var resultInChild = child.FindChildByNameStartWithIgnoreCase(name);
            if (resultInChild != null)
                return resultInChild;
        }
        return null;
    }

    public static T FindComponentInAllScenes<T>()
    {
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            var find = FindComponentInScene<T>(scene);
            if (find != null)
                return find;
        }

        return default;
    }

    public static T FindComponentInScene<T>(Scene scene)
    {
        var rootGos = scene.GetRootGameObjects();
        foreach (var go in rootGos)
        {
            var find = go.GetComponentInChildren<T>();
            if (find != null)
                return find;
        }

        return default;
    }
    
    public static bool HasComponent<T>(this Component main) where T : Component
    {
        return main.GetComponent<T>() != null;
    }

    public static Color32 To32Format(this Color color)
    {
        var r = (byte)(color.r * 256);
        var g = (byte)(color.g * 256);
        var b = (byte)(color.b * 256);
        return new Color32(r, g, b, 1);
    }

#if UNITY_EDITOR
    public static string GetFullPath(this Object asset)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), AssetDatabase.GetAssetPath(asset));
        path = path.Replace("/", "\\");
        return path;
    }
#endif

    public static bool ContainNull<T>(this IEnumerable<T> collection) where T : Object
    {
        return collection.Any(item => item == null);
    }

    public static void ClearNull<T>(ref T[] collection) where T : Object
    {
        var nullCount = collection.Count(t => t == null);
        var result = new T[collection.Length - nullCount];
        var i = 0;
        foreach (var item in collection)
            if (item != null)
            {
                result[i] = item;
                i++;
            }

        collection = result;
    }

    #region MyRegion

#if UNITY_EDITOR
    public static bool CheckStaticFlag(this GameObject gameObject, StaticEditorFlags staticEditorFlags)
    {
        var staticFlags = GameObjectUtility.GetStaticEditorFlags(gameObject);
        return (staticFlags & staticEditorFlags) > 0;
    }

    public static void SetStaticFlag(this GameObject gameObject, StaticEditorFlags staticEditorFlags)
    {
        var staticFlags = GameObjectUtility.GetStaticEditorFlags(gameObject);
        staticFlags |= staticEditorFlags;
        GameObjectUtility.SetStaticEditorFlags(gameObject, staticFlags);
    }

    public static void ClearStaticFlag(this GameObject gameObject, StaticEditorFlags staticEditorFlags)
    {
        var staticFlags = GameObjectUtility.GetStaticEditorFlags(gameObject);
        staticFlags &= ~staticEditorFlags;
        GameObjectUtility.SetStaticEditorFlags(gameObject, staticFlags);
    }
#endif

    #endregion

    #region MATERIAL

    // ReSharper disable Unity.PerformanceAnalysis
    public static void SafelySetColor(this Material material, int propertyId, Color value)
    {
        if (material.HasProperty(propertyId))
            material.SetColor(propertyId, value);
#if UNITY_EDITOR
        Debug.LogError($"Material {material} don't have property with id {propertyId}");
#endif
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static void SafelySetFloat(this Material material, int propertyId, float value)
    {
        if (material.HasProperty(propertyId))
            material.SetFloat(propertyId, value);
#if UNITY_EDITOR
        Debug.LogError($"Material {material} don't have property with id {propertyId}");
#endif
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static void SafelySetTexture(this Material material, int propertyId, Texture value)
    {
        if (material.HasProperty(propertyId))
            material.SetTexture(propertyId, value);
#if UNITY_EDITOR
        Debug.LogError($"Material {material} don't have property with id {propertyId}");
#endif
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static void SafelySetTextureScale(this Material material, int propertyId, Vector2 value)
    {
        if (material.HasProperty(propertyId))
            material.SetTextureScale(propertyId, value);
#if UNITY_EDITOR
        Debug.LogError($"Material {material} don't have property with id {propertyId}");
#endif
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static void SafelySetTextureOffset(this Material material, int propertyId, Vector2 value)
    {
        if (material.HasProperty(propertyId))
            material.SetTextureOffset(propertyId, value);
#if UNITY_EDITOR
        Debug.LogError($"Material {material} don't have property with id {propertyId}");
#endif
    }

    #endregion

    #region TRANSFORM
    
    public static bool Pass(this Transform target, Vector3 point)
    {
        var vector = point - target.position;
        return Vector3.Dot(vector, target.forward) <= 0;
    }

    public static void LookAt(this Component component, Transform target)
    {
        var transform = component.transform;
        var distance  = target.position - transform.position;
        transform.rotation = Quaternion.LookRotation(distance);
    }

    public static void SetPositionAndRotation(this Transform target, Transform reference)
    {
        target.position = reference.position;
        target.rotation = reference.rotation;
    }

    public static Quaternion LookRotationBaseOnChild(Transform target, Vector3 childPosition, Quaternion rootOffset,
        Quaternion childOffset)
    {
        var direction = childOffset * (target.position - childPosition).normalized;
        var rotation = Quaternion.LookRotation(direction);
        rotation *= rootOffset;

        return rotation;
    }

    public static Quaternion LookRotationBaseOnChild(Quaternion rootOriginalRotation, Transform target,
        Vector3 childCurrentPosition, Quaternion childOriginalInverseRotation)
    {
        var childDistance = childOriginalInverseRotation * (target.position - childCurrentPosition);
        var rotation = Quaternion.LookRotation(childDistance);
        rotation *= rootOriginalRotation;
        return rotation;
    }

    public static void SetRotationBaseOnChild(this Transform root, Quaternion rootOriginalRotation,
        Transform target,
        Vector3 childCurrentPosition, Quaternion childOriginalInverseRotation)
    {
        var childDistance = childOriginalInverseRotation * (target.position - childCurrentPosition);
        var rotation = Quaternion.LookRotation(childDistance);
        rotation *= rootOriginalRotation;
        root.rotation = rotation;
    }

    public static void RotateSlerpBaseOnChild(this Transform root, Quaternion rootOriginalRotation,
        Transform target,
        Vector3 childCurrentPosition, Quaternion childOriginalInverseRotation, float slerpParam = 0.5f)
    {
        var childDistance = childOriginalInverseRotation * (target.position - childCurrentPosition);
        var desiredRotation = Quaternion.LookRotation(childDistance);
        desiredRotation *= rootOriginalRotation;
        root.rotation = Quaternion.Slerp(root.rotation, desiredRotation, slerpParam);
    }

    public static void RotateTowardBaseOnChild(this Transform root, Quaternion rootOriginalRotation,
        Transform target,
        Vector3 childCurrentPosition, Quaternion childOriginalInverseRotation, float rotateAmount)
    {
        var childDistance = childOriginalInverseRotation * (target.position - childCurrentPosition);
        var desiredRotation = Quaternion.LookRotation(childDistance);
        desiredRotation *= rootOriginalRotation;
        root.rotation = Quaternion.RotateTowards(root.rotation, desiredRotation, rotateAmount);
    }

    public static void ChatGPTRotateBaseOnChild(this Transform transform, Transform target, Transform child)
    {
        var direction = target.position - child.position;
        var childRotation = Quaternion.LookRotation(direction, Vector3.up);
        var parentRotation = childRotation * Quaternion.Inverse(child.localRotation);
        transform.rotation = parentRotation;
    }

    public static void RotateBaseOnChild(this Transform root, Transform child, Transform target)
    {
        var direction = target.position - child.position;
        var childRotation = Quaternion.LookRotation(direction, Vector3.up);
        var desiredRotation = childRotation * Quaternion.Inverse(child.localRotation);
        root.rotation = desiredRotation;
    }

    public static void RotateTowardBaseOnChild(this Transform root, Transform child, Transform target,
        float rotateAmount)
    {
        var direction = target.position - child.position;
        var childRotation = Quaternion.LookRotation(direction, Vector3.up);
        var desiredRotation = childRotation * Quaternion.Inverse(child.localRotation);
        root.rotation = Quaternion.RotateTowards(root.rotation, desiredRotation, rotateAmount);
    }

    public static void RotateSlerpBaseOnChild(this Transform root, Transform child, Transform target,
        float slerpParam)
    {
        var direction = target.position - child.position;
        var childRotation = Quaternion.LookRotation(direction, Vector3.up);
        var desiredRotation = childRotation * Quaternion.Inverse(child.localRotation);
        root.rotation = Quaternion.Slerp(root.rotation, desiredRotation, slerpParam);
    }

    public static void RotateToward(this Transform main, Quaternion target, float maxDegreesDelta)
    {
        main.rotation = Quaternion.RotateTowards(main.rotation, target, maxDegreesDelta);
    }

    public static void RotateLocalToward(this Transform main, Quaternion localTarget, float maxDegreesDelta)
    {
        main.localRotation = Quaternion.RotateTowards(main.localRotation, localTarget, maxDegreesDelta);
    }

    public static void RotateLocalSlerp(this Transform main, Quaternion localTarget, float factor)
    {
        main.localRotation = Quaternion.Slerp(main.localRotation, localTarget, factor);
    }

    public static void DeltaRotateToward(this Transform main, Quaternion target, float amount)
    {
        main.rotation = Quaternion.RotateTowards(main.rotation, target, amount * Time.deltaTime);
    }

    /// <summary>
    /// Rotate transform's local rotation to target with amount multiply delta time.
    /// </summary>
    /// <param name="main"></param>
    /// <param name="target"></param>
    /// <param name="amount"></param>
    public static void DeltaRotateLocalToward(this Transform main, Quaternion target, float amount)
    {
        main.localRotation = Quaternion.RotateTowards(main.localRotation, target, amount * Time.deltaTime);
    }

    public static Vector3 GetRandomPointDistance(this Transform center, float distance)
    {
        var randomDirection = Random.onUnitSphere;
        return center.position + randomDirection * distance;
    }

    /// <summary>
    /// Calculate time to rotate from current rotation to look-at-target rotation
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="target"></param>
    /// <param name="rotateSpeed"></param>
    /// <returns></returns>
    public static float GetRotateTime(this Transform transform, Transform target, float rotateSpeed)
    {
        return transform.GetRotateTime(target.position, rotateSpeed);
    }

    /// <summary>
    /// Calculate time to rotate from current rotation to look-at-target rotation
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="target"></param>
    /// <param name="rotateSpeed"></param>
    /// <returns></returns>
    public static float GetRotateTime(this Transform transform, Vector3 target, float rotateSpeed)
    {
        Vector3 direction = (target - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, direction);
        return angle / rotateSpeed;
    }

    // public static void SetPosition(this MonoBehaviour main, Vector3 position)
    // {
    //     main.transform.position = position;
    // }

    public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
    {
        var parent = transform.parent;
        Vector3 parentLossyScale = (parent == null) ? Vector3.one : parent.lossyScale;
        transform.localScale = new Vector3(globalScale.x / parentLossyScale.x, globalScale.y / parentLossyScale.y,
            globalScale.z / parentLossyScale.z);
    }

    #endregion

    #region ANIMATOR

    /// <summary>
    /// Check if the animator is playing state with specific name 
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateName"></param>
    /// <returns></returns>
    public static bool IsPlayingStateName(this Animator animator, string stateName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    /// <summary>
    /// Check if the animator is playing state with specific name 
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateHashedName"></param>
    /// <returns></returns>
    public static bool IsPlayingStateName(this Animator animator, int stateHashedName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateHashedName);
    }

    /// <summary>
    /// Check if the animator is playing state with specific tag
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static bool IsPlayingStateTag(this Animator animator, string tag)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsTag(tag);
    }

    /// <summary>
    /// Check if the animator is playing state with specific tag
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="hashedTag"></param>
    /// <returns></returns>
    public static bool IsPlayingStateTag(this Animator animator, int hashedTag)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsTag(hashedTag);
    }

    public static bool IsPlayingStateNoTransition(this Animator animator, int hashedName)
    {
        return !animator.IsInTransition(0) && animator.IsPlayingStateName(hashedName);
    }

    /// <summary>
    /// Return true if the animator is in transition from input state
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateName"></param>
    /// <param name="layerIndex"></param>
    /// <returns></returns>
    public static bool IsInTransitionFrom(this Animator animator, string stateName, int layerIndex = 0)
    {
        return animator.IsInTransition(layerIndex) &&
               animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
    }

    /// <summary>
    /// Return true if the animator is in transition from input state
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateHashName"></param>
    /// <param name="layerIndex"></param>
    /// <returns></returns>
    public static bool IsInTransitionFrom(this Animator animator, int stateHashName, int layerIndex = 0)
    {
        return animator.IsInTransition(layerIndex) &&
               animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateHashName);
    }

    /// <summary>
    /// Return true if the animator is in transition from input state
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateName"></param>
    /// <param name="layerIndex"></param>
    /// <returns></returns>
    public static bool IsInTransitionFromTag(this Animator animator, string stateName, int layerIndex = 0)
    {
        return animator.IsInTransition(layerIndex) &&
               animator.GetCurrentAnimatorStateInfo(layerIndex).IsTag(stateName);
    }

    /// <summary>
    /// Return true if the animator is in transition from input state
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateHashTag"></param>
    /// <param name="layerIndex"></param>
    /// <returns></returns>
    public static bool IsInTransitionFromTag(this Animator animator, int stateHashTag, int layerIndex = 0)
    {
        return animator.IsInTransition(layerIndex) &&
               animator.GetCurrentAnimatorStateInfo(layerIndex).IsTag(stateHashTag);
    }

    /// <summary>
    /// Optimize function AnimatorStateInfo.IsName
    /// </summary>
    /// <param name="animatorStateInfo"></param>
    /// <param name="hashName"></param>
    /// <returns></returns>
    public static bool IsName(this AnimatorStateInfo animatorStateInfo, int hashName)
    {
        return animatorStateInfo.fullPathHash == hashName || animatorStateInfo.shortNameHash == hashName;
    }

    /// <summary>
    /// Optimize function AnimatorStateInfo.IsTag
    /// </summary>
    /// <param name="animatorStateInfo"></param>
    /// <param name="hashTag"></param>
    /// <returns></returns>
    public static bool IsTag(this AnimatorStateInfo animatorStateInfo, int hashTag)
    {
        return animatorStateInfo.tagHash == hashTag;
    }

    #endregion

    #region RECT TRANSFORM

    public static void SetSize(this RectTransform rectTransform, Vector2 newSize)
    {
        var parentSize = rectTransform.parent.GetComponent<RectTransform>().rect.size;
        var anchorPercent = rectTransform.anchorMax - rectTransform.anchorMin;
        var anchorSize = parentSize * anchorPercent;
        rectTransform.sizeDelta = newSize - anchorSize;
    }

    public static bool TrySetAnchor(this RectTransform rectTransform, Vector2 parentSize, Vector2 targetAnchorMin,
        Vector2 targetAnchorMax)
    {
        if (targetAnchorMin.x < 0 || targetAnchorMin.x > 1 || targetAnchorMin.y < 0 || targetAnchorMin.y > 1)
            return false;

        if (targetAnchorMax.x < 0 || targetAnchorMax.x > 1 || targetAnchorMax.y < 0 || targetAnchorMax.y > 1)
            return false;

        var sizeDelta = rectTransform.rect.size;
        var anchorPosition = rectTransform.anchoredPosition;
        // var oldAverageAnchor = (rectTransform.anchorMin + rectTransform.anchorMax) / 2;
        // var targetAverageAnchor = (targetAnchorMin + targetAnchorMax) / 2;
        var oldAverageAnchor =
            Lerp(rectTransform.anchorMin, rectTransform.anchorMax, rectTransform.pivot);
        var targetAverageAnchor = Lerp(targetAnchorMin, targetAnchorMax, rectTransform.pivot);

        anchorPosition += (oldAverageAnchor - targetAverageAnchor) * parentSize;
        sizeDelta -= parentSize * (targetAnchorMax - targetAnchorMin);

        rectTransform.anchorMin = targetAnchorMin;
        rectTransform.anchorMax = targetAnchorMax;
        rectTransform.anchoredPosition = anchorPosition;
        rectTransform.sizeDelta = sizeDelta;

        return true;
    }

    public static void SetAnchor(this RectTransform rectTransform, Vector2 parentSize, Vector2 targetAnchorMin,
        Vector2 targetAnchorMax)
    {
        var sizeDelta = rectTransform.rect.size;
        var anchorPosition = rectTransform.anchoredPosition;
        var oldAverageAnchor =
            Lerp(rectTransform.anchorMin, rectTransform.anchorMax, rectTransform.pivot);
        var targetAverageAnchor = Lerp(targetAnchorMin, targetAnchorMax, rectTransform.pivot);

        anchorPosition += (oldAverageAnchor - targetAverageAnchor) * parentSize;
        sizeDelta -= parentSize * (targetAnchorMax - targetAnchorMin);

        rectTransform.anchorMin = targetAnchorMin;
        rectTransform.anchorMax = targetAnchorMax;
        rectTransform.anchoredPosition = anchorPosition;
        rectTransform.sizeDelta = sizeDelta;
    }

    #endregion

    #region SUPPORT

    public static Vector2 Lerp(Vector2 a, Vector2 b, Vector2 t)
    {
        return new Vector2(Mathf.Lerp(a.x, b.x, t.x), Mathf.Lerp(a.y, b.y, t.y));
    }

    #endregion
}