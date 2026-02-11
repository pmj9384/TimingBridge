using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallzone : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {

        if (GameManager.Instance.BridgeManager.CanBuild)
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            Debug.Log("FallZone: 플레이어 낙하 지점 도착! 게임 오버 처리");
            // 3. 게임 오버 상태로 변경
            StartCoroutine(GameOverSequence(other.gameObject));
        }
    }
    private IEnumerator GameOverSequence(GameObject player)
    {
        Debug.Log("추락 시작!");
        GameManager.Instance.SetGameState(GameManager.GameState.GameOver);
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.SetGameState(GameManager.GameState.GameStop);
    }
}
