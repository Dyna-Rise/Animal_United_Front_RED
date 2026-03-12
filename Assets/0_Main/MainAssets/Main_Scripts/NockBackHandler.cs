using UnityEngine;
using System.Collections; // コルーチンを使用するために追加

public class KnockbackHandler : MonoBehaviour
{
    // コンポーネント参照
    PlayerMove playerMove;
    PlayerDash playerDash;
    CharacterController controller;

    [Header("ノックバック力・リアクション時間・硬直時間")]
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.3f;

    Vector3 knockbackDirection;   // ノックバック方向
    Coroutine knockBackCoroutine;   // ノックバックコルーチン

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        playerDash = GetComponent<PlayerDash>();
        controller = GetComponent<CharacterController>();
    }

    void OnTriggerEnter(Collider other)
    {
        //フラグがない時、Enemyに触れたら
        if (other.CompareTag("Enemy") && knockBackCoroutine == null)
        {
            // 敵との接触位置からノックバック方向を計算
            knockbackDirection = (transform.position - other.transform.position).normalized;

            // Y軸方向のノックバックも考慮
            knockbackDirection.y = 0.5f;
            knockbackDirection.Normalize(); // 再び正規化

            StartKnockback();　//ノックバックの開始
        }
    }

    void Update()
    {
        // ノックバックコルーチンがある時
        if (knockBackCoroutine != null)
        {
            Vector3 currentMove = playerMove.MoveDirection; // PlayerMoveの現在の移動方向を取得

            // ノックバック方向にノックバックの力を乗せておく
            Vector3 finalKnockbackVelocity = knockbackDirection * knockbackForce;

            controller.Move((currentMove + finalKnockbackVelocity) * Time.deltaTime);
        }
    }

    //ノックバック
    void StartKnockback()
    {

        playerMove.DisableMoveInput();
        playerDash.StopDash();

        knockBackCoroutine = StartCoroutine(KnockbackRoutine()); //ノックバックの開始
    }

    IEnumerator KnockbackRoutine()
    {
        //float timer = 0f;
        //while (timer < knockbackDuration)
        //{
        //    timer += Time.deltaTime;
        //    yield return null;
        //}
        yield return new WaitForSeconds(knockbackDuration);
        playerMove.EnableMoveInput();
        knockBackCoroutine = null;
    }

}