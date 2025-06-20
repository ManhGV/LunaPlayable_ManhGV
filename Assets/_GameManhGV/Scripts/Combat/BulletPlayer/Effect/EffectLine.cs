using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectLine : EffectBase
{
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] float _linedistance = 5f; // Khoảng cách giữa các điểm trên đường thẳng
    float xOffset = 0;
    private Transform posStart;
    private Transform posEnd;
    
    public void OnInit(Transform posStart, Transform posEnd)
    {
        this.posStart = posStart;
        this.posEnd = posEnd;
        OnInit();
        Update();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (_lineRenderer == null)
        {
            print("Null LineRenderer");
            return; 
        }
#endif
            
        if(!posStart.gameObject.activeSelf || !posEnd.gameObject.activeSelf || Vector3.Distance(posStart.position, posEnd.position) > _linedistance)
            OnDespawn();
        
        _lineRenderer.SetPosition(0, posStart.position);
        _lineRenderer.SetPosition(1, posEnd.position);
        
        xOffset += Time.deltaTime * 7;
        if (xOffset >= 0.6) xOffset = 0;
        _lineRenderer.material.mainTextureOffset = new Vector2(xOffset, 0);
    }
}