using UnityEngine;

public class LightController : MonoBehaviour
{
    [Header("スピード")]
    public float speed = 2.0f;

    void FixedUpdate()
    {
        transform.Rotate(speed, speed, speed);        
    }
}
