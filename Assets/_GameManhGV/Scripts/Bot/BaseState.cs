using System;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class BaseState<EState> : MonoBehaviour where EState : Enum
{
    [SerializeField] protected BotNetwork thisBotNetwork;
    protected bool isDoneState;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        thisBotNetwork = GetComponent<BotNetwork>();
    }
#endif
    public EState StateKey { get; private set; }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract EState GetNextState();
    public void Initialize(EState state)
    {
        this.StateKey = state;
    }
}
