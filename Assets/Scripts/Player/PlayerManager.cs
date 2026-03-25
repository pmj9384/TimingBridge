using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : InGameManager
{
    [SerializeField] private float moveSpeed = 5.0f; // 인스펙터에서 수정 가능
    public System.Action OnPlayerArrived;
    private PlayerMove playerMove;
    private Rigidbody rb;
    private Vector3 lastSuccessPos;
    public bool IsMoving => playerMove != null && playerMove.IsMoving;
    public override void Initialize()
    {
        base.Initialize();
        GameObject playerObj = GameObject.FindWithTag("Player");
        playerMove = playerObj.GetComponent<PlayerMove>();
        rb = playerObj.GetComponent<Rigidbody>();
        lastSuccessPos = playerObj.transform.position;
        playerMove.OnArrival += () =>
        {
            lastSuccessPos = playerMove.transform.position;
            OnPlayerArrived?.Invoke();
        };
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

        // 4. 다리 ActionMap — GamePlay 상태에서만 활성화
        GameManager.Instance.AddGameStateEnterAction(GameManager.GameState.GamePlay,    () => StartCoroutine(EnableBridgeActionMapNextFrame()));
        GameManager.Instance.AddGameStateEnterAction(GameManager.GameState.WaitLoading, () => SetBridgeActionMap(false));
        GameManager.Instance.AddGameStateEnterAction(GameManager.GameState.GameReady,   () => SetBridgeActionMap(false));
        GameManager.Instance.AddGameStateEnterAction(GameManager.GameState.GameStop,    () => SetBridgeActionMap(false));
        GameManager.Instance.AddGameStateEnterAction(GameManager.GameState.GameOver,    () => SetBridgeActionMap(false));
        GameManager.Instance.AddGameStateEnterAction(GameManager.GameState.GameClear,   () => SetBridgeActionMap(false));
    }

    private System.Collections.IEnumerator EnableBridgeActionMapNextFrame()
    {
        yield return null; // Resume 버튼 터치가 끝날 때까지 대기
        var bridge = GameManager.Instance.BridgeManager.Spawner.CurrentBridge;
        if (bridge == null) yield break;
        bridge.GetComponent<UnityEngine.InputSystem.PlayerInput>().currentActionMap?.Enable();
    }

    private void SetBridgeActionMap(bool enabled)
    {
        var bridge = GameManager.Instance.BridgeManager.Spawner.CurrentBridge;
        if (bridge == null) return;
        var pi = bridge.GetComponent<UnityEngine.InputSystem.PlayerInput>();
        bridge.GetComponent<CubeRoad>().ResetGrow();
        pi.currentActionMap?.Disable();
    }


    // GameManager나 BridgeManager가 호출할 함수
    public void MovePlayer(Vector3 targetPos, bool isSuccess)
    {
        playerMove?.MoveToTarget(targetPos, moveSpeed, isSuccess);
    }

    public void Revive()
    {
        // 물리 복원 (velocity는 isKinematic 전에 초기화)
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.useGravity = false;

        var bridgeManager = GameManager.Instance.BridgeManager;
        bridgeManager.Revive();

        bridgeManager.Spawner.ReviveReset();

        // 위치 & 회전 즉시 복원 (마지막으로 발판에 도착했던 위치)
        playerMove.transform.SetPositionAndRotation(lastSuccessPos, Quaternion.identity);

        playerMove.OnArrival += OnReviveArrival;
        MovePlayer(lastSuccessPos, true);
    }

    private void OnReviveArrival()
    {
        playerMove.OnArrival -= OnReviveArrival;
        GameManager.Instance.SetGameState(GameManager.GameState.GamePlay);
    }
}
