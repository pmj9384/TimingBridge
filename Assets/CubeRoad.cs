using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CubeRoad : MonoBehaviour
{
    [SerializeField] private Transform bridgeMesh;
    [SerializeField] private float growSpeed = 5.0f;

    private Coroutine _growRoutine;

    public void OnNewAction(InputAction.CallbackContext context)
    {
        // 버튼 시ㄱ
        if (context.started)
        {
            if (_growRoutine != null) StopCoroutine(_growRoutine);
            _growRoutine = StartCoroutine(GrowRoutine());
        }
        // 손땟을때
        else if (context.canceled)
        {
            if (_growRoutine != null) StopCoroutine(_growRoutine);
            // 여기서 나중에 FallDown() 같은 쓰러지는 함수를 호출
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
}