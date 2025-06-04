using UnityEngine;

public class CaculatorTimeAnim : MonoBehaviour
{
    [SerializeField] private Animator animator;
    float time = 0f;
    private void OnValidate()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
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
