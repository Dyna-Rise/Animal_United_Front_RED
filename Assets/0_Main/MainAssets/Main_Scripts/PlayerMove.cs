using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    //コンポーネント取得
    CharacterController controller;

    [Header("スピード・ジャンプ")]
    public float playerSpeed = 3.0f;
    public float playerJump = 8.0f;

    float initialSpeed;

    [Header("重力")]
    public float gravity = 20.0f;

    Vector3 moveDirection; //事前計算値
    public float inputDirection; //ボタン入力値 (publicに変更)
    float currentInputDirection; //最終的な方向
    float lastInputDirection = 1; //最後の向き

    // ダッシュによって入力方向が上書きされているかを示すフラグ
    bool isDashDirectionOverridden = false;
    float dashInputDirection; // ダッシュ中に強制する入力方向

    bool canMoveInput = true; // 移動入力受付フラグ

    //moveDirectionのプロパティ（読み取り専用）
    public Vector3 MoveDirection
    {
        get { return moveDirection; }
    }

    public float CurrentInputDirection
    {
        get { return currentInputDirection; }
    }

    public float LastInputDirection
    {
        get { return lastInputDirection; }
    }

    //移動ボタンのアクション
    void OnMove(InputValue value)
    {
        // ダッシュ中でなければ通常の入力を受け付け
        if (!isDashDirectionOverridden && canMoveInput)
        {
            Vector2 input = value.Get<Vector2>();
            inputDirection = input.x;
            if(inputDirection > 0)
            {
                lastInputDirection = 1;
                Debug.Log("lastInput:" + lastInputDirection);
            }
            else if(inputDirection < 0)
            {
                lastInputDirection = -1;
            }
        }
        else
        {
            inputDirection = 0;
        }
    }
    //ジャンプボタンのアクション
    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            Jump();
        }
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        initialSpeed = playerSpeed;
    }

    void Update()
    {
        // ダッシュによって入力方向が上書きされていればその値を使用
        if (isDashDirectionOverridden)
        {
            currentInputDirection = dashInputDirection;
            if(currentInputDirection > 0)
            {
                lastInputDirection = 1;
            }
            else if (currentInputDirection < 0)
            {
                lastInputDirection = -1;
            }
        }
        else
        {
            currentInputDirection = inputDirection;
        }

        //見た目の向きの調整
        if (currentInputDirection > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (currentInputDirection < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        //x方向の事前計算
        moveDirection.x = currentInputDirection * playerSpeed;

        //y方向の事前計算
        moveDirection.y -= gravity * Time.deltaTime;

        //事前計算に基づいて移動
        controller.Move(moveDirection * Time.deltaTime);

        //もし地面についていたらy成分は0にリセット
        if (controller.isGrounded) moveDirection.y = 0;
    }

    //ジャンプ
    void Jump()
    {
        //地面にいれば
        if (controller.isGrounded)
        {
            moveDirection.y = playerJump;
        }
    }

    // PlayerDashからダッシュ方向を強制的に設定するメソッド
    public void SetDashDirection(float direction)
    {
        isDashDirectionOverridden = true;
        //inputDirectionが0でも強制的に決まった方向にダッシュさせる
        dashInputDirection = direction;
    }

    // PlayerDashからダッシュ方向の強制を解除するメソッド
    public void ResetDashDirection()
    {
        isDashDirectionOverridden = false;
        dashInputDirection = 0f;
    }

    //移動入力を無効化するメソッド
    public void DisableMoveInput()
    {
        inputDirection = 0;
        canMoveInput = false;
    }

    //移動入力を有効化するメソッド
    public void EnableMoveInput()
    {
        canMoveInput = true;
    }

    //プレイヤーのスピードを初期設定に戻す
    public void ReturnSpeed()
    {
        playerSpeed = initialSpeed;
    }
}
