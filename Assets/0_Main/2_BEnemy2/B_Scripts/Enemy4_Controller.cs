using System.Collections;
using UnityEngine;

public class Enemy4_Controller : MonoBehaviour
{

    public int life;
    GameObject player;

    //[SerializeField] GameObject enemyShot;
    //[SerializeField] GameObject enemyGuard;
    public GameObject enemyShot;
    public GameObject enemyGuard;
    GuardController guardC;
    public Transform gate;

    Rigidbody rbody;

    Coroutine shot;
    bool isShot = false;

    Coroutine guard;
    bool isGuard;

    float interval = 5.0f;

    //float playerDirection;
    float shotSpeed = 5.0f;

    Coroutine onAttack;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //gate = transform.Find("gate");
        //enemyGuard = GameObject.FindGameObjectWithTag("EnemyGuard");

        guardC = GetComponentInChildren<GuardController>();
        enemyGuard.transform.SetParent(transform);
        enemyGuard.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (onAttack == null)
        {
            //Debug.Log("コルーチン起動");
            onAttack = StartCoroutine(OnAttack());
        }
    }

    IEnumerator OnAttack()
    {
        //player = GameObject.FindGameObjectWithTag("Player");

        //playerDirection = player.transform.position.x - gameObject.transform.position.x;

        if (guard == null && isShot == false)
        {
            //Debug.Log("弾を生成");
            GameObject obj = Instantiate(enemyShot,
                  gate.transform.position,
                  Quaternion.identity);

            rbody = obj.GetComponent<Rigidbody>();

            rbody.AddForce(Vector3.left * shotSpeed, ForceMode.Impulse);

            //Debug.Log("飛ばす");

            yield return new WaitForSeconds(interval);

            Destroy(obj);

            isShot = true;
            isGuard = false;
            guard = null;
            //Debug.Log("ショット終わり");

        }

        if (shot == null && isGuard == false)
        {
            enemyGuard.SetActive(true);


            yield return new WaitForSeconds(interval);

            enemyGuard.SetActive(false);



            isGuard = true;
            isShot = false;
            shot = null;
            //Debug.Log("ガード終わり");
        }

        onAttack = null;

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == ("PlayerAttack") && guardC.guarded == false)
        {
            life--;
            //Debug.Log(life);

            if (life <= 0)
            {
                Destroy(gameObject);
            }
        }

    }

}
