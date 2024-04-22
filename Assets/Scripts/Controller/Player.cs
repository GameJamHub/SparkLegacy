using System;
using System.Collections;
using System.Collections.Generic;
using Codebase.Core;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private Controller2D m_controller2D;
    [SerializeField] private float m_moveSpeed = 5f;
    [SerializeField] private float m_jumpHeight = 4f;
    [SerializeField] private float m_timeToJumpApex = 0.4f;
    [SerializeField] private float m_accelerationTimeAirborne = 0.2f;
    [SerializeField] private float m_accelerationTimeGrounded = 0.1f;


    private float m_jumpVelocity;
    private float m_gravity;
    private Vector3 m_velocity;
    private Vector2 m_axisValue;
    private bool m_canMove;
    private bool m_isJumpPressed;
    private float m_velocityXSmoothing;

    private void OnEnable()
    {
        GameplayEvents.OnMovement += HandleOnMovement;
        GameplayEvents.OnJump += HandleOnJump;
    }

    private void OnDisable()
    {
        GameplayEvents.OnMovement -= HandleOnMovement;
        GameplayEvents.OnJump -= HandleOnJump;
    }

    private void Start()
    {
        m_gravity = -(2 * m_jumpHeight) / (Mathf.Pow(m_timeToJumpApex, 2));
        m_jumpVelocity = Mathf.Abs(m_gravity) * m_timeToJumpApex;
    }

    private void HandleOnMovement(Vector2 axisValue, bool canMove)
    {
        m_axisValue = axisValue;
        m_canMove = canMove;
    }

    private void HandleOnJump()
    {
        m_isJumpPressed = true;
    }
    
    private void Update()
    {
        if (m_controller2D.collisionInfo.above || m_controller2D.collisionInfo.below)
        {
            m_velocity.y = 0;
        }

        if (m_isJumpPressed && m_controller2D.collisionInfo.below)
        {
            m_velocity.y = m_jumpVelocity;
            m_isJumpPressed = false;
        }

        float targetXVelocity = m_axisValue.x * m_moveSpeed;
        m_velocity.x = Mathf.SmoothDamp(m_velocity.x, targetXVelocity, ref m_velocityXSmoothing, (m_controller2D.collisionInfo.below)?m_accelerationTimeGrounded:m_accelerationTimeAirborne);
        m_velocity.y += m_gravity * Time.deltaTime;
        m_controller2D.Move(m_velocity * Time.deltaTime);
    }
}
