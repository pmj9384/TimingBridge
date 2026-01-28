using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : InGameManager
{
    [SerializeField] private float moveSpeed = 5.0f; // 인스펙터에서 수정 가능
    [SerializeField] private PlayerMove playerMove;

    public override void Initialize()
    {
        base.Initialize();
    }


    // GameManager나 BridgeManager가 호출할 함수
    public void MovePlayer(Vector3 targetPos)
    {
        if (playerMove != null)
        {
            playerMove.MoveToTarget(targetPos, moveSpeed);
        }
    }
}
