using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MolotovRota : MonoBehaviour
{
    Transform myTrans;
    [SerializeField] Vector3 rotaDir=Vector3.zero;
    Quaternion rotaQuaternion = Quaternion.identity;
    private void OnEnable()
    {
        myTrans = transform;
        if (rotaDir == Vector3.zero)
        {
            rotaQuaternion = Quaternion.Euler(Random.Range(-5f,5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        }
        else
        {
            rotaQuaternion = Quaternion.Euler(rotaDir);
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        myTrans.rotation *= rotaQuaternion;
    }
}
