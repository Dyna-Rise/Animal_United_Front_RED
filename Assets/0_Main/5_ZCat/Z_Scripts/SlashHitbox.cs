using UnityEngine;

public class SlashHitbox : MonoBehaviour
{
    public float damage = 10f; // この攻撃のダメージ量

    void OnTriggerEnter2D(Collider2D other) // 2Dゲームの場合
    // void OnTriggerEnter(Collider other) // 3Dゲームの場合
    {
        if (other.CompareTag("Enemy"))
        {
            // 例: 敵にダメージを与える処理を呼び出す
            // EnemyスクリプトにTakeDamageメソッドがあると仮定
            // Enemy enemy = other.GetComponent<Enemy>();
            // if (enemy != null)
            // {
            //     enemy.TakeDamage(damage);
            // }

            Debug.Log($"Enemyに接触！ ダメージ: {damage}");

            // ヒットエフェクトの生成、SEの再生など
            // Destroy(gameObject); // 一度ヒットしたら当たり判定を消す場合
        }
    }

    // 攻撃が終了したときに当たり判定を無効にする処理もここで制御できます。
    // 例:
    // public void DisableHitbox()
    // {
    //     Collider2D col = GetComponent<Collider2D>(); // 2Dの場合
    //     if (col != null) col.enabled = false;
    // }
}