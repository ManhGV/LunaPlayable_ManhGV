using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BulletRockZombie : BulletParabolZombie
{
    
    [SerializeField] protected MolotovRota molotovRota;
    [Header("Explosion rock Settings")]
    [SerializeField] private Transform[] rockChildExplosion;
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float timeMocehorizontal = 0.3f;
    [SerializeField] private float fallSpeed = 3f;
    Transform originTransform;

    public override void OnInit(Vector3 _posPlayer)
    {
        base.OnInit(_posPlayer);
        molotovRota.enabled = true;
        important = false;
    }

    public override void SetupSpawn(Transform _parent,float _scale)
    {
        base.SetupSpawn(_parent, _scale);
        molotovRota.gameObject.SetActive(true);
        molotovRota.transform.rotation = Quaternion.Euler(Vector3.zero);
        important = true;
        foreach (Transform VARIABLE in rockChildExplosion)
        {
            VARIABLE.localPosition = Vector3.zero;
            VARIABLE.localRotation = Quaternion.Euler(Vector3.zero);
            VARIABLE.gameObject.SetActive(false);
        }
    }

    public override void OnDead()
    {
        base.OnDead();
        molotovRota.enabled = false;
        molotovRota.gameObject.SetActive(false);
        foreach (Transform VARIABLE in rockChildExplosion)
            StartCoroutine(IEExplodeAndFall(VARIABLE));
        Invoke(nameof(OnDespawn),3f);
    }

    IEnumerator IEExplodeAndFall(Transform rock)
    {
        rock.gameObject.SetActive(true);
        Vector3 origin = rock.position;

        // Hướng nổ ngẫu nhiên theo mặt phẳng XZ
        Vector2 randDir = Random.insideUnitCircle.normalized;
        Vector3 moveDir = new Vector3(randDir.x, randDir.x/4, randDir.y) * explosionRadius;

        float horizontalElapsed = 0f;
        float horizontalDuration = timeMocehorizontal; // thời gian văng ngang

        Vector3 startPos = rock.position;
        Vector3 targetPos = startPos + moveDir;
        float timer = 0;
        while (true)
        {
            // Rơi theo trục Y
            timer += Time.deltaTime;
            rock.position += Vector3.down * fallSpeed * Time.deltaTime;

            // Nổ ngang (chỉ trong vài frame đầu)
            if (horizontalElapsed < horizontalDuration)
            {
                float t = horizontalElapsed / horizontalDuration;
                rock.position = Vector3.Lerp(startPos, targetPos, t) + Vector3.down * fallSpeed * horizontalElapsed;
                horizontalElapsed += Time.deltaTime;
            }

            // Raycast kiểm tra chạm đất
            if (Physics.Raycast(rock.position, Vector3.down, out RaycastHit hit, 0.1f, _groundLayer))
            {
                rock.position = hit.point;
                rock.rotation = Quaternion.Euler(Vector3.zero);
                yield return new WaitForSeconds(2f);
                rock.gameObject.SetActive(false);
                yield break;
            }else if (timer >= 4)
            {
                rock.rotation = Quaternion.Euler(Vector3.zero);
                rock.gameObject.SetActive(false);
                yield break;
            }

            yield return null;
        }
    }
}
