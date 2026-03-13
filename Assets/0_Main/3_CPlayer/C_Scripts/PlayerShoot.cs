using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public float deleteTime = 5.0f;
    void Start()
    {
        Destroy(gameObject, deleteTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

}
