using UnityEngine;

public class BossZomSwat_Start : BaseState<BossZomSwatState>
{
    [SerializeField] float timeStart;
    float timer;
    public override void EnterState()
    {
        GameManager.Instance.StartCutScene();
        isDoneState = false;
        timer = timeStart;
    }

    public override void UpdateState()
    {
        if (isDoneState)
            return;
        timer -= Time.deltaTime;
        if (timer <= 0)
            isDoneState = true;
    }

    public override void ExitState()
    {
        GameManager.Instance.EndCutScene();
    }

    public override BossZomSwatState GetNextState()
    {
        if (thisBotNetwork.IsDead)
        {
            return BossZomSwatState.Dead;
        }
        else
        {
            if (isDoneState)
                return BossZomSwatState.Move;
            else
                return StateKey;
        }
    }
}