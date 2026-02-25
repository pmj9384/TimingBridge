using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : InGameManager
{
    [SerializeField] private float moveSpeed = 5.0f; // 인스펙터에서 수정 가능
    public System.Action OnPlayerArrived;
    private PlayerMove playerMove;
    private Rigidbody rb;
    public bool IsMoving => playerMove != null && playerMove.IsMoving;
    public override void Initialize()
    {
        base.Initialize();
        GameObject playerObj = GameObject.FindWithTag("Player");
        playerMove = playerObj.GetComponent<PlayerMove>();
        rb = playerObj.GetComponent<Rigidbody>();
        playerMove.OnArrival += () => OnPlayerArrived?.Invoke();
        playerMove.OnFailureArrival += () => GameManager.Instance.SetGameState(GameManager.GameState.GameOver);

        // 3. 게임오버 시 물리 작동 액션 등록
        GameManager.Instance.AddGameStateEnterAction(GameManager.GameState.GameOver, () =>
        {
            playerMove.StopMoving();
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.velocity = Vector3.zero; // 앞으로 가던 힘 삭제
            rb.angularVelocity = Vector3.zero;
            Debug.Log("플레이어 추락 시퀀스 시작");
        });
    }


    // GameManager나 BridgeManager가 호출할 함수
    public void MovePlayer(Vector3 targetPos, bool isSuccess)
    {
        playerMove?.MoveToTarget(targetPos, moveSpeed, isSuccess);

    }
}
