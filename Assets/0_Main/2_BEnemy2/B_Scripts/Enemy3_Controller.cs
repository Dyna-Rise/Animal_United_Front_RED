using UnityEngine;

public class Enemy3_Controller : MonoBehaviour
{
    GameObject player;
    float distance;
    float searchRange = 5.0f;

    Vector3 d3posPlayer;
    Vector3 d3posEnemy;

    Rigidbody rbody;
    CapsuleCollider capsuleCollider;

    float distruction = 5.0f;

    float chaseSpeed = 3.0f;

    float dx;
    float dy;

    bool active = false;

    Vector3 temporaryDistance;

    int ground;
    int enemy3;

    float body;

    float damageCount;
    bool isDamaged = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        ground = LayerMask.NameToLayer("Ground");
        enemy3 = LayerMask.NameToLayer("Enemy");

        Physics.IgnoreLayerCollision(ground, enemy3, true);
        rbody.linearVelocity = new Vector3(-1, 0, 0);

        //Debug.Log(distance);

        d3posPlayer = player.transform.position;
        d3posEnemy = transform.position;

        Vector2 d2posPlayer = new Vector2(d3posPlayer.x, d3posPlayer.z);
        Vector2 d2posEnemy = new Vector2(d3posEnemy.x, d3posEnemy.z);

        distance = Vector2.Distance(d2posPlayer, d2posEnemy);

        if (!active && distance <= searchRange)
        {
             dx = player.transform.position.x - transform.position.x;
             dy = player.transform.position.y - transform.position.y;

            Destroy(gameObject, distruction);
            active = true;
        }
        else if (active)
        {
            float rad = Mathf.Atan2(dx, dy);

            float vx = Mathf.Cos(rad);
            float vy = Mathf.Sin(rad);
            rbody.linearVelocity = new Vector3(vx, vy, 0).normalized * chaseSpeed;
        }
    }

    void Inactive()
    {
        active = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == ("PlayerAttack"))
        {
            if (damageCount == 0 && isDamaged == false)
            {
                life--;
                damageCount++;

                Invoke("Damaged", 1.0f);
            }
        }
    }

    void Damaged()
    {
        isDamaged = true;
    }


    
}
