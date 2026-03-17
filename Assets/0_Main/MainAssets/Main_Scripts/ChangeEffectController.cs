using UnityEngine;

public class ChangeEffectController : MonoBehaviour
{
    [Header("削除時間")]
    public float deleteTime = 0.5f;

    void Start()
    {
        Destroy(gameObject, deleteTime);
    }
}
