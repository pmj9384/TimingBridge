using UnityEngine;

/// <summary>
/// 오션 타일 3개를 순환시켜 플레이어를 따라오게 합니다.
/// 사용법: 빈 오브젝트에 이 스크립트 부착 후 oceanTilePrefab에 Ocean 프리팹 연결.
/// </summary>
public class OceanTileManager : MonoBehaviour
{
    [SerializeField] private GameObject oceanTilePrefab;
    [SerializeField] private float tileSize = 750f;

    private Transform _player;
    private GameObject[] _tiles;
    private float _fixedY;
    private int _backIndex;

    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;

        _fixedY = -6.05f;
        _backIndex = 0;

        _tiles = new GameObject[3];
        float startZ = _player != null
            ? Mathf.Floor(_player.position.z / tileSize) * tileSize
            : 0f;

        for (int i = 0; i < 3; i++)
        {
            float tileZ = startZ + (i - 1) * tileSize;
            _tiles[i] = Instantiate(oceanTilePrefab, new Vector3(0f, _fixedY, tileZ), Quaternion.identity);
        }
    }

    private void LateUpdate()
    {
        if (_player == null) return;

        float backTileZ = _tiles[_backIndex].transform.position.z;

        // 플레이어가 뒤 타일의 앞쪽 경계를 넘으면 그 타일을 맨 앞으로 이동
        if (_player.position.z > backTileZ + tileSize)
        {
            int frontIndex = (_backIndex + 2) % 3;
            float newZ = _tiles[frontIndex].transform.position.z + tileSize;
            _tiles[_backIndex].transform.position = new Vector3(0f, _fixedY, newZ);
            _backIndex = (_backIndex + 1) % 3;
        }
    }
}
