using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private bool _isMoving = false;
    public bool IsMoving => _isMoving;
    public System.Action OnArrival;

    public void MoveToTarget(Vector3 targetPos, float speed, bool isSuccess)
    {
        if (_isMoving) return;
        StartCoroutine(MoveRoutine(targetPos, speed, isSuccess));
    }

    private IEnumerator MoveRoutine(Vector3 targetPos, float speed, bool isSuccess)
    {
        _isMoving = true;

        Vector3 finalTarget = new Vector3(targetPos.x, transform.position.y, targetPos.z);
        if (isSuccess)
        {
            // 성공 시 목적지(바닥 중앙)까지 거리 체크하며 이동
            while (Vector3.Distance(transform.position, finalTarget) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, finalTarget, speed * Time.deltaTime);
                yield return null;
            }
            transform.position = finalTarget; // 위치 보정
            _isMoving = false; // 여기서 딱 멈춤!

            Debug.Log("성공: 발판 중앙 도착");
            GameManager.Instance.BridgeManager.Spawner.ReleasePreviousSet();
            OnArrival?.Invoke();
        }
        else
        {
            // 실패 시 목적지 따위 무시하고 앞으로 계속 직진
            Debug.Log("실패: 멈추지 않고 직진 시작");
            while (_isMoving)
            {
                // finalTarget 방향으로 계속 전진 (멈춤 조건 없음!)
                transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, speed * Time.deltaTime);
                yield return null;
            }
            // _isMoving은 FallZone 트리거에서 false로 만들어줌!
        }
    }
    public void StopMoving()
    {
        _isMoving = false; 
    }
}
