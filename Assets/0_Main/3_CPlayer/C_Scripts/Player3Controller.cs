using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player3Controller : MonoBehaviour
{
    CharacterController controller;             // isGroundフラグを取るためのcontroller
    PlayerMove playerMove;                      // 移動処理スクリプト（ガード中止める用）
    PlayerDash playerDash;                      // ダッシュ処理スクリプト（ガード中止める用）
    GameObject guradObj;                        // 生成したGuradオブジェクトを入れておく変数

    [Header("Gurad")]
    public GameObject guradPrefab;              // 生成するGuradPrefab
    public GameObject gate;                     // Guradオブジェクトの発生位置を指定するgeteオブジェクト
    bool isGurad;                               // Gurad入力検知用

    [Header("Hovering")]
    bool jumpButtonHeld;                        // Hovering入力検知用
    bool isHovering;                            // Hovering中かどうかの管理フラグ
    public float hovGravityScale = 0.1f;        // Hovering中の重力係数
    public float hovTime;                       // Hovering継続可能時間格納用
    public float maxHoverTime = 1.5f;           // Hovering継続可能時間

    [Header("HoverUp")]
    public float hovUpPower = 0.5f;             // 上昇処理の初速
    public float hovUpMaxPower = 2.5f;          // 上昇処理の最大速度
    public float hovUpTime;                     // 上昇時間計測用
    public float hovAccelerationTime = 0.5f;    // 上昇の最大速度に達するまでの時間
    float hovCurrentPower;                      // 上昇の現在速度格納用
    Coroutine hovUp;                            // 上昇処理コルーチン発動フラグ
    bool isHoverUp;                             // 上昇処理可能フラグ


    void OnInteract(InputValue value)
    {
        isGurad = value.isPressed;          // キーを押してるか判定
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
            GuradOn();  // ガードをON
        }
        else
        {
            GuradOff(); //ガードをOFF
        }

        // Hovering機能
        if (controller.isGrounded)  // 接地状態ではフラグをOFF、タイマーを0にしておく
        {
            isHovering = false;
            hovTime = 0;
        }

        // Hovering制限時間処理
        if (isHovering) hovTime += Time.deltaTime;              // Hoveringフラグが立っている間の時間を計測する
        if (hovTime >= maxHoverTime) isHovering = false;        // 制限時間を超えたらフラグをHoveringフラグをOFFにする

        if (jumpButtonHeld && hovTime <= maxHoverTime)          // キーを押している間かつ制限時間内ならHovering処理
        {
            Hovering();                                         // Jumpキーホールド中Hoveringする
        }
        if (jumpButtonHeld && 
            playerMove.MoveDirection.y > 0) isHoverUp = false;  // 上昇中はHoverUp処理を受け付けない
        if (!jumpButtonHeld) isHoverUp = true;                  // Jumpボタンを離したら上昇可能フラグを復活

        // Hovering中のキャラ上昇処理（HoverUp）
        if (playerMove.MoveDirection.y < -1.0f &&               // 落下中である（最高点より少し落下しはじめ）
            jumpButtonHeld &&                                   // ボタン入力中である
            hovUp == null &&                                    // コルーチン発動条件を満たしている（クールタイムではない）
            isHoverUp &&                                        // 1回のボタンホールドに対して1回しか上昇できないフラグチェック
            isHovering                                          // Hovering可能時間中である（制限時間内である）
            )
        {
            isHoverUp = false;                                  // 発動後すぐに発動フラグを切る（1ボタンホールドにつき1上昇）
            hovUp = StartCoroutine(HoverUpCol());               // 上昇処理コルーチン発動
        }
    }

    // Gurad開始処理
    void GuradOn()
    {
        if (guradObj == null)                           // 複数生成防止用（念のため）
        {
            guradObj = Instantiate(                     // gateを起点にインスタンティエート
                guradPrefab,
                gate.transform.position,
                Quaternion.identity
            );
            GetComponent<Collider>().enabled = false;   // ガード中は本体のコライダーをOFFにする
        }
        playerMove.enabled = false;     // ガード中は移動処理を止める
        playerDash.enabled = false;     // ガード中はダッシュ処理を止める
    }

    // Gurad終了処理
    void GuradOff()
    {
        if (guradObj != null)           // 念のためGuradオブジェクトがない場合は処理しないようにしておく
        {
            Destroy(guradObj);                          // ボタンを離したらオブジェクトを消去する
            GetComponent<Collider>().enabled = true;    // 本体のコライダーをONに戻す
            playerMove.enabled = true;                  // 移動処理を復活
            playerDash.enabled = true;                  // ダッシュ処理を復活
        }
    }

    // Hoveringメイン処理
    void Hovering()
    {
        if (!isHovering) StartHoveringTimer();              // Hovering開始時にタイマーを起動
        if (hovTime >= maxHoverTime)                        // 制限時間を超えたら強制的に止める
        {
            isHovering = false;                             //HoveringフラグをOFF
            return;
        }

        if (playerMove.MoveDirection.y < 0 && isHovering)   //下降中のみホバリングする（重力を弱める）
        {
            // PlayerMoveで設定された重力値にホバー中重力係数をかける
            float hoverGravity = playerMove.gravity * hovGravityScale;
            float hoveringY = playerMove.MoveDirection.y + (playerMove.gravity - hoverGravity) * Time.deltaTime;
            playerMove.SetMoveDirectionY(hoveringY);        // PlayerMoveに重力落下の計算値を渡す
        }
    }

    // Hovering中の上昇処理用コルーチン
    IEnumerator HoverUpCol()
    {
        // 上昇時間中の処理
        while (hovUpTime < hovAccelerationTime)
        {
            hovUpTime += Time.deltaTime;
            float t = Mathf.Clamp01(hovUpTime / hovAccelerationTime);           　　// 01でClamp
            hovCurrentPower = Mathf.SmoothStep(hovUpPower, hovUpMaxPower, t); 　　　// SmoothStepで上昇パワーを補完する
            playerMove.SetMoveDirectionY(hovCurrentPower);                        　// PlayerMoveに現在フレームのパワー値を渡す
            yield return null;                                                      // 無限ループ防止用に1フレーム待たせる
        }
        yield return new WaitForSeconds(0.5f);  // 連続上昇防止用にウェイトを入れておく
        hovUpTime = 0.0f;                     　// 上昇時間をリセットしておく
        hovUp = null;                         　// コルーチン発動フラグをリセットしておく
    }

    // ホバリングタイマー起動処理
    void StartHoveringTimer()
    {
        hovTime = 0f;       // タイマー計測開始
        isHovering = true;  //HoveringフラグをON
    }
}
