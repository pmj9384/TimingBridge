using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private bool _isMoving = false;
    public bool IsMoving => _isMoving;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }
    public System.Action OnArrival;
    public System.Action OnFailureArrival;

    public void MoveToTarget(Vector3 targetPos, float speed, bool isSuccess)
    {
        if (_isMoving) return;
        StartCoroutine(MoveRoutine(targetPos, speed, isSuccess));
    }

    private IEnumerator MoveRoutine(Vector3 targetPos, float speed, bool isSuccess)
    {
        _isMoving = true;
        _animator?.SetTrigger("Walk");
        SoundManager.Instance.PlaySfxLoop(SfxClipId.Footstep);

        Vector3 finalTarget = new Vector3(targetPos.x, transform.position.y, targetPos.z);
        if (isSuccess)
        {
            // 성공 시 목적지(바닥 중앙)까지 거리 체크하며 이동
            while (Vector3.Distance(transform.position, finalTarget) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, finalTarget, speed * Time.deltaTime);
                yield return null;
            }
            transform.position = finalTarget;
            SoundManager.Instance.StopSfxLoop();
            _isMoving = false;
            _animator?.SetTrigger("Idle");

            Debug.Log("성공: 발판 중앙 도착");
            OnArrival?.Invoke();
        }
        else
        {
            // 실패 시 다리 끝까지 이동 후 정지
            while (Vector3.Distance(transform.position, finalTarget) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, finalTarget, speed * Time.deltaTime);
                yield return null;
            }
            transform.position = finalTarget;
            SoundManager.Instance.StopSfxLoop();
            _isMoving = false;
            _animator?.SetTrigger("Idle");
            OnFailureArrival?.Invoke();
        }
    }
    public void StopMoving()
    {
        _isMoving = false; 
    }
}
