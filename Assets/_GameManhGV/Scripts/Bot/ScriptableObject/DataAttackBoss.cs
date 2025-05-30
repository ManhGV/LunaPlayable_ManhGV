using System;
using UnityEngine;

[CreateAssetMenu(menuName = "DataAttackBoss", fileName = "ScriptableObject/DataAttackBoss")]
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