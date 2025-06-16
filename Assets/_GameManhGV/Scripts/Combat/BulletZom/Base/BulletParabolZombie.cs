using System;
using System.Collections;
using System.Data;
using UnityEngine;

public class BulletParabolZombie : BulletZomBase
{
    [Header("Referent")]
    [SerializeField] Transform detector;
    [SerializeField] float speedMoveForDistance = .2f;
    [SerializeField] float height = 3f;
    float timeMove = 1f;
    Coroutine _flyCoroutine;

    public virtual void SetupSpawn(Transform _parent, float _scale)
    {
        TF.parent = _parent;
        TF.rotation = Quaternion.Euler(Vector3.zero);
        TF.localScale = Vector3.one * _scale;
        colliderThis.enabled = true;
        _healthFill.fillAmount = 1f;
    }
    
    public override void OnInit(Vector3 _posPlayer)
    {
        base.OnInit(_posPlayer);
        TF.parent = null;
        detector.gameObject.SetActive(true);
        timeMove = speedMoveForDistance * Vector3.Distance(TF.position, posPlayer);
        _flyCoroutine = StartCoroutine(IEFlyInArc(TF.position, posPlayer, timeMove, height));
    }

    protected virtual void Update()
    {
        if (_isDead)
            return;
        detector.LookAt(GameManager.Instance.GetMainCameraTransform());
        if(Vector3.Distance(TF.position, posPlayer) <2f)
            OnTakeDamagePlayer();
    }

    IEnumerator IEFlyInArc(Vector3 startPoint, Vector3 endPoint, float duration, float height)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Vị trí thẳng giữa A và B
            Vector3 linearPoint = Vector3.Lerp(startPoint, endPoint, t);

            // Thêm độ cong theo chiều cao parabol
            float arcHeight = 4 * height * t * (1 - t); // đỉnh ở giữa

            // Thêm vào chiều Y
            linearPoint.y += arcHeight;

            transform.position = linearPoint;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPoint; // đảm bảo kết thúc chính xác
    }

    public override void TakeDamage(DamageInfo damageInfo)
    {
        base.TakeDamage(damageInfo);
        if(!important && _isDead)
           OnDead();
    }

    protected override void OnTakeDamagePlayer()
    {
        base.OnTakeDamagePlayer();
        OnDead();
    }

    public override void OnDead()
    {
        base.OnDead();
        colliderThis.enabled = false;
        detector.gameObject.SetActive(false);
        if(_flyCoroutine != null)
            StopCoroutine(_flyCoroutine);
    }

    public override void OnDespawn()
    {
        TF.localScale = Vector3.one;
        base.OnDespawn();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (TF == null || posPlayer == null)
            return;

        Vector3 startPoint = TF.position;
        Vector3 endPoint = posPlayer;

        Gizmos.color = Color.blue;

        int resolution = 30; // số đoạn chia nhỏ đường cong
        Vector3 previousPoint = startPoint;

        for (int i = 1; i <= resolution; i++)
        {
            float t = i / (float)resolution;

            // Tính vị trí theo t trên đoạn thẳng
            Vector3 pointOnLine = Vector3.Lerp(startPoint, endPoint, t);

            // Tính độ cong theo công thức parabol (đỉnh giữa)
            float arcHeight = 4 * height * t * (1 - t);

            // Thêm vào trục Y
            pointOnLine.y += arcHeight;

            Gizmos.DrawLine(previousPoint, pointOnLine);
            previousPoint = pointOnLine;
        }
    }
#endif
}