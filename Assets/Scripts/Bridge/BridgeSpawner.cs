using UnityEngine;
using UnityEngine.Pool;

public class BridgeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bridgePrefab;
    [SerializeField] private GameObject platformPrefab;
    private ObjectPool<GameObject> bridgePool;
    private ObjectPool<GameObject> platformPool;

    private GameObject oldPlatform;
    private GameObject currentBridge;
    private GameObject previousBridge;
    private GameObject currentPlatform;
    private GameObject previousPlatform;
    public GameObject CurrentBridge => currentBridge;

    // 현재 활성화된 다리를 추적 (다음 다리 위치 계산용)

    public void InitializeSpawner()
    {

        // 1 오브젝트 풀 생성 및 규칙 정의
        bridgePool = GameManager.Instance.ObjectPool.CreateObjectPool(
        bridgePrefab,
        createFunc: () => Instantiate(bridgePrefab),
        onGet: (obj) => obj.SetActive(true),
        onRelease: (obj) => obj.SetActive(false)
    );
        platformPool = GameManager.Instance.ObjectPool.CreateObjectPool(
        platformPrefab,
        createFunc: () => Instantiate(platformPrefab),
        onGet: (obj) => obj.SetActive(true),
        onRelease: (obj) => obj.SetActive(false)
   );
        GameObject startPlatform = platformPool.Get();
        startPlatform.transform.position = Vector3.zero;
        startPlatform.transform.rotation = Quaternion.identity;

        // 얘를 'current'로 일단 둬서 SpawnNextBridge가 얘를 previous로 밀어냄
        currentPlatform = startPlatform;
    }

    public void SpawnNextBridge(Vector3 currentPlatformPos, bool isFirst = false)
    {
        // 2. 지금 쓰고 있던 걸 '이전 것'으로 보관 (이제 새 걸 받을 준비)
        oldPlatform = previousPlatform;
        previousBridge = currentBridge;
        previousPlatform = currentPlatform;
        // 3. 풀에서 다리 꺼내기
        GameObject bridgeObj = bridgePool.Get();
        bridgeObj.transform.position = new Vector3(currentPlatformPos.x, 0.25f, currentPlatformPos.z);
        bridgeObj.transform.rotation = Quaternion.identity;
        bridgeObj.transform.localScale = Vector3.one;

        currentBridge = bridgeObj;
        // 4. 위치 계산
        if (!isFirst)
        {
            GameObject nextPlatform = platformPool.Get();

            float randomDist = Random.Range(10f, 20f);

            Vector3 nextPos = new Vector3(currentPlatformPos.x, 0f, currentPlatformPos.z + randomDist);
            nextPlatform.transform.position = nextPos;
            nextPlatform.transform.rotation = Quaternion.identity;
            currentPlatform = nextPlatform;
        }
        var cubeRoad = bridgeObj.GetComponent<CubeRoad>();
        cubeRoad.onBridgeResults = GameManager.Instance.BridgeManager.OnBridgeResult;
        cubeRoad.isPlayerMoving = () => GameManager.Instance.PlayerManager.IsMoving;
    }

    // 다리가 화면 뒤로 사라지거나 실패했을 때 호출
    public void DespawnBridge(GameObject bridge)
    {
        bridgePool.Release(bridge);
    }
    public void ReleasePreviousSet()
    {
        // 1. 새로운 걸 만들기 전에, 아주 오래된다리와 발판은 풀로 반환
        if (previousBridge != null)
        {
            bridgePool.Release(previousBridge);
            previousBridge = null; // 중복 반환 방지
        }

        if (oldPlatform != null)
        {
            platformPool.Release(oldPlatform);
            oldPlatform = null;
        }

    }
    public Vector3 GetCurrentPlatformPos()
    {
        // 생성된 플랫폼의 중앙 위치(Pivot)를 반환
        return currentPlatform.transform.position;
    }
}
