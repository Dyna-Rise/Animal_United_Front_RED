using UnityEngine;

public class Enemy3_Controller : MonoBehaviour
{
    GameObject player;
    float distance;
    float searchRange = 5.0f;

    Vector3 d3posPlayer;
    Vector3 d3posEnemy;

    Rigidbody rbody;

    float distruction = 15.0f;

    float chaseSpeed = 3.0f;

    bool active = false;

    Vector3 temporaryDistance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("左行く");
        rbody.linearVelocity = new Vector3(-1, 0, 0);

        d3posPlayer = player.transform.position;
        d3posEnemy = transform.position;

        Vector2 d2posPlayer = new Vector2(d3posPlayer.x, d3posPlayer.z);
        Vector2 d2posEnemy = new Vector2(d3posEnemy.x, d3posEnemy.z);

        distance = Vector2.Distance(d2posPlayer, d2posEnemy);

        if (!active && distance <= searchRange)
        {
            temporaryDistance = player.transform.position;
            Destroy(gameObject, distruction);
            active = true;
        }
        else if (active)
        {
            Vector3 target = (temporaryDistance - d3posEnemy).normalized;
            rbody.linearVelocity = target * chaseSpeed;
            Debug.Log(target);
            if (temporaryDistance.x <= 1)
            {
                Invoke("Inactive", 5.0f);
            }
        }
    }

    void Inactive()
    {
        active = false;
    }
}
