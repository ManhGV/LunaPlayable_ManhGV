using UnityEngine;
using static GameConstants;
public class GameUnit : MonoBehaviour
{
    public PoolType PoolType;
    private Transform tf;
    public Transform TF
    {
        get
        {
            if (tf == null)
            {
                tf = transform;
            }

            return tf;
        }
    }
}
