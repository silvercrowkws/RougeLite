using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    PlayerInputActions inputActions;

    /// <summary>
    /// 플레이어의 이동 방향
    /// </summary>
    private Vector2 moveInput;

    /// <summary>
    /// 플레이어의 이동 속도
    /// </summary>
    public float moveSpeed = 3f;

    /// <summary>
    /// 플레이어의 리지드 바디
    /// </summary>
    private Rigidbody2D rb;

    private void Awake()
    {
        Debug.Log($"경로 확인 Save Path: {Application.persistentDataPath}");
        // 진짜로 할때는 경로를 바꿔야?

        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        inputActions.Move.Enable();
        inputActions.Move.Move2D.performed += OnMove;
        inputActions.Move.Move2D.canceled += OnMove;
    }

    private void OnDisable()
    {
        inputActions.Move.Move2D.canceled -= OnMove;
        inputActions.Move.Move2D.performed -= OnMove;
        inputActions.Move.Disable();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"충돌! 충돌한 대상: {collision.gameObject.name}");
    }
}
