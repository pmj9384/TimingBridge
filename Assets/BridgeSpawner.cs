using UnityEngine;
using UnityEngine.Pool;

public class BridgeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bridgePrefab;
    private ObjectPool<GameObject> _pool;

    // 현재 활성화된 다리를 추적 (다음 다리 위치 계산용)
    private GameObject _currentBridge;

    public void InitializeSpawner()
    {
        Debug.Log("브릿지 생성");
        // 1 오브젝트 풀 생성 및 규칙 정의
        _pool = GameManager.Instance.ObjectPool.CreateObjectPool(
            bridgePrefab,
            createFunc: () => Instantiate(bridgePrefab),
            onGet: (obj) => obj.SetActive(true),
            onRelease: (obj) => obj.SetActive(false)
        );

        // 2. 게임 시작 시 첫 번째 다리 생성 (플레이어 발밑)
        SpawnNextBridge(transform.position, true);
        Debug.Log("브릿지 생성");
    }

    public void SpawnNextBridge(Vector3 lastPosition, bool isFirst = false)
    {
        // 3. 풀에서 다리 꺼내기
        GameObject bridgeObj = _pool.Get();
        _currentBridge = bridgeObj;

        // 4. 위치 계산
        if (isFirst)
        {
            bridgeObj.transform.position = lastPosition;
        }
        else
        {
            // 랜덤 거리 주기 
            float randomDist = Random.Range(4f, 8f);
            bridgeObj.transform.position = lastPosition + new Vector3(randomDist, 0, 0);
        }

        bridgeObj.transform.rotation = Quaternion.identity;

        // 5. 다리의 액션을 매니저의 결과 보고 함수와 연결
        var cubeRoad = bridgeObj.GetComponent<CubeRoad>();
        cubeRoad.onBridgeResults = GameManager.Instance.BridgeManager.OnBridgeResult;
    }

    // 다리가 화면 뒤로 사라지거나 실패했을 때 호출
    public void DespawnBridge(GameObject bridge)
    {
        _pool.Release(bridge);
    }
}