using System;
using UnityEngine;

public abstract class StateBase<EState,TBot> : MonoBehaviour where EState : Enum where TBot : ZombieBase
{
    [SerializeField] protected TBot thisBotNetworks;
    [SerializeField] protected StateControllerBase<TBot> thisStateController;
#if UNITY_EDITOR
    private void OnValidate()
    {
        thisBotNetworks = GetComponent<TBot>();
        thisStateController = GetComponent<StateControllerBase<TBot>>();
    }
#endif
    
    public EState StateKey { get; private set; }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public void Initialize(EState state)
    {
        this.StateKey = state;
    }
}
