using UnityEngine;

public class ThrowingController : MonoBehaviour
{
    public float deleteTime = 10.0f;

    void Start()
    {
        Destroy(gameObject, deleteTime);
    }

}
