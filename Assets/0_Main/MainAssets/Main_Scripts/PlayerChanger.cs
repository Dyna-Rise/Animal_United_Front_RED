using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Listを使用するために追加

public class PlayerChanger : MonoBehaviour
{
    [Header("制限時間")]
    public float player2TimeMax = 15.0f; // Player1の最大変身時間
    public float player3TimeMax = 15.0f; // Player2の最大変身時間

    [Header("デフォルトキャラ")]
    public GameObject defaultPlayer;

    [Header("チェンジキャラ")]
    public GameObject[] changePlayers; // インスペクターで設定する変更可能なプレハブ

    [Header("切り替えエフェクト")]
    public GameObject changeEffect;

    PlayerFollow playerFollow; //プレイヤーの位置追従

    // 変身可能時間の現在値
    float player2CurrentTime;
    float player3CurrentTime;

    GameObject player1, player2, player3;
    public bool isPlayer2, isPlayer3; //変身フラグ

    Coroutine gameOverCoroutine;

    public float Player2CurrentTime
    {
        get { return player2CurrentTime; }
    }

    public float Player3CurrentTime
    {
        get { return player3CurrentTime; }
    }


    void Awake()
    {
        playerFollow = GetComponent<PlayerFollow>();

        player1 = Instantiate(
            defaultPlayer,
            playerFollow.transform.position,
            Quaternion.identity);

        // 変身可能時間を最大値で初期化
        player2CurrentTime = player2TimeMax;
        player3CurrentTime = player3TimeMax;
    }

    public void Player2Change()
    {
        if (player2CurrentTime > 0)
        {
            if (player1 != null) Destroy(player1);
            if (player3 != null) Destroy(player3);
            isPlayer3 = false;
            player2 = Instantiate(
                changePlayers[0],
                playerFollow.transform.position,
                Quaternion.identity);
            //エフェクトの発生
            Instantiate(
                changeEffect,
                playerFollow.transform.position + new Vector3(0, 0, -1),
                Quaternion.identity);
            StartCoroutine(PlayerFollowRest());
            isPlayer2 = true;
        }
    }
    public void Player3Change()
    {
        if (player3CurrentTime > 0)
        {
            if (player1 != null) Destroy(player1);
            if (player2 != null) Destroy(player2);
            isPlayer2 = false;
            player3 = Instantiate(
                changePlayers[1],
                playerFollow.transform.position,
                Quaternion.identity);
            //エフェクトの発生
            Instantiate(
                changeEffect,
                playerFollow.transform.position + new Vector3(0, 0, -1),
                Quaternion.identity);
            StartCoroutine(PlayerFollowRest());
            isPlayer3 = true;
        }
    }

    public void DefaultPlayerChange()
    {
        if(player2 != null)Destroy(player2);
        if(player3 != null)Destroy(player3);
        player1 = Instantiate(
            defaultPlayer,
            playerFollow.transform.position,
            Quaternion.identity);
        //エフェクトの発生
        Instantiate(
            changeEffect,
            playerFollow.transform.position + new Vector3(0, 0, -1),
            Quaternion.identity);
        StartCoroutine(PlayerFollowRest());
        isPlayer2 = false;
        isPlayer3 = false;
    }

    IEnumerator PlayerFollowRest()
    {
        yield return new WaitForSeconds(0.1f);
        playerFollow.GetComponent<PlayerFollow>().TargetReset(); //取りなおし
    }

    void Update()
    {
        //Player2切り替え中
        if (isPlayer2)
        {
            player2CurrentTime -= Time.deltaTime;
            if (player2CurrentTime <= 0)
            {
                player2CurrentTime = 0;
                DefaultPlayerChange();
            }
        }//非切り替え時、時間の回復
        else if (!isPlayer2 && player2CurrentTime < player2TimeMax)
        {
            player2CurrentTime += Time.deltaTime;
            if (player2CurrentTime >= player2TimeMax)
            {
                player2CurrentTime = player2TimeMax;
            }
        }

        //Player3切り替え中
        if (isPlayer3)
        {
            player3CurrentTime -= Time.deltaTime;
            if (player3CurrentTime <= 0)
            {
                player3CurrentTime = 0;
                DefaultPlayerChange();
            }
        }//非切り替え時、時間の回復
        else if (!isPlayer3 && player3CurrentTime < player3TimeMax)
        {
            player3CurrentTime += Time.deltaTime;
            if (player3CurrentTime >= player3TimeMax)
            {
                player3CurrentTime = player3TimeMax;
            }
        }

        if(GameManager.gameState == GameState.gameover)
        {
            if(gameOverCoroutine == null)
            {
                gameOverCoroutine = StartCoroutine(GameOver());
            }
        }
    }

   IEnumerator GameOver()
   {
        GameObject obj;
        if(isPlayer2)
        {
            obj = player2;
        }
        else if (isPlayer3)
        {
            obj = player3;
        }
        else
        {
            obj = player1;
        }

        obj.GetComponent<SphereCollider>().enabled = false;
        obj.GetComponent<PlayerMove>().SetMoveDirectionX(0);
        obj.GetComponent<PlayerMove>().SetMoveDirectionY(0);

        yield return new WaitForSeconds(1.0f);
        Destroy(obj);
   }
}
