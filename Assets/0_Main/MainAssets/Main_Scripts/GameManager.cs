using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    none,
    stageplay,
    gameover,
    stageclear,
    gameclear
}

public class GameManager : MonoBehaviour
{
    [Header("体力最大値")]
    public int maxLife = 5;
    public static int playerLife;
    public static GameState gameState;

    [Header("ネクストステージ")]
    public string sceneName;

    void Start()
    {
        playerLife = maxLife;
        gameState = GameState.stageplay;
    }

    void Update()
    {
        if(gameState == GameState.gameover)
        {
            //ゲームオーバー
            Debug.Log("ゲームオーバー");
        }

        if(gameState == GameState.gameclear)
        {
            //ステージクリア
            Debug.Log("ゲームクリア");
            StartCoroutine(ToEnding());
        }
    }

    IEnumerator ToEnding()
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("Ending");
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Next()
    {
        SceneManager.LoadScene(sceneName);
    }
}
