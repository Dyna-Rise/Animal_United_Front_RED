using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    //コンポーネント
    Rigidbody rbody;

    [Header("体力・スピード")]
    public int life = 3;
    public float speed = 1.0f;

    [Header("ダメージ時間・ダメージ移動量")]
    public float stunTime = 0.2f;
    public float damageSpeed = 0.5f;

    float damageTimer; //ダメージ時間を測るタイマー
    bool isDamage; //ダメージフラグ

    [Header("点滅対象")]
    public GameObject enemyBody;

    int direction = -1; //方向値

    void Start()
    {
        rbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //ダメージ中なら減らす
        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;

            float val = Mathf.Sin(Time.time * 50);
            if (val > 0) enemyBody.SetActive(true);
            else enemyBody.SetActive(false);

        }
        else if (isDamage)
        {
            enemyBody.SetActive(true);
            isDamage = false;
        }

        //方向値の方へ回転
        if (direction == 1)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    void FixedUpdate()
    {
        //ダメージが入っているなら動きが鈍い
        if (damageTimer > 0) rbody.linearVelocity = new Vector3(direction, 0, 0) * damageSpeed;
        else rbody.linearVelocity = new Vector3(direction, 0, 0) * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerAttack")
        {
            if (damageTimer <= 0 && !isDamage)
            {
                life--;
                if (life <= 0)
                {
                    Destroy(gameObject);
                }
                damageTimer = stunTime;
                isDamage = true;
            }
        }
    }

    //センサーがGroundを抜けた場合
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            direction *= -1; //逆転させる
        }

    }

    //何かと衝突したら
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != 6)
            direction *= -1; //逆転させる
    }
}