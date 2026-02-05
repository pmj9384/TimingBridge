using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private bool _isMoving = false;
    public bool IsMoving => _isMoving;
    public System.Action OnArrival;

    public void MoveToTarget(Vector3 targetPos, float speed)
    {
        if (_isMoving) return;
        StartCoroutine(MoveRoutine(targetPos, speed));
    }

    private IEnumerator MoveRoutine(Vector3 targetPos, float speed)
    {
        _isMoving = true;

        Vector3 finalTarget = new Vector3(targetPos.x, transform.position.y, targetPos.z);

        while (Vector3.Distance(transform.position, finalTarget) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, finalTarget, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = finalTarget;
        _isMoving = false;

        Debug.Log("플레이어 도착");
        GameManager.Instance.BridgeManager.Spawner.ReleasePreviousSet();
        OnArrival?.Invoke();
    }
}
