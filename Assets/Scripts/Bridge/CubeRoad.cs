using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public class CubeRoad : MonoBehaviour
{
    [SerializeField] private Transform bridgeMesh;
    [SerializeField] private float growSpeed = 5.0f;
    private float maxScale = 7.0f;
    private float minScale = 0.1f;
    private bool isShrinking = false;
    private Coroutine _growRoutine;
    private Transform _checkPoint;

    public Action<bool, Vector3, bool> onBridgeResults;
    public Func<bool> isPlayerMoving;
    private bool _canGrow = true;
    private Rigidbody _rb;

    private void Awake()
    {
        InitReferences();
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _canGrow = true;
        isShrinking = false;
    }

    private void OnDisable()
    {
        if (_rb != null)
        {
            _rb.isKinematic = true;
            _rb.useGravity = false;
        }
    }

    public void OnGameOver()
    {
        if (_rb != null)
        {
            foreach (var col in GetComponentsInChildren<Collider>())
                col.enabled = false;
            _rb.isKinematic = false;
            _rb.useGravity = true;
        }
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

    public void OnNewAction(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.GamePlay) return;
        if (isPlayerMoving?.Invoke() == true) return;
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
            yield return null; // 다음 프레임까지 대기 (Update와 동일한 효과이나 필요할 때만 작동)

            if (!isShrinking)
            {
                newScale.y += growSpeed * Time.deltaTime;
                // 최대치 도달하면 방향 전환
                if (newScale.y >= maxScale) isShrinking = true;
            }
            else
            {
                newScale.y -= growSpeed * Time.deltaTime;
                // 최소치 도달하면 방향 전환
                if (newScale.y <= minScale) isShrinking = false;
            }

            bridgeMesh.localScale = newScale;
            yield return null;
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
        SoundManager.Instance.PlaySfx(SfxClipId.BridgeFall);
        yield return new WaitForEndOfFrame();
        CheckSuccess();
    }
    private void CheckSuccess()
    {
        int platformLayer = LayerMask.GetMask("Platform");
        Vector3 startPos = _checkPoint.position + Vector3.up * 0.1f;

        Debug.DrawRay(startPos, Vector3.down * 5.0f, Color.red, 5.0f);
        RaycastHit hit;
        if (Physics.Raycast(startPos, Vector3.down, out hit, 5.0f, platformLayer))
        {
            float dist = Mathf.Abs(hit.point.z - hit.collider.bounds.center.z);
            float criticalZone = hit.collider.bounds.extents.z * 0.25f;
            bool isCritical = dist < criticalZone;
            Debug.Log($"발판 성공! isCritical={isCritical}, dist={dist:F2}");
            onBridgeResults?.Invoke(true, hit.point, isCritical);
        }
        else
        {
            Debug.Log("실패!");
            onBridgeResults?.Invoke(false, _checkPoint.position, false);
        }
    }
}
