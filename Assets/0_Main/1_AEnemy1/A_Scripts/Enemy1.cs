using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float speed = 15f;
    public bool isToRight = false;
    public float revTime = 0;
    public LayerMask groundLayer;
    bool onGround = false;
    float time = 0;

    Rigidbody2D rbody;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isToRight)
        {
            transform.position = new Vector2(1, -1);
        }
        rbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (revTime > 0)
        {
            time += Time.deltaTime;
            if (time >= revTime)
            {
                isToRight = !isToRight;
                time = 0;
                if (isToRight)
                {
                    transform.position = new Vector2(-1, 1);
                }
                else
                {
                    transform.position = new Vector2(1, 1);
                }
            }
        }
    }
    private void OnTriggerExit(Collision other)
    {
        if (other.gameObject.layer == 6)
        {
            transform.position = new Vector2(-1, 1);
        }
        else
        {
            transform.position = new Vector2(1, 1);
        }
    }
    void FixedUpdate()
    {
        if (onGround)
        {
            Rigidbody2D rbody = GetComponent<Rigidbody2D>();
            if (isToRight)
            {
                rbody.linearVelocity = new Vector2(speed, rbody.linearVelocity.y);
            }
            else
            {
                rbody.linearVelocity = new Vector2(-speed, rbody.linearVelocity.y);
            }
        }
    }
}