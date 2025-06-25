using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectLine : EffectBase
{
    [SerializeField] LineRenderer _lineRenderer;
    float _linedistance; // Khoảng cách giữa các điểm trên đường thẳng
    float xOffset = 0;
    private Transform posStart;
    private Transform posEnd;
    private EffectElectricHit effectElectricHit;
    
    public void OnInit(Transform posStart_project, Transform posEnd_centerZom,float radiusExplosion)
    {
        _linedistance = radiusExplosion;
        this.posStart = posStart_project;
        this.posEnd = posEnd_centerZom;
        OnInit();
        Update();
        if (posEnd_centerZom.childCount <= 1)
        {
            effectElectricHit = SimplePool.Spawn<EffectElectricHit>(GameConstants.PoolType.vfx_ElectricHit, TF.position, Quaternion.identity);
            effectElectricHit.OnInit(posEnd_centerZom.localScale.x,posEnd_centerZom);
        }
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
        if(!posStart.gameObject.activeSelf || !posEnd.gameObject.activeSelf||Vector3.Distance(posStart.position, posEnd.position)>_linedistance)
            OnDespawn();
        
        _lineRenderer.SetPosition(0, posStart.position);
        _lineRenderer.SetPosition(1, posEnd.position);
        
        xOffset += Time.deltaTime * 7;
        if (xOffset >= 0.6) xOffset = 0;
        _lineRenderer.material.mainTextureOffset = new Vector2(xOffset, 0);
    }

    public override void OnDespawn()
    {
        effectElectricHit = null;
        base.OnDespawn();
    }
}