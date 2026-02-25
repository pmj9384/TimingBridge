using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class BridgeManager : InGameManager
{
    public static event Action<int> OnScoreChanged;

    public bool IsLastSuccess { get; private set; }
    public Action<Vector3>[] onBridgeResults;
    public Action<bool, Vector3> OnBridgeResultOccurred;

    private int score = 0;
    [SerializeField] private BridgeSpawner bridgeSpawner;
    // 나중에 스포너를 다른곳으로 쓸수있게 프로퍼티 열어둠
    public BridgeSpawner Spawner => bridgeSpawner;
    public bool CanBuild { get; private set; } = true;
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
            GameManager.Instance.PlayerManager.MovePlayer(pos, false);
        });

        AddResultAction(true, (pos) =>
        {
            Vector3 realTarget = Spawner.GetCurrentPlatformPos();
            GameManager.Instance.PlayerManager.MovePlayer(realTarget, true);
            GameManager.Instance.PlayerManager.OnPlayerArrived += Spawner.ReleasePreviousSet;
            bridgeSpawner.SpawnNextBridge(realTarget);
        });

    }


    public void OnBridgeResult(bool isSuccess, Vector3 targetPos)
    {
        CanBuild = false;
        // [추가] 상태 기록 및 외부 방송
        IsLastSuccess = isSuccess;
        OnBridgeResultOccurred?.Invoke(isSuccess, targetPos);
        if (isSuccess)
        {
            score++;
            OnScoreChanged?.Invoke(score);
        }

        int index = isSuccess ? 1 : 0;

        // 해당되는 액션 실행!
        onBridgeResults[index]?.Invoke(targetPos);
    }

    public void AddResultAction(bool isSuccess, Action<Vector3> action)
    {
        onBridgeResults[isSuccess ? 1 : 0] += action;
    }

}
