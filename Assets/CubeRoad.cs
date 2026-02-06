using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public class CubeRoad : MonoBehaviour
{
    [SerializeField] private Transform bridgeMesh;
    [SerializeField] private float growSpeed = 5.0f;
    private Coroutine _growRoutine;
    private Transform _checkPoint;
    public Action<bool, Vector3> onBridgeResults;
    private bool _canGrow = true;

    private void Awake()
    {
        InitReferences();

    }
    private void Start()
    {
    }
    private void InitReferences()
    {
        var tip = GetComponentInChildren<BridgeTip>();

        if (tip != null)
        {
            _checkPoint = tip.transform;
        }
        else
        {
            Debug.LogError($"{gameObject.name}: 다리끝에 bridgeTip이 없음");
        }
    }
    private void OnEnable()
    {
        _canGrow = true;
    }

    public void OnNewAction(InputAction.CallbackContext context)
    {
        Debug.Log($"<color=cyan>입력 들어옴!</color> Phase: {context.phase}");
        if (GameManager.Instance.PlayerManager.IsMoving) return;
        if (!_canGrow) return;
        // 버튼 누르기 시작
        if (context.started)
        {

            if (_growRoutine != null) StopCoroutine(_growRoutine);
            _growRoutine = StartCoroutine(GrowRoutine());
        }
        // 손땟을때
        else if (context.canceled)
        {
            _canGrow = false;
            if (_growRoutine != null) StopCoroutine(_growRoutine);
            StartCoroutine(FallDownRoutine());
        }
    }

    private IEnumerator GrowRoutine()
    {
        while (true)
        {
            Vector3 newScale = bridgeMesh.localScale;
            newScale.y += growSpeed * Time.deltaTime;
            bridgeMesh.localScale = newScale;
            yield return null; // 다음 프레임까지 대기 (Update와 동일한 효과이나 필요할 때만 작동)
        }
    }
    private IEnumerator FallDownRoutine()
    {
        float targetAngle = 90f;
        float currentAngle = 0f;
        float fallSpeed = 250f;

        while (currentAngle < targetAngle)
        {
            float step = fallSpeed * Time.deltaTime;
            currentAngle += step;

            transform.rotation = Quaternion.Euler(currentAngle, 0, 0); //x 축 기준 회전
            yield return null;
        }
        transform.rotation = Quaternion.Euler(targetAngle, 0, 0);
        CheckSuccess();
    }
    private void CheckSuccess()
    {
        RaycastHit hit;

        if (Physics.Raycast(_checkPoint.position, Vector3.down, out hit, 2.0f))
        {
            Debug.Log("발판 성공!");
            onBridgeResults?.Invoke(true, hit.point);
        }
        else
        {
            Debug.Log("실패!");
            onBridgeResults?.Invoke(false, Vector3.zero);
        }
    }
}