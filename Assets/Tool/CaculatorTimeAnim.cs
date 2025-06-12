using UnityEngine;

public class CaculatorTimeAnim : MonoBehaviour
{
    [Header("Caculator Distance")] 
    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;
    
    [Header("Anim")]
    [SerializeField] private Animator animator;
    float time = 0f;
    private void OnValidate()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        print("distance: " + Vector3.Distance(point1.position, point2.position));
        time += Time.deltaTime;
    }

    public void ResetTime()
    {
        time = 0f;
    }
    
    public void PrintTime()
    {
        Debug.Log(" : " + time);
    }
}
