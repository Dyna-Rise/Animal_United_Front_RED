using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2Controller : MonoBehaviour
{
    GameObject player; //Player情報

    [Header("体力・スピード")]
    public int life = 3;

    [Header("投擲物・発射位置")]
    public GameObject throwObject;
    public GameObject gate;

    [Header("投擲間隔・角度・パワー")]
    public float interval = 5.0f;
    public float angle = 135.0f;
    public float thowPower = 10.0f;

    [Header("索敵範囲")]
    public float range = 5.0f;

    [Header("ダメージ時間・ダメージ移動量")]
    public float stunTime = 0.2f;
    public float damageSpeed = 0.5f;

    float damageTimer; //ダメージ時間を測るタイマー
    bool isDamage; //ダメージフラグ

    [Header("点滅対象")]
    public GameObject enemyBody;


    //投擲コルーチン
    Coroutine throwingCoroutine;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerFollower");
        
    }

    // Update is called once per frame
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

        if (throwingCoroutine == null)
        {
            throwingCoroutine = StartCoroutine(ThrowingCol());
        }
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

    IEnumerator ThrowingCol()
    {
        yield return new WaitForSeconds(interval);
        Throwing();
        throwingCoroutine = null;
    }

    void Throwing()
    {
        GameObject obj = Instantiate(
            throwObject,
            gate.transform.position,
            Quaternion.identity);

        float dx = Mathf.Cos(angle * Mathf.Deg2Rad);
        float dy = Mathf.Sin(angle * Mathf.Deg2Rad);
        Vector3 v = new Vector3(dx, dy, 0).normalized * thowPower;

        obj.GetComponent<Rigidbody>().AddForce(v, ForceMode.Impulse);
    }
}
