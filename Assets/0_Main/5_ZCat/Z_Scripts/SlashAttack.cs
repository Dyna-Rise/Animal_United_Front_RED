using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SlashAttack : MonoBehaviour
{
    PlayerMove playerMove;
    GameObject playerFollower;
    AudioSource audio;

    [Header("生成プレハブ・位置")]
    public GameObject[] slashEffectPrefabs;
    public Vector2[] slashOffsets;

    public float attackCooldown = 0.2f;   // 攻撃クールダウン
    public bool comboCooldown = false;
    public float comboResetTime = 1.0f;   // 次の攻撃を入力しないとコンボがリセットされる時間
    public float effectLifetime = 0.2f;   // 生成された攻撃エフェクトの表示時間

    int currentComboStage = 0;    // 現在のコンボ段階 (0:初期状態, 1:一段目, 2:二段目, 3:三段目)
    float lastAttackTime = -Mathf.Infinity; // 最後に攻撃した時間
    float comboTimer = 0f;        // コンボリセット用のタイマー

    [Header("SE")]
    public AudioClip slashClip;

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        playerFollower = GameObject.FindGameObjectWithTag("PlayerFollower");
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        // コンボリセットタイマーの処理
        if (currentComboStage > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                ResetCombo();
            }
        }
    }

    // InputSystemのOnAttackイベントから呼び出されるメソッド
    void OnAttack(InputValue value)
    {
        //クールダウンされていたら&コンボクールが入っていなければ
        if (value.isPressed && Time.time >= lastAttackTime + attackCooldown && !comboCooldown)
        {
            // ダッシュ中かどうかなどで攻撃を制限
            // if (playerDash != null && playerDash.IsDashing) return; 

            PerformSlashAttack(); //攻撃メソッド
            comboTimer = comboResetTime; // コンボリセットタイマーをリセット

            lastAttackTime = Time.time; //最後の時間を更新
        }
    }

    void PerformSlashAttack()
    {
        currentComboStage++;
        //最後の攻撃だった場合はコンボクールを入れる
        if(currentComboStage == 3)
        {
            comboCooldown = true;
            StartCoroutine(ComboCoolDownCol());
        }
        else if (currentComboStage > 3)　//コンボ数を超えたら最初にリセット
        {
            currentComboStage = 1; 
        }

        // 現在のコンボ段階に応じた攻撃エフェクトを生成
        Slash(currentComboStage - 1); // 配列インデックスは0から始まるため -1
    }

    //コンボクールダウンタイムを0.5秒後に解除
    IEnumerator ComboCoolDownCol()
    {
        yield return new WaitForSeconds(0.5f);
        comboCooldown = false;
    }

    // 各段階の攻撃を生成
    void Slash(int stageIndex)
    {
        GetComponent<AudioSource>().PlayOneShot(slashClip);

        // プレイヤーの最後の向きを取得
        float playerFacingDirectionX = playerMove.LastInputDirection;
        //Debug.Log(playerMove.LastInputDirection);

        // 攻撃エフェクトの生成位置を計算
        Vector3 spawnOffset = slashOffsets[stageIndex];
        if(playerFacingDirectionX < 0)
        {
            spawnOffset.x *= -1;
        }

        Vector3 spawnPosition = transform.position + spawnOffset;

        // 攻撃エフェクトを生成
        GameObject slashEffect = Instantiate(
            slashEffectPrefabs[stageIndex],
            spawnPosition,
            Quaternion.identity);

        //生成エフェクトをPlayerの子オブジェクトにする
        slashEffect.transform.parent = playerFollower.transform;

        //生成したエフェクトの向きを調整
        if (playerFacingDirectionX < 0)
        {
            slashEffect.transform.localScale = new Vector3(-2, 2, 1);
        }
        else if(playerFacingDirectionX > 0)
        {
            slashEffect.transform.localScale = new Vector3(2, 2, 1);
        }

        // 生成されたエフェクトは、時間差で破棄
        Destroy(slashEffect, effectLifetime);
    }

    void ResetCombo()
    {
        currentComboStage = 0;
        comboTimer = 0f;
    }
}