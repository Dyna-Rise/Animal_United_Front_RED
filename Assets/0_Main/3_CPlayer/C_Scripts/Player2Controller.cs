using UnityEngine;
using UnityEngine.InputSystem;

public class Player2Controller : MonoBehaviour
{
    PlayerMove playerMove;
    public GameObject shotPrefab;       // 弾プレハブ
    public GameObject gate;             // 弾発生位置

    public int shotSpeed = 12;          // 弾速


    void OnAttack(InputValue value)
    {
        if (value.isPressed) Shot();    // キー入力時発射処理
    }

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
    }

    // 発射処理
    void Shot()
    {
        float dir = playerMove.LastInputDirection;  // PlayerMoveからPlayerの向き情報を取得（1, -1）
        GameObject shotObj = Instantiate(           // 弾をインスタンティエート
            shotPrefab,
            gate.transform.position,
            Quaternion.identity
        );

        Rigidbody rb = shotObj.GetComponent<Rigidbody>();
        // 弾速にPlayerの向きを掛けてAddForceで押し出す
        rb.AddForce(new Vector3(shotSpeed * dir, 0, 0), ForceMode.Impulse);
    }
}
