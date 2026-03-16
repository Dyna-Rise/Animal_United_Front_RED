using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BossController : MonoBehaviour
{
    public int hp = 10;     // ボスの体力
    public float attackRange = 10.0f;  // 攻撃してくる距離
    public float moveSpeed = 2.0f;   // Lerpの係数
    public float distance;         // PlayerとBossの距離
    public Vector2 moveRangeX = new Vector2(-5f, 5f); // 出現するX範囲
    private float waitTimeAfterAttack = 1.0f; // 攻撃後の隙
    // 横移動のX軸の限界
    const int MinLane = -2;
    const int MaxLane = 2;
    const float LaneWidth = 2.0f;

    // 落下ポイントのX座標リスト（例：5箇所）
    float[] dropPoints = { -4f, -2f, 0f, 2f, 4f };

    public float dropSpeed;   // Bossが落ちてくる速さ

    public GameObject bossSlashPrefab;          // 近接攻撃のオブジェクト
    public GameObject bossShotPrefab;           // 遠距離攻撃のオブジェクト

    public float bossShotSpeed = 5.0f;         // 弾のスピード

    GameObject player;
    public bool offScreenFlag = true;       // 画面外フラグ
    bool inDamage;              // ダメージ中管理フラグ
    Coroutine runawayCoroutine; // 逃亡コルーチン
    Coroutine surpriseAttackCoroutine;  // 急襲コルーチン
    private Vector3 targetPosition;
    private Vector3 startPos; // 画面外の待機位置

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        startPos = transform.position;
        targetPosition = startPos;
    }

    // Update is called once per frame
    void Update()
    {
        // ダメージ中は点滅処理
        if (inDamage)
        {
            float val = Mathf.Sin(Time.time * 50);
            if (val > 0)
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
        // 体力が残っている場合
        if (hp > 0)
        {
            if (player != null)
            {
                Vector3 playerPos = player.transform.position;  // その時のPlayerの位置
                distance = Vector3.Distance(transform.position, playerPos);   // PlayerとBossの距離の差

                // ①外フラグがON かつ 逃亡コルーチン null→逃亡コルーチンの開始
                if (offScreenFlag == true && runawayCoroutine == null)
                {
                    runawayCoroutine = StartCoroutine(RunawayCol());
                }

                // ②外フラグがON かつ 逃亡コルーチンあり⇒毎フレーム Vector3.Lerpなどで逃亡地点に向けて逃げていく
                else if (offScreenFlag == true && runawayCoroutine != null)
                {

                    // 逃亡地点に逃げる
                    transform.position = Vector3.Lerp(
                        transform.position,
                        targetPosition,
                        Time.deltaTime * moveSpeed
                        );
                }

                // ③外フラグがOFF 急襲コルーチン null→急襲コルーチンの開始
                if (offScreenFlag == false && surpriseAttackCoroutine == null)
                {
                    surpriseAttackCoroutine = StartCoroutine(SurpriseAttackCol());
                }

                // ④外フラグがOFF かつ 急襲コルーチンあり毎フレームVector3.Lerpなどで急襲地点に向けてやってくる
                if (offScreenFlag == false && surpriseAttackCoroutine != null)
                {
                    // 急襲地点にやってくる
                    transform.position = Vector3.Lerp(
                        transform.position,
                        targetPosition,
                        Time.deltaTime * moveSpeed
                        );
                }
            }
        }
    }

    // 逃亡コルーチン
    IEnumerator RunawayCol()
    {
        // 帰還・待機時のスピード（少しゆっくり）
        moveSpeed = 5.0f;

        // 次の出現位置をランダムに決めて画面外で待機
        targetPosition = new Vector3(Random.Range(moveRangeX.x, moveRangeX.y), startPos.y, 0);

        yield return new WaitForSeconds(2.0f);

        offScreenFlag = false; // 出撃準備完了
        runawayCoroutine = null;
    }

    // 急襲コルーチン
    IEnumerator SurpriseAttackCol()
    {
        if (player == null) yield break;

        // --- 1. 落下地点（X座標）を先に決める ---
        float totalWidth = moveRangeX.y - moveRangeX.x;
        float sectorWidth = totalWidth / 5.0f;
        int randomSector = Random.Range(0, 5);
        float targetX = moveRangeX.x + (sectorWidth * randomSector) + (sectorWidth / 2.0f);

        // --- 2. 画面外の上空に「瞬時に」移動させる ---
        // 現在のY座標よりも高い位置、または画面外の固定値（例: 8.0fなど）にワープ
        float spawnY = 8.0f;
        transform.position = new Vector3(targetX, spawnY, 0);

        // --- 3. 落下先のターゲットを設定する ---
        // 真下に落としたいので、Xは変えずにYだけプレイヤーの高さ付近にする
        float targetY = player.transform.position.y + 1.0f; // 攻撃を繰り出す高さ
        targetPosition = new Vector3(targetX, targetY, 0);

        // --- 4. 落下スピードを上げる ---
        moveSpeed = 15.0f; // 急降下なので速めに設定

        // 移動（落下）が完了するまで少し待つ
        // もし移動にUpdate文のMoveTowards等を使っているなら、距離が縮まるのを待つのもアリです
        yield return new WaitForSeconds(0.6f);

        // --- 急襲時のスピード（素早く！） ---
        //moveSpeed = 10.0f;

        //// 画面内を5分割してランダムな位置をターゲットにする
        //float totalWidth = moveRangeX.y - moveRangeX.x; // 全体の横幅
        //float sectorWidth = totalWidth / 5.0f;          // 1エリアあたりの幅
        //int randomSector = Random.Range(0, 5);          // 0〜4のインデックスを抽選

        //// エリアの左端 ＋ エリアの半分 ＝ エエリアの中心座標
        //float targetX = moveRangeX.x + (sectorWidth * randomSector) + (sectorWidth / 2.0f);

        //// Y座標は「画面内の高さ（例: 2.0f）」など固定値、またはプレイヤーの高さ付近に設定
        ////float targetY = 2.0f;
        //float targetY = player.transform.position.y + Random.Range(0, 3);

        //targetPosition = new Vector3(targetX, targetY, 0);

        //// 移動完了の時間を見積もって待つ (スピードを速めたので、待機時間も少し短く調整する)
        //yield return new WaitForSeconds(0.8f);

        //// プレイヤーの頭上あたりをターゲットにする
        //targetPosition = new Vector3(
        //    player.transform.position.x,
        //    player.transform.position.y + 2.0f,
        //    0
        //    );

        // 移動完了を待つ（距離が十分縮まるまで、あるいは一定時間）
        //yield return new WaitForSeconds(1.5f);

        // 攻撃判断
        float dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist <= attackRange)
        {
            BossSlashAttack();  // 近接攻撃メソッド
        }
        else
        {
            BossShotAttack();   // ショット攻撃メソッド
        }

        yield return new WaitForSeconds(waitTimeAfterAttack); // 攻撃後の隙

        // --- 6. 帰還 ---
        targetPosition = startPos; // 帰還準備
        offScreenFlag = true;
        surpriseAttackCoroutine = null;

        //yield return new WaitForSeconds(3.0f);
        //// 近接攻撃かショットか
        //if (distance <= range)
        //{
        //    // 近接攻撃
        //    BossSlashAttack();
        //}
        //else
        //{
        //    // ショット
        //    BossShotAttack();
        //}
        //// 一定時間待つ (攻撃後の隙)
        //yield return new WaitForSeconds(3.0f);
        //// 外フラグON、急襲コルーチンの解除
        //offScreenFlag = true;
        //surpriseAttackCoroutine = null;
    }

    void OnCollisionEnter(Collision other)
    {
        // ダメージ中でない時の処理
        if (!inDamage)
        {
            // PlayerAttackタグを持つオブジェクトに当たったらダメージ
            if (other.gameObject.tag == "PlayerAttack")
            {
                Debug.Log("プレイヤーの攻撃がボスにヒット！");
            }
            // 体力がなくなったら死亡
            if (hp <= 0)
            {

            }
        }

    }

    // 近接攻撃メソッド
    void BossSlashAttack()
    {
        float rad = GetAngleToPlayer();
        float deg = rad * Mathf.Rad2Deg;

        // 生成位置：本体からプレイヤー方向に少しオフセット（Sin/Cosを利用）
        Vector3 spawnPos = transform.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * 0.8f;

        // BossSlashを生成（Z軸の向きをプレイヤーに合わせる）
        Instantiate(bossSlashPrefab, spawnPos, Quaternion.Euler(0, 0, deg));
        Debug.Log("BossSlash生成");
    }

    // ショット攻撃メソッド
    void BossShotAttack()
    {
        float rad = GetAngleToPlayer();
        float deg = rad * Mathf.Rad2Deg;

        Vector3 spawnPos = transform.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * 1.0f;

        // BossShotを生成
        GameObject shot = Instantiate(bossShotPrefab, spawnPos, Quaternion.Euler(0, 0, deg));

        // AddForce用の方向ベクトル（Sin/Cosを利用）
        Vector3 shootDir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);

        Rigidbody rb = shot.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(shootDir * 500f);
        }
        Debug.Log("BossShot生成");
    }

    // プレイヤーへの角度（ラジアン）を取得する共通処理
    float GetAngleToPlayer()
    {
        Vector3 diff = player.transform.position - transform.position;
        return Mathf.Atan2(diff.y, diff.x);
    }

    // 落下準備の関数（コルーチンなどの中で呼ぶ）
    void PrepareDrop()
    {
        // 1. ランダムに1つ場所を選ぶ
        int index = Random.Range(0, dropPoints.Length);
        float targetX = dropPoints[index];

        // 2. 画面外の上空にパッと配置する
        // targetYは画面に映らない十分な高さ（例：10）
        float spawnY = 10f;
        transform.position = new Vector3(targetX, spawnY, 0);

        // 3. ここで落下のスイッチを入れる（物理演算なら重力を有効にするなど）
        StartCoroutine(DropRoutine());
    }

    IEnumerator DropRoutine()
    {
        // 例：地面（y=0付近）に着くまで真下に移動させる
        while (transform.position.y > 0.5f)
        {
            transform.Translate(Vector3.down * Time.deltaTime * dropSpeed);
            yield return null;
        }
        // 着地後の処理...
    }

    private void OnDrawGizmos()
    {
        // Sceneビューで攻撃範囲と分割エリアを確認できるようにする
        Gizmos.color = Color.red;
        float totalWidth = moveRangeX.y - moveRangeX.x;
        float sectorWidth = totalWidth / 5.0f;
        for (int i = 0; i <= 5; i++)
        {
            float x = moveRangeX.x + (sectorWidth * i);
            Gizmos.DrawLine(new Vector3(x, 10, 0), new Vector3(x, -10, 0));
        }
    }
}
