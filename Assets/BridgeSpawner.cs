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
        // 2. 게임 시작 시 첫 번째 다리 생성 (플레이어 발밑)
        SpawnNextBridge(transform.position, true);
        Debug.Log("브릿지 생성");
    }

    public void SpawnNextBridge(Vector3 currentPlatformPos, bool isFirst = false)
    {
        // 2. [계보 이동] 지금 쓰고 있던 걸 '이전 것'으로 보관 (이제 새 걸 받을 준비)
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

            // [유니버설 기술] 랜덤 거리를 통해 게임의 난이도를 동적으로 조절
            float randomDist = Random.Range(4f, 8f);

            Vector3 nextPos = new Vector3(currentPlatformPos.x, 0f, currentPlatformPos.z + 15f);
            nextPlatform.transform.position = nextPos;
            nextPlatform.transform.rotation = Quaternion.identity;
            currentPlatform = nextPlatform;
        }
        var cubeRoad = bridgeObj.GetComponent<CubeRoad>();
        cubeRoad.onBridgeResults = GameManager.Instance.BridgeManager.OnBridgeResult;
    }

    // 다리가 화면 뒤로 사라지거나 실패했을 때 호출
    public void DespawnBridge(GameObject bridge)
    {
        bridgePool.Release(bridge);
    }
    public void ReleasePreviousSet()
    {
        // 1. [정리] 새로운 걸 만들기 전에, 아주 오래된(이전 단계) 다리와 발판은 풀로 반환
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
}