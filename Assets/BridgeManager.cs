using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class BridgeManager : InGameManager
{
    public Action<Vector3>[] onBridgeResults;
    [SerializeField] private BridgeSpawner bridgeSpawner;
    // 나중에 스포너를 다른곳으로 쓸수있게 프로퍼티 열어둠
    public BridgeSpawner Spawner => bridgeSpawner;
    public override void Initialize()
    {
        base.Initialize();
        if (bridgeSpawner != null)
        {
            bridgeSpawner.InitializeSpawner();
        }

        GameManager.Instance.AddGameStateEnterAction(GameManager.GameState.GameReady, () =>
        {
            bridgeSpawner.SpawnNextBridge(bridgeSpawner.transform.position, true);
        });


        onBridgeResults = new Action<Vector3>[2];

        // 2. 결과에 따른 로직 등록 (GameManager의 InitializeStateActions와 같은 방식)
        AddResultAction(false, (pos) =>
        {
            GameManager.Instance.SetGameState(GameManager.GameState.GameOver);
        });

        AddResultAction(true, (pos) =>
        {
            GameManager.Instance.PlayerManager.MovePlayer(pos);
            bridgeSpawner.SpawnNextBridge(pos);
        });
    }


    public void OnBridgeResult(bool isSuccess, Vector3 targetPos)
    {
        int index = isSuccess ? 1 : 0;

        // 해당되는 액션 실행!
        onBridgeResults[index]?.Invoke(targetPos);
    }

    public void AddResultAction(bool isSuccess, Action<Vector3> action)
    {
        onBridgeResults[isSuccess ? 1 : 0] += action;
    }

}
