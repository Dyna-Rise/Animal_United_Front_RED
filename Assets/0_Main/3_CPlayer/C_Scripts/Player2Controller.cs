using UnityEngine;
using UnityEngine.InputSystem;

public class Player2Controller : MonoBehaviour
{
    PlayerMove playerMove;
    public GameObject shotPrefab;
    public GameObject gate;

    public int shotSpeed = 5;


    void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            Shot();
        }
    }

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Shot()
    {
        float dir = playerMove.LastInputDirection;
        GameObject shotObj = Instantiate(
            shotPrefab,
            gate.transform.position,
            Quaternion.identity
        );

        Rigidbody rb = shotObj.GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(shotSpeed * dir, 0, 0), ForceMode.Impulse);
    }
}
