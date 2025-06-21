using UnityEngine;

public class BulletFireZombie : BulletZomBase
{
    [Header("Referent")]
    [SerializeField] Transform detector;
    [SerializeField] AudioClip[] audioClip;
    [SerializeField] protected float _speed;
    Vector3 dir;

    public override void OnInit(Vector3 _posPlayer)
    {
        base.OnInit(_posPlayer);
        dir = (_posPlayer - TF.position).normalized;
        AudioManager.Instance.PlaySound(audioClip[0],1);
    }

    private void Update()
    {
        if (_isDead)
            return;
        TF.position += dir * _speed * Time.deltaTime;
        detector.LookAt(_posCamera);
        if (Vector3.Distance(posPlayer, TF.position) < .5f)
            OnTakeDamagePlayer();
    }

    public override void TakeDamage(DamageInfo damageInfo)
    {
        base.TakeDamage(damageInfo);
        if(_isDead)
            OnDespawn();
    }

    protected override void OnTakeDamagePlayer()
    {
        base.OnTakeDamagePlayer();
        OnDespawn();
    }

    public override void OnDespawn()
    {
        AudioManager.Instance.PlaySound(audioClip[1],1);
        base.OnDespawn();
    }

}