using System.Collections;
using UnityEngine;

public class Enemy4_Controller : MonoBehaviour
{

    GameObject player;

    GameObject enemyShot;
    GameObject enemyGuard;
    GameObject gate;

    Rigidbody rbody;

    Coroutine shot;
    bool isShoot;

    Coroutine guard;
    bool isGuard;

    float interval = 5.0f;

    float playerDirection;
    float shotSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rbody = enemyShot.GetComponent<Rigidbody>();
        gate = GameObject.FindGameObjectWithTag("gate");
        enemyGuard = GameObject.FindGameObjectWithTag("EnemyGuard");

        enemyGuard.transform.SetParent(transform);
        enemyGuard.SetActive(false);

        StartCoroutine(OnAttack());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator OnAttack()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        playerDirection = player.transform.position.x - gameObject.transform.position.x;
        if (shot != null && isShoot == false)
        {
            Instantiate(enemyShot,
                gate.transform.position,
                Quaternion.identity);

            if (playerDirection != 0)
            {

                rbody.AddForce(new Vector3(playerDirection * shotSpeed, 0, 0), ForceMode.Impulse);

                yield return new WaitForSeconds(interval);

                Destroy(enemyShot);

                isShoot = true;
                guard = null;
            }
        }

        if (guard != null && isGuard == false)
        {
            enemyGuard.SetActive(true);

            yield return new WaitForSeconds(interval);

            enemyGuard.SetActive(false);

            isGuard = true;
            shot = null;
        }

    }

}
