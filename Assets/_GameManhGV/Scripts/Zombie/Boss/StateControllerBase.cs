using static GameConstants;
using System.Collections.Generic;
using UnityEngine;

public class StateControllerBase<TBotNet> : MonoBehaviour where TBotNet : ZombieBase
{
    [SerializeField] protected bool canDead = true;
    public Dictionary<ZomAllState, StateBase<ZomAllState,TBotNet>> stateController = new Dictionary<ZomAllState, StateBase<ZomAllState,TBotNet>>();
    public StateBase<ZomAllState,TBotNet> _currentState;
    
    [SerializeField] protected TBotNet botNetworks;
    protected bool _isTransition;

    protected virtual  void OnEnable()
    {
#if UNITY_EDITOR
        if(botNetworks==null)
            Debug.LogError("Bot Network trong  stateController Null kìa" + gameObject.name);
#endif
        canDead = true;
        if(botNetworks is BossNetwork bossNetwork)
            bossNetwork.ActionEventDetectorDead += OnEventDetectorDead;
            
        botNetworks.OnTakeDamage += OnTakeDame;
        botNetworks.ZombieDead += ZombieDead;
        ResetState();
    }

    protected virtual void OnEventDetectorDead(int obj)
    {
        
    }

    protected virtual void OnDisable()
    {
        botNetworks.OnTakeDamage -= OnTakeDame;
        botNetworks.ZombieDead -= ZombieDead;
        if(botNetworks is BossNetwork bossNetwork)
            bossNetwork.ActionEventDetectorDead -= OnEventDetectorDead;
    }
    
    protected virtual void Update()
    {
        if (_currentState == null)
        {
//            print("Null current state");
            return;
        }
        
        if (_isTransition)
        {
  //          print("Đang đổi trạng thái");
            return;
        }
        _currentState.UpdateState();
    }

    protected virtual void ResetState()
    {
        
    }
    
    protected virtual  void OnTakeDame(int damage)
    {
        
    }
    
    protected virtual void ZombieDead(bool isDead)
    {
        if (!canDead)
            return;
        canDead = false;
        ChangeState(ZomAllState.Dead);
    }

    #region State Controller
    public void ChangeState(ZomAllState newAllState)
    {
//        print("Đổi trạng thái sang: " + newAllState);
        if (_currentState ==null || _currentState.StateKey.Equals(newAllState) || _isTransition)
            return;

        TransitionState(newAllState);
    }
    private void TransitionState(ZomAllState newAllState)
    {
        _isTransition = true;
        _currentState.ExitState();
        _currentState = stateController[newAllState];
        _currentState.EnterState();
        _isTransition = false;
    }
    #endregion
}
