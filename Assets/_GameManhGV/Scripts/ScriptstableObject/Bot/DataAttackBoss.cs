using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DataAttackBoss", menuName = "ScriptableObjects/DataAttackBoss")]
public class DataAttackBoss : ScriptableObject
{
    public DataSkill[] _dataSkill;
    
    [Serializable]
    public struct DataSkill
    {
        public AudioClip audioAttack;
        public int damageSkill;
        public float timeAttack;
    }
}