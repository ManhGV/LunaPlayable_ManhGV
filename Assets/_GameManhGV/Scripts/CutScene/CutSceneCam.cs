using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneCam : Singleton<CutSceneCam>
{
    [SerializeField] private Camera _cam;
    [SerializeField] private Transform[] pointMove;
    [SerializeField] private float timeMove = 1f;

    public void Setparent(int _indexPoint,float _fieldOfView)
    {
        _cam.fieldOfView = _fieldOfView;
        TF.parent = pointMove[_indexPoint];
        TF.localPosition = Vector3.zero;
        TF.localRotation = Quaternion.Euler(Vector3.zero);
    }
    
    public void MoveFromAToB(int _indexPoin1, int _indexPoint2, float _duration, float _fieldOfView)
    {
        _cam.fieldOfView = _fieldOfView;
        StartCoroutine(IEMoveFromAToB(pointMove[_indexPoin1].position, pointMove[_indexPoint2].position, _duration));
    }
    
    IEnumerator IEMoveFromAToB(Vector3 startPos, Vector3 endPos,float _duration)
    {
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            TF.position = Vector3.Lerp(startPos, endPos, elapsed / _duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        TF.position = endPos; // đảm bảo tới đúng điểm B
    }
}
