using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : InGameManager
{
    [SerializeField] private float moveSpeed = 5.0f; // 인스펙터에서 수정 가능
    [SerializeField] private PlayerMove playerMove;
    public System.Action OnPlayerArrived;

    public bool IsMoving => playerMove != null && playerMove.IsMoving;
    public override void Initialize()
    {
        base.Initialize();
        playerMove.OnArrival += () => OnPlayerArrived?.Invoke();
    }


    // GameManager나 BridgeManager가 호출할 함수
    public void MovePlayer(Vector3 targetPos)
    {
        playerMove?.MoveToTarget(targetPos, moveSpeed);

    }
}
