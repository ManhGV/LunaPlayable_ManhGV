using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CraskGroundRockZom : GameUnit
{
#if UNITY_EDITOR
    [SerializeField] private GameConstants.ProjecctileZombie _projecctileZombie;
    private void OnValidate()
    {
        PoolType = (GameConstants.PoolType)_projecctileZombie;
    }
#endif
    
    [SerializeField] private Transform[] _rockChildMove;
    [SerializeField] private float _speedMove = 1.0f;
    [SerializeField] private float _timeDelay = 0.5f;
    [SerializeField] private float _timeDespawnOneChild = 2.0f;
    private int _completedRocks = 0;

    public void OnInit(Vector3 _positionPlayer,Vector3 _posZomAttack)
    {
        TF.position = new Vector3(TF.position.x, 0, TF.position.z);
        StartCoroutine(IEOnInit(_posZomAttack,_positionPlayer));
    }

    public void OnDespawn()
    {
        _completedRocks = 0;
        SimplePool.Despawn(this);
    }

    private IEnumerator IEOnInit(Vector3 _posZomAttack,Vector3 _posPlayerMain)
    {
        _posZomAttack.y = 0;
        Vector3 direction = (_posZomAttack - _posPlayerMain).normalized;
        float spacing = Vector3.Distance(_posPlayerMain, _posZomAttack) / _rockChildMove.Length - 1; // Khoảng cách giữa các điểm

        for (int i = 1; i < _rockChildMove.Length + 1; i++)
        {
            Vector3 spawnPos = _posPlayerMain + direction * (spacing * i);
            spawnPos.y = -4f;
            _rockChildMove[i - 1].position = spawnPos;
            StartCoroutine(IEMoveRockChild(_rockChildMove[i-1]));
            if (i == 4)
            {
                print("TODO: Take damage to player");
            }
            yield return new WaitForSeconds(_timeDelay);
        }
    }
    
    private IEnumerator IEMoveRockChild(Transform _rockChild)
    {
        Vector3 targetPos =_rockChild.localPosition;
        targetPos.y = 0;
        while (Vector3.Distance(_rockChild.localPosition, targetPos) > 0.01f)
        {
            _rockChild.localPosition = Vector3.MoveTowards(_rockChild.localPosition, targetPos, _speedMove * Time.deltaTime);
            yield return null;
        }

        // Dừng lại một chút nếu muốn
        yield return new WaitForSeconds(_timeDespawnOneChild);

        targetPos.y = -4f;
        while (Vector3.Distance(_rockChild.localPosition, targetPos) > 0.01f)
        {
            _rockChild.localPosition = Vector3.MoveTowards(_rockChild.localPosition, targetPos, _speedMove/2 * Time.deltaTime);
            yield return null;
        }
        
        _completedRocks++;

        if (_completedRocks >= _rockChildMove.Length)
        {
            OnDespawn();
        }
    }
}
