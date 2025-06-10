using System;
using UnityEngine;

public abstract class StateBase<EState,TBot> : MonoBehaviour where EState : Enum where TBot : ZombieBase
{
    protected bool isDoneState;
    protected TBot thisBotNetworks;

#if UNITY_EDITOR
    private void OnValidate()
    {
        thisBotNetworks = GetComponent<TBot>();
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
