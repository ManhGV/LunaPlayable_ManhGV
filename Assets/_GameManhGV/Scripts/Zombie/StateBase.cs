using System;
using UnityEngine;

public abstract class StateBase<EState,TBot> : MonoBehaviour where EState : Enum where TBot : ZombieBase
{
    protected TBot thisBotNetworks {get; private set;}
    protected StateControllerBase<TBot> thisStateController { get; private set; }
    
    public EState StateKey { get; private set; }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public void Initialize(EState state, TBot _botNet, StateControllerBase<TBot> _stateControllerBase)
    {
        StateKey = state;
        thisBotNetworks = _botNet;
        thisStateController = _stateControllerBase;
    }
}
