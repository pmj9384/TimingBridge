using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class BridgeManager : InGameManager
{
    public static event Action<int> OnScoreChanged;

    public bool IsLastSuccess { get; private set; }
    public Action<Vector3>[] onBridgeResults;
    public Action<bool, Vector3, bool> OnBridgeResultOccurred;

    private int score = 0;
    public int Score => score;
    [SerializeField] private BridgeSpawner bridgeSpawner;
    [SerializeField] private GameObject criticalEffect;
    [SerializeField] private DifficultyConfig difficultyConfig;
    // 나중에 스포너를 다른곳으로 쓸수있게 프로퍼티 열어둠
    public BridgeSpawner Spawner => bridgeSpawner;
    public bool CanBuild { get; set; } = true;
    private bool hasRevive = true;
    public bool HasRevive => hasRevive;
    public override void Initialize()
    {
        base.Initialize();
        if (bridgeSpawner != null)
        {
            bridgeSpawner.InitializeSpawner();
        }

        GameManager.Instance.AddGameStateEnterAction(GameManager.GameState.GamePlay, () =>
        {
            if (GameManager.Instance.PreviousState == GameManager.GameState.GameStop) return;
            SpawnBridge(bridgeSpawner.GetCurrentPlatformPos());
            bridgeSpawner.ShowCurrentBridge();
        });
        GameManager.Instance.AddGameStateEnterAction(GameManager.GameState.GameStop, () =>
        {
            bridgeSpawner.CurrentBridge?.GetComponent<CubeRoad>()?.ResetGrow();
        });
        GameManager.Instance.AddGameStateEnterAction(GameManager.GameState.GameOver, () =>
        {
            bridgeSpawner.CurrentBridge?.GetComponent<CubeRoad>()?.OnGameOver();
            GameDataManager.Instance.PlayerAccountData.TryUpdateBestScore(score);
            SaveLoadSystem.Instance.Save();
        });

        GameManager.Instance.PlayerManager.OnPlayerArrived += () =>
        {
            Spawner.ReleasePreviousSet();
            Spawner.ShowCurrentBridge();
            CanBuild = true;
        };

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
            SpawnBridge(realTarget);
        });

    }


    private void SpawnBridge(Vector3 pos)
    {
        float growSpeed     = difficultyConfig != null ? difficultyConfig.GetGrowSpeed(score)      : 5f;
        float platformScale = difficultyConfig != null ? difficultyConfig.GetPlatformZScale(score) : 1f;
        bridgeSpawner.SpawnNextBridge(pos, growSpeed, platformScale);
    }

    public void OnBridgeResult(bool isSuccess, Vector3 targetPos, bool isCritical = false)
    {
        CanBuild = false;
        IsLastSuccess = isSuccess;
        OnBridgeResultOccurred?.Invoke(isSuccess, targetPos, isCritical);
        if (isSuccess)
        {
            if (isCritical)
            {
                score += 2;
                SoundManager.Instance.PlaySfx(SfxClipId.LandCritical);
                if (criticalEffect != null)
                {
                    Vector3 fxPos = new Vector3(targetPos.x, targetPos.y + 1f, targetPos.z);
                    GameObject fx = Instantiate(criticalEffect, fxPos, Quaternion.identity);
                    Destroy(fx, 3f);
                }
            }
            else
            {
                score += 1;
                SoundManager.Instance.PlaySfx(SfxClipId.LandSuccess);
            }
            OnScoreChanged?.Invoke(score);
        }
        else
        {
            SoundManager.Instance.PlaySfx(SfxClipId.LandFail);
        }

        int index = isSuccess ? 1 : 0;

        // 해당되는 액션 실행!
        onBridgeResults[index]?.Invoke(targetPos);
    }

    public void Revive()
    {
        hasRevive = false;
        CanBuild = true;
    }

    public void AddResultAction(bool isSuccess, Action<Vector3> action)
    {
        onBridgeResults[isSuccess ? 1 : 0] += action;
    }

}
