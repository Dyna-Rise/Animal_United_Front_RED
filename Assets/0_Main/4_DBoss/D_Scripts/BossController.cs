using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("体力")]
    public int hp = 10;
    public GameObject bodyObject;   // 子オブジェクトの"Body"
    public float invincibilityDuration = 1.0f; // ダメージ後の無敵時間

    [Header("移動")]
    public float moveSpeed = 2.0f;       // 基本の移動速度
    public float dropSpeed = 4.5f;       // 急襲時の落下速度
    public float minXCoordinate = -5.0f; // 移動範囲（左端）
    public float maxXCoordinate = 5.0f;  // 移動範囲（右端）
    public Vector3 startPos;             // 画面外の待機・帰還位置

    [Header("攻撃")]
    public float attackRange = 5.0f;     // 攻撃に切り替わる距離
    public float bossShotSpeed = 5.0f;   // 弾の速度
    public float waitTimeAfterAttack = 1.0f; // 攻撃後の硬直時間

    [Header("プレハブ")]
    public GameObject bossSlashPrefab;   // 近接攻撃のプレハブ
    public GameObject bossShotPrefab;    // 遠距離攻撃のプレハブ

    // --- 内部処理用の変数（インスペクターには表示しない） ---
    private GameObject player;
    private Vector3 targetPosition;
    private float damageTimer = 0f;
    private bool isInvincible = false;
    private Coroutine runawayCoroutine;
    private Coroutine surpriseAttackCoroutine;

    [Header("Debug Info")] // 動作確認用にインスペクターで見れるようにしておく
    public bool offScreenFlag = true;
    public float distanceToPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        targetPosition = startPos;
    }

    // Update is called once per frame
    void Update()
    {
        HandleDamageFlashing();

        if (hp > 0 && player != null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            // ① 画面外へ逃亡中
            if (offScreenFlag && runawayCoroutine == null)
            {
                runawayCoroutine = StartCoroutine(RunawayCol());
            }
            // ② 画面外へ移動中の座標更新
            else if (offScreenFlag && runawayCoroutine != null)
            {
                MoveTowardsTarget();
            }
            // ③ 急襲準備
            if (!offScreenFlag && surpriseAttackCoroutine == null)
            {
                surpriseAttackCoroutine = StartCoroutine(SurpriseAttackCol());
            }
            // ④ 急襲中の座標更新
            else if (!offScreenFlag && surpriseAttackCoroutine != null)
            {
                MoveTowardsTarget();
            }
        }
    }


    // 移動処理の共通化
    void MoveTowardsTarget()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
    }

    // 点滅処理の分離
    void HandleDamageFlashing()
    {
        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;
            bool isActive = Mathf.Sin(damageTimer * 50f) > 0;
            if (bodyObject != null) bodyObject.SetActive(isActive);
        }
        else if (isInvincible)
        {
            if (bodyObject != null) bodyObject.SetActive(true);
            isInvincible = false;
        }
    }

    IEnumerator RunawayCol()
    {
        moveSpeed = 2.0f; // 帰還はゆっくり
        targetPosition = new Vector3(transform.position.x, startPos.y, 0);

        while (Vector3.Distance(transform.position, targetPosition) > 0.5f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2.0f);
        offScreenFlag = false;
        runawayCoroutine = null;
    }

    IEnumerator SurpriseAttackCol()
    {
        if (player == null) yield break;

        // 1. 落下地点の決定
        float totalWidth = maxXCoordinate - minXCoordinate;
        float sectorWidth = totalWidth / 5.0f;
        int randomSector = Random.Range(0, 5);
        float targetX = minXCoordinate + (sectorWidth * randomSector) + (sectorWidth / 2.0f);

        transform.position = new Vector3(targetX, startPos.y, 0);
        targetPosition = transform.position;

        yield return new WaitForSeconds(0.5f);

        // 2. 落下ターゲット（高さ）の決定
        float playerY = player.transform.position.y;
        float groundY = 0f;
        float targetY = Random.Range(playerY + 2.0f, groundY + 3.5f);

        targetPosition = new Vector3(targetX, targetY, 0);
        moveSpeed = dropSpeed;

        // 3. 到着待ち
        while (Vector3.Distance(transform.position, targetPosition) > 0.5f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.85f);

        // 4. 攻撃実行
        if (distanceToPlayer <= attackRange)
        {
            BossSlashAttack();
        }
        else
        {
            if (hp >= 5) BossShotAttack();
            else BossShotAttack3Way();
        }

        yield return new WaitForSeconds(waitTimeAfterAttack);

        // 5. 帰還準備
        targetPosition = new Vector3(transform.position.x, startPos.y, 0);
        offScreenFlag = true;
        surpriseAttackCoroutine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack") && !isInvincible && hp > 0)
        {
            TakeDamage(1);
        }
    }

    void TakeDamage(int amount)
    {
        hp -= amount;
        damageTimer = invincibilityDuration;
        isInvincible = true;

        if (hp <= 0) BossDie();
    }

    void BossDie()
    {
        StopAllCoroutines();
        if (bodyObject != null) bodyObject.SetActive(false);
        Destroy(gameObject, 1.0f);
    }

    void BossSlashAttack()
    {
        float rad = GetAngleToPlayer();
        Vector3 spawnPos = transform.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * 0.8f;
        GameObject slash = Instantiate(bossSlashPrefab, spawnPos, Quaternion.Euler(0, 0, rad * Mathf.Rad2Deg));
        Destroy(slash, 0.5f);
    }

    void BossShotAttack()
    {
        float rad = GetAngleToPlayer();
        Vector3 shootDir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
        GameObject shot = Instantiate(bossShotPrefab, transform.position + shootDir, Quaternion.Euler(0, 0, rad * Mathf.Rad2Deg));

        Rigidbody rb = shot.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = shootDir * bossShotSpeed;
        Destroy(shot, 5.0f);
    }

    void BossShotAttack3Way()
    {
        float baseRad = GetAngleToPlayer();
        float[] shotAngles = { 0f, 20f, -20f };

        foreach (float angleOffset in shotAngles)
        {
            float shotRad = baseRad + (angleOffset * Mathf.Deg2Rad);
            Vector3 shootDir = new Vector3(Mathf.Cos(shotRad), Mathf.Sin(shotRad), 0).normalized;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.right, shootDir);
            GameObject shot = Instantiate(bossShotPrefab, transform.position + shootDir, rotation);

            Rigidbody rb = shot.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = shootDir * bossShotSpeed;
            Destroy(shot, 5.0f);
        }
    }

    float GetAngleToPlayer()
    {
        Vector3 diff = player.transform.position - transform.position;
        return Mathf.Atan2(diff.y, diff.x);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        float totalWidth = maxXCoordinate - minXCoordinate;
        float sectorWidth = totalWidth / 5.0f;
        for (int i = 0; i <= 5; i++)
        {
            float x = minXCoordinate + (sectorWidth * i);
            Gizmos.DrawLine(new Vector3(x, 15, 0), new Vector3(x, -5, 0));
        }
    }
}