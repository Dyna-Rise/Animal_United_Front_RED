using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; // コルーチンを使用するために追加

public class PlayerDash : MonoBehaviour
{
    //コンポーネント取得
    PlayerMove playerMove;
    CharacterController controller;

    [Header("ダッシュ力・持続時間・クールタイム")]
    public float dashSpeedMultiplier = 3.0f;
    public float dashDuration = 0.4f;
    public float dashCooldown = 0.5f;

    bool isDashing; //ダッシュフラグ
    float cooldownTimer; //クールダウン時間のカウント

    float lastMoveDirectionX = 1f;

    [Header("残像プレハブ")]
    public GameObject afterimagePrefab; // 作成した残像プレハブをアタッチする

    [Header("生成間隔・Z位置オフセット")]
    public float afterimageSpawnInterval = 0.05f;
    public Vector3 afterimageOffset = new Vector3(0, 0, 0.1f);

    Coroutine currentDashCoroutine; // ダッシュコルーチンを保持
    Coroutine afterimageCoroutine; // 残像生成コルーチン

    //ダッシュフラグのプロパティ（読み取り専用）
    public bool IsDashing
    {
        get { return isDashing; }
    }

    //ダッシュボタン
    void OnCrouch(InputValue value)
    {
        //ダッシュフラグがまだ立っておらず、クールダウンも済の場合
        if (value.isPressed && !isDashing && cooldownTimer <= 0)
        {
            currentDashCoroutine = StartCoroutine(StartDash());
        }
    }

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // あらたに移動していれば最後に記憶していた方向を更新
        if (playerMove != null && playerMove.inputDirection != 0)
        {
            lastMoveDirectionX = Mathf.Sign(playerMove.inputDirection);
        }

        // クールダウンタイマーの更新
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // ダッシュフラグが立っている時
        if (isDashing)
        {
            // ダッシュ中は強制的にダッシュ方向へ移動させる
            playerMove.SetDashDirection(lastMoveDirectionX);
        }
        else
        {
            // ダッシュ中でないときは通常の移動入力に戻す
            playerMove.ResetDashDirection();
        }
    }

    IEnumerator StartDash()
    {
        Dash();
        yield return new WaitForSeconds(dashDuration);
        EndDash();
    }

    //ダッシュ
    void Dash()
    {
        isDashing = true; //フラグを立てる
        cooldownTimer = dashCooldown; //クールダウン時間の設定

        playerMove.playerSpeed *= dashSpeedMultiplier; //プレイヤーの移動値を加工
        //playerMove.SetDashDirection(lastMoveDirectionX); 

        // 残像生成コルーチンを開始
        if (afterimagePrefab != null)
        {
            afterimageCoroutine = StartCoroutine(SpawnAfterimagesRoutine());
        }
    }

    //ダッシュ通常終了メソッド
    void EndDash()
    {
        isDashing = false; //ダッシュフラグ解除
        currentDashCoroutine = null; //ダッシュコルーチン参照の解除

        playerMove.ReturnSpeed(); //プレイヤーの移動値をもとに戻す
        playerMove.ResetDashDirection(); //通常の移動入力に戻す

        // 残像生成コルーチンを停止
        if (afterimageCoroutine != null)
        {
            StopCoroutine(afterimageCoroutine);
            afterimageCoroutine = null;　//生成コルーチン参照の解除
        }
    }

    //ダッシュ中断メソッド
    public void StopDash()
    {
        isDashing = false; //ダッシュフラグ解除
        playerMove.ReturnSpeed(); //プレイヤーの移動値をもとに戻す
        playerMove.ResetDashDirection(); //通常の移動入力に戻す

        if (afterimageCoroutine != null)
        {
            StopCoroutine(afterimageCoroutine);
            afterimageCoroutine = null;　//生成コルーチン参照の解除
        }

        //中断処理
        if (currentDashCoroutine != null)
        {
            StopCoroutine(currentDashCoroutine); 
            currentDashCoroutine = null; //ダッシュコルーチン参照の解除
        }
    }

    // 残像を定期的に生成するコルーチン
    IEnumerator SpawnAfterimagesRoutine()
    {
        while (isDashing) // ダッシュ中のみ
        {
            SpawnAfterimage(); //生成メソッド
            yield return new WaitForSeconds(afterimageSpawnInterval); //間隔を空ける
        }
    }

    // 残像生成メソッド
    void SpawnAfterimage()
    {
        // 現在のキャラクターの位置(+ずらし位置)と回転を取得
        Vector3 spawnPosition = transform.position + afterimageOffset;
        Quaternion spawnRotation = transform.rotation;

        // 残像プレハブを生成
        GameObject afterimage = Instantiate(
            afterimagePrefab,
            spawnPosition,
            spawnRotation
            );

        // 残像の向きを合わせる
        afterimage.transform.localScale = transform.localScale;
    }
}