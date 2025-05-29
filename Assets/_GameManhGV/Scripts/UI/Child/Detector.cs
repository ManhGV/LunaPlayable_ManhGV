using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Detector : MonoBehaviour
{
    [SerializeField] private bool PlayAwake;
    [SerializeField] private float _scaleDefault;
    [Header("Look Camera")]
    [SerializeField] private Transform cameraTransform;
    
    [Header("Phóng to sau đó thu nhỏ Derector")]
    [SerializeField] private float _scaleSpeed = 60f;  
    [SerializeField] private float _scaleMultiplier = 10f;

    private Vector3 _originalScale;
    private bool _isPlaying;
    
    [SerializeField] private float lifeTime = 2f;
    private Coroutine _currentCoroutine;
    private Coroutine _ActiveFalseCoroutine;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        cameraTransform = Camera.main.transform;
    }
#endif
    private void OnEnable()
    {
        transform.localScale = Vector3.one * _scaleDefault;
        _originalScale = Vector3.one * _scaleDefault;
        _ActiveFalseCoroutine = StartCoroutine(DisableThis());
        
        if(PlayAwake)
            Play();
    }

    private void OnDisable()
    {
        if (_ActiveFalseCoroutine != null)
            StopCoroutine(_ActiveFalseCoroutine);
    }

    private void LateUpdate()
    {
        if (cameraTransform != null)
        {
            transform.LookAt(cameraTransform);
        }
    }

    private IEnumerator DisableThis()
    {
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
    }
    
    public void Play()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }

        _currentCoroutine = StartCoroutine(AnimateScale());
    }

    private IEnumerator AnimateScale()
    {
        Vector3 targetScale = _originalScale;
        Vector3 startScale = _originalScale * _scaleMultiplier;
        print(targetScale+"  "+startScale);

        // Gán giá trị phóng to ngay lập tức
        transform.localScale = startScale;

        while (true)
        {
            if (Vector3.Distance(transform.localScale, targetScale) > 0.001f)
                transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, _scaleSpeed * Time.deltaTime);
            else
                break;
            yield return null;
        }

        _currentCoroutine = null;
    }
}
