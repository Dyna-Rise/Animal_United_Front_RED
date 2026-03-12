using UnityEngine;

public class AfterimageDestroyer : MonoBehaviour
{
    public float lifetime = 0.3f; // 破棄されるまでの時間

    void Start()
    {
        // lifetime秒後にこのGameObjectを破棄する
        Destroy(gameObject, lifetime);
    }    
}