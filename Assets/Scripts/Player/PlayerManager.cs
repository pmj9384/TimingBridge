using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerManager : InGameManager
{
    [SerializeField] private float moveSpeed = 5.0f; // 인스펙터에서 수정 가능
    public System.Action OnPlayerArrived;
    private PlayerMove playerMove;
    private Rigidbody rb;
    private Vector3 lastSuccessPos;
    private CinemachineVirtualCamera vcam;
    public bool IsMoving => playerMove != null && playerMove.IsMoving;
    public override void Initialize()
    {
        base.Initialize();
        GameObject playerObj = GameObject.FindWithTag("Player");
        playerMove = playerObj.GetComponent<PlayerMove>();
        rb = playerObj.GetComponent<Rigidbody>();
        lastSuccessPos = playerObj.transform.position;
        vcam = GameObject.Find("Virtual Camera")?.GetComponent<CinemachineVirtualCamera>();

        playerMove.OnArrival += () =>
        {
            lastSuccessPos = playerMove.transform.position;
            OnPlayerArrived?.Invoke();
        };
        playerMove.OnFailureArrival += () => GameManager.Instance.SetGameState(GameManager.GameState.GameOver);

        // 3. 게임오버 시 물리 작동 액션 등록
        GameManager.Instance.AddGameStateEnterAction(GameManager.GameState.GameOver, StartFallSequence);

        // 4. 다리 ActionMap — GamePlay 상태에서만 활성화
        GameManager.Instance.AddGameStateEnterAction(GameManager.GameState.GamePlay,    () => StartCoroutine(EnableBridgeActionMapNextFrame()));
        GameManager.Instance.AddGameStateEnterAction(GameManager.GameState.WaitLoading, () => SetBridgeActionMap(false));
        GameManager.Instance.AddGameStateEnterAction(GameManager.GameState.GameReady,   () => SetBridgeActionMap(false));
        GameManager.Instance.AddGameStateEnterAction(GameManager.GameState.GameStop,    () => SetBridgeActionMap(false));
        GameManager.Instance.AddGameStateEnterAction(GameManager.GameState.GameOver,    () => SetBridgeActionMap(resetBridge: false));
        GameManager.Instance.AddGameStateEnterAction(GameManager.GameState.GameClear,   () => SetBridgeActionMap(false));
    }

    private void StartFallSequence()
    {
        playerMove.StopMoving();

        // 카메라 추적 해제 (플레이어 회전에 카메라가 딸려가지 않도록)
        if (vcam != null) vcam.Follow = null;

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.velocity = playerMove.transform.forward * 2.5f + Vector3.up * 1.5f;
        rb.angularVelocity = new Vector3(
            UnityEngine.Random.Range(3f, 5f),
            UnityEngine.Random.Range(-2f, 2f),
            UnityEngine.Random.Range(-1f, 1f)
        );
    }

    private System.Collections.IEnumerator EnableBridgeActionMapNextFrame()
    {
        // 터치/클릭이 완전히 끝날 때까지 대기 (광고 닫기, Resume 버튼 입력 블리드 방지)
        yield return new WaitUntil(() =>
            (UnityEngine.InputSystem.Touchscreen.current == null || !UnityEngine.InputSystem.Touchscreen.current.primaryTouch.isInProgress) &&
            (UnityEngine.InputSystem.Mouse.current == null || !UnityEngine.InputSystem.Mouse.current.leftButton.isPressed)
        );
        var bridge = GameManager.Instance.BridgeManager.Spawner.CurrentBridge;
        if (bridge == null) yield break;
        bridge.GetComponent<UnityEngine.InputSystem.PlayerInput>().currentActionMap?.Enable();
    }

    private void SetBridgeActionMap(bool resetBridge = true)
    {
        var bridge = GameManager.Instance.BridgeManager.Spawner.CurrentBridge;
        if (bridge == null) return;
        var pi = bridge.GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (resetBridge) bridge.GetComponent<CubeRoad>().ResetGrow();
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

        // 카메라 재연결
        if (vcam != null) vcam.Follow = playerMove.transform;

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
