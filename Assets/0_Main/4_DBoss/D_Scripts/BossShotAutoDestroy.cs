using UnityEngine;

public class ShotAutoDestroy : MonoBehaviour
{
    // カメラに映らなくなった瞬間に呼ばれる
    void OnBecameInvisible()
    {
        // 念のため、親オブジェクトなどがいないか確認して自分を消去
        Destroy(gameObject);
    }
}
