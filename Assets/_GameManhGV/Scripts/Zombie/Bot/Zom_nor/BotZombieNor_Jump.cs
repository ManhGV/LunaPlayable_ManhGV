using static GameConstants;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class BotZombieNor_Jump : StateBase<ZomAllState, BotNetwork>
{
    [SerializeField] private BotZombieNorStateController _stateController;
    [SerializeField] private Transform TF;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpVelocity = 5f;           // tốc độ nhảy lên (m/s)
    [SerializeField] private float fallSpeed = 10f;             // tốc độ rơi xuống (m/s)
    [SerializeField] private float moveForwardDistance = 2f;    // khoảng cách nhảy ngang


    private Coroutine jumpCoroutine;

    public override void EnterState()
    {
        if (jumpCoroutine != null)
            StopCoroutine(jumpCoroutine);

        jumpCoroutine = StartCoroutine(JumpRoutine());
    }
    
    private IEnumerator JumpRoutine()
    {
        thisBotNetworks.ChangeAnimAndType("Jump", 0);
        yield return new WaitForSeconds(.4f);
        thisBotNetworks.ChangeAnimAndType("Jump", 1);
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + transform.forward * moveForwardDistance;
        float groundY = startPos.y;

        float g = -Physics.gravity.y;

        // === Tính toán dựa trên vận tốc nhảy ===
        float jumpUpDuration = jumpVelocity / g;                    // t = v / g
        float jumpHeight = (jumpVelocity * jumpVelocity) / (2 * g); // h = v^2 / 2g

        float elapsedTime = 0f;

        // === Nhảy lên ===
        while (elapsedTime < jumpUpDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / jumpUpDuration;
            float height = 4 * jumpHeight * t * (1 - t); // Parabol đẹp và mượt

            float forwardT = Mathf.Clamp01(elapsedTime / jumpUpDuration);
            Vector3 horizontalPos = Vector3.Lerp(startPos, targetPos, forwardT);

            float y = groundY + height;
            TF.position = new Vector3(horizontalPos.x, y, horizontalPos.z);

            yield return null;
        }

        // === Rơi xuống ===
        float currentY = TF.position.y;

        while (_stateController.EndJump())
        {
            float dt = Time.deltaTime;
            currentY -= fallSpeed * dt;

            float forwardT = 1f; // giữ nguyên vị trí ngang cuối cùng
            Vector3 horizontalPos = Vector3.Lerp(startPos, targetPos, forwardT);

            TF.position = new Vector3(horizontalPos.x, currentY, horizontalPos.z);

            yield return null;
        }
        
        thisBotNetworks.ChangeAnimAndType("Jump", 2);
        startPos = TF.position;
        if (Physics.Raycast(TF.position, Vector3.down, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 groundPoint = hit.point;
            
            elapsedTime = 0f;
            while (elapsedTime < .15f)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / .15f;

                Vector3 newPos = Vector3.Lerp(startPos, groundPoint, t);
                thisBotNetworks.TF.position = newPos;

                yield return null;
            }
            thisBotNetworks.TF.position = groundPoint;
        }   
        yield return new WaitForSeconds(.5f);    

        // Đảm bảo đứng chính xác trên mặt đất
        //Debug.Log("Jump ended when grounded.");
        if(thisBotNetworks.IsDead)
            thisStateController.ChangeState(ZomAllState.Dead);
        else
            thisStateController.ChangeState(ZomAllState.Move);
    }


    public override void UpdateState() { }

    public override void ExitState()
    {
        if (jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
            jumpCoroutine = null;
        }
    }
}
