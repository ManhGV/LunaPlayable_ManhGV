using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBloodBlobZom : BulletParabolZombie
{
    [SerializeField] private GameObject _body;
    [SerializeField] private ParticleSystem vfxExplosion;
    
    public override void SetupSpawn(Transform _parent, float _scale)
    {
        base.SetupSpawn(_parent, _scale);
        _body.SetActive(true);
        important = true;
    }

    public override void OnInit(Vector3 _posPlayer)
    {
        base.OnInit(_posPlayer);
        important = false;
    }

    public override void OnDead()
    {
        base.OnDead();
        _body.SetActive(false);
        vfxExplosion.Play();   
        Invoke(nameof(Destroy), .65f);
    }
}