using UnityEngine;
using static GameConstants;

public class BossZomSwat_Start : StateBase<ZomAllState,BossZomSwatNetword>
{
    [SerializeField] HumanMoveBase humanMoveBase;
    [SerializeField] private Transform _pointMoveEnd;
    [SerializeField] private Transform _pointInitMove;
    [SerializeField] float timeChangeCam = 2.5f;
    [SerializeField] float timeEndState = 2.5f;
    bool isSetparent = false;
    bool isDoneMove = false;
    public override void EnterState()
    {
        
    }

    public override void UpdateState()
    {
        if (isDoneMove)
        {
            timeEndState -= Time.deltaTime;
            if(timeEndState <= 0)
                thisStateController.ChangeState(ZomAllState.Move);
            return;
        }    
        
        if (isSetparent)
        {
            //TODO:Move cam
            humanMoveBase.SetBotMove(_pointMoveEnd.position,.9f);
            float distance = Vector3.Distance(humanMoveBase.myTrans.position, _pointMoveEnd.position);
            if (distance < 0.1f)
            {
                isDoneMove = true;
                CutSceneCam.Instance.Setparent(4,70f);
                thisBotNetworks.ChangeAnim("RageCutScene");
            }
            
            return;
        }
        
        if (!isSetparent && timeChangeCam <= 0)
        {
            isSetparent = true;
            CutSceneCam.Instance.Setparent(3,50f);
            thisBotNetworks.ChangeAnim("MoveCutScene");
            return;
        }
            
        timeChangeCam -= Time.deltaTime;
    }

    public override void ExitState()
    {
        thisBotNetworks.SetIntAnim("MoveType", 0);
        thisBotNetworks.TF.position = _pointInitMove.position;
        GameManager.Instance.EndCutScene();
    }
}