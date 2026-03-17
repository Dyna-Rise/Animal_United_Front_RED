using UnityEngine;

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
    public static int playerLife = 5;
    public static GameState gameState;

    void Start()
    {
        gameState = GameState.stageplay;
    }

    void Update()
    {
        if(gameState == GameState.gameover)
        {
            //ゲームオーバー
            Debug.Log("ゲームオーバー");
        }

        if(gameState == GameState.stageclear)
        {
            //ステージクリア
            Debug.Log("ステージクリア");
        }

    }
}
