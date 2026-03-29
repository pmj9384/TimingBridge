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

    public void InitializeSpawner()
    {
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
        startPlatform.transform.position = new Vector3(0f, -1.09f, 0f);
        startPlatform.transform.rotation = Quaternion.identity;

        currentPlatform = startPlatform;
    }

    public void SpawnNextBridge(Vector3 currentPlatformPos, float growSpeed, float platformZScale, bool isFirst = false)
    {
        oldPlatform    = previousPlatform;
        previousBridge = currentBridge;
        previousPlatform = currentPlatform;

        GameObject bridgeObj = bridgePool.Get();
        bridgeObj.transform.position   = new Vector3(currentPlatformPos.x, 0.25f, currentPlatformPos.z);
        bridgeObj.transform.rotation   = Quaternion.identity;
        bridgeObj.transform.localScale = Vector3.one;
        bridgeObj.GetComponentInChildren<MeshRenderer>().enabled = false;
        currentBridge = bridgeObj;

        if (!isFirst)
        {
            GameObject nextPlatform = platformPool.Get();
            float randomDist = Random.Range(10f, 20f);
            nextPlatform.transform.position   = new Vector3(currentPlatformPos.x, -1.09f, currentPlatformPos.z + randomDist);
            nextPlatform.transform.rotation   = Quaternion.identity;
            Vector3 prefabScale = platformPrefab.transform.localScale;
            nextPlatform.transform.localScale = new Vector3(prefabScale.x, prefabScale.y, platformZScale);
            currentPlatform = nextPlatform;
        }

        var cubeRoad = bridgeObj.GetComponent<CubeRoad>();
        cubeRoad.onBridgeResults = GameManager.Instance.BridgeManager.OnBridgeResult;
        cubeRoad.isPlayerMoving  = () => GameManager.Instance.PlayerManager.IsMoving;
        cubeRoad.SetGrowSpeed(growSpeed);
    }

    public void ShowCurrentBridge()
    {
        if (currentBridge != null)
            currentBridge.GetComponentInChildren<MeshRenderer>().enabled = true;
    }

    public void DespawnBridge(GameObject bridge)
    {
        bridgePool.Release(bridge);
    }

    public void ReleasePreviousSet()
    {
        if (previousBridge != null)
        {
            bridgePool.Release(previousBridge);
            previousBridge = null;
        }
        if (oldPlatform != null)
        {
            platformPool.Release(oldPlatform);
            oldPlatform = null;
        }
    }

    public Vector3 GetCurrentPlatformPos()
    {
        return currentPlatform.transform.position;
    }

    public Vector3 ReviveReset()
    {
        if (currentBridge != null)   { bridgePool.Release(currentBridge);    currentBridge = null; }
        if (currentPlatform != null) { platformPool.Release(currentPlatform); currentPlatform = null; }

        Vector3 revivePos = previousPlatform.transform.position;

        currentPlatform  = previousPlatform;
        previousPlatform = null;
        previousBridge   = null;
        oldPlatform      = null;

        return revivePos;
    }
}
