using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player3Controller : MonoBehaviour
{
    CharacterController controller;
    PlayerMove playerMove;
    PlayerDash playerDash;
    public GameObject guradPrefab;
    GameObject guradObj;
    public GameObject gate;
    bool isGurad;
    public float hoverGravityScale = 0.05f;
    public float hoverUpPower = 1.0f;
    public float hoverUpMaxPower = 5.0f;
    public float hoverUpTime;
    public float hoverAccelerationTime = 1.0f;
    float hoverCurrentPower;

    public float hoverTime;
    public float maxHoverTime = 1.5f;
    Coroutine hoverUp;
    bool isHoverUp;
    bool isHovering;

    bool jumpButtonHeld;

    void OnInteract(InputValue value)
    {
        isGurad = value.isPressed;  // キーを押してるか判定
    }
 
    void OnJump(InputValue value)
    {
        jumpButtonHeld = value.isPressed;   // キーを押してるか判定
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerMove = GetComponent<PlayerMove>();
        playerDash = GetComponent<PlayerDash>();
    }

    void Update()
    {
        // Gurad機能
        if (isGurad)
        {
            if (!controller.isGrounded) return; // 空中では発動不可
            GuradOn();
        }
        else
        {
            GuradOff();
        }

        // Hovering機能
        if (controller.isGrounded)  // 接地状態ではフラグをOFF、タイマーを0にしておく
        {
            isHovering = false;
            hoverTime = 0;
        }

        // Hovering制限時間処理
        if (isHovering) hoverTime += Time.deltaTime;
        if(hoverTime >= maxHoverTime) isHovering = false;
        if (jumpButtonHeld && hoverTime <= maxHoverTime)
        {
            Hovering(); // Jumpボタンホールド中Hoveringする
        }
        if (jumpButtonHeld && playerMove.MoveDirection.y > 0) isHoverUp = false;    // 上昇
        if(!jumpButtonHeld) isHoverUp = true;

        // Hovering中のキャラ上昇処理
        if (playerMove.MoveDirection.y < 0 &&           // 落下中である
            jumpButtonHeld &&                           // ボタン入力時である
            hoverUp == null &&                          // コルーチン発動条件を満たしている（クールタイムではない）
            isHoverUp &&                                // 1回のボタンホールドに対して1回しか上昇できないフラグチェック
            isHovering                                  // Hovering中である（制限時間内である）
            )
        {
            isHoverUp = false;                          // 発動後すぐに発動フラグを切る（1ボタンにつき1上昇なので）
            hoverUp = StartCoroutine(HoverUpCol());     // 上昇処理コルーチン発動
        }
    }

    void GuradOn()
    {
        if (guradObj == null)   // 複数生成防止用
        {
            guradObj = Instantiate( // gateを起点にインスタンティエート
                guradPrefab,
                gate.transform.position,
                Quaternion.identity
            );
            guradObj.GetComponent<Collider>().enabled = false;  // 生成したらコライダーをOFFにする
        }

        playerMove.enabled = false;     // 移動処理を止める
        playerDash.enabled = false;     // ダッシュ処理を止める
    }

    void GuradOff()
    {
        if (guradObj != null)
        {
            Destroy(guradObj);          // ボタンを離したらオブジェクトを消去する
            playerMove.enabled = true;  // 移動処理を復活
            playerDash.enabled = true;  // ダッシュ処理を復活
        }
    }

    void Hovering()
    {
        if(!isHovering) StartHoveringTimer();   // ホバリング開始時にタイマーを起動
        if (hoverTime >= maxHoverTime)          // 制限時間を超えたら強制的に止める
        {
            isHovering = false;
            return;
        }

        if (playerMove.MoveDirection.y < 0 && isHovering)
        {
            float hoverGravity = playerMove.gravity * hoverGravityScale;
            float hoveringY = playerMove.MoveDirection.y + (playerMove.gravity - hoverGravity) * Time.deltaTime;
            playerMove.SetMoveDirectionY(hoveringY);
        }
    }

    IEnumerator HoverUpCol()
    {
        while (hoverUpTime < hoverAccelerationTime)
        {
            hoverUpTime += Time.deltaTime;
            float t = Mathf.Clamp01(hoverUpTime / hoverAccelerationTime);
            hoverCurrentPower = Mathf.SmoothStep(hoverUpPower, hoverUpMaxPower, t);
            playerMove.SetMoveDirectionY(hoverCurrentPower);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        hoverUpTime = 0.0f;
        hoverUp = null;
    }

    void StartHoveringTimer()
    {
        hoverTime = 0f;
        isHovering = true;
    }
}
