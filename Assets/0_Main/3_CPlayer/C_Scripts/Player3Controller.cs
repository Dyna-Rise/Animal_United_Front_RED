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
    public float hoverUpPower = 5.0f;
    public float hoverUpCoolTime = 1.5f;
    Coroutine hoverUp;

    bool jumpButtonHeld;

    void OnAttack(InputValue value)
    {
        if (!controller.isGrounded) return;
            if (value.isPressed)
        {
            if (!isGurad)
            {
                GuradOn();
                isGurad = true;
            }
            else
            {
                GuradOff();
                isGurad = false;
            }
        }
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerMove = GetComponent<PlayerMove>();
        playerDash = GetComponent<PlayerDash>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame && (hoverUp == null) && playerMove.MoveDirection.y < 0)
            {
                hoverUp = StartCoroutine(HoverUpCol(hoverUpCoolTime));
            }

            if (Keyboard.current.spaceKey.isPressed)
            {
                Hovering();
            }
        }
    }

    void GuradOn()
    {
        if (guradObj == null)
        {
            guradObj = Instantiate(
                guradPrefab,
                gate.transform.position,
                Quaternion.identity
            );
        }

        playerMove.enabled = false;
        playerDash.enabled = false;
    }

    void GuradOff()
    {
        if (guradObj != null)
        {
            Destroy(guradObj);
            playerMove.enabled = true;
            playerDash.enabled = true;
        }
    }
    void Hovering()
    {
        if (playerMove.MoveDirection.y < 0)
        {
            float hoverGravity = playerMove.gravity * hoverGravityScale;
            float hoveringY = playerMove.MoveDirection.y + (playerMove.gravity - hoverGravity) * Time.deltaTime;
            playerMove.SetMoveDirectionY(hoveringY);
        }
    }

    IEnumerator HoverUpCol(float t)
    {
        playerMove.SetMoveDirectionY(hoverUpPower);
        yield return new WaitForSeconds(t);
        hoverUp = null;
    }
}
