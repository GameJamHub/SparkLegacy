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
    [SerializeField] private float m_wallSlideSpeedMax = 3f;
    [SerializeField] private Vector2 m_wallJumpClimb;
    [SerializeField] private Vector2 m_wallJumpOff;
    [SerializeField] private Vector2 m_wallLeap;
    [SerializeField] private float m_wallStickTime = 0.25f;


    private float m_jumpVelocity;
    private float m_gravity;
    private Vector3 m_velocity;
    private Vector2 m_axisValue;
    private bool m_canMove;
    private bool m_isJumpPressed;
    private float m_velocityXSmoothing;
    private float m_timeToWallUnstick;

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
        int wallDirX = (m_controller2D.collisionInfo.left) ? -1 : 1;
        
        float targetXVelocity = m_axisValue.x * m_moveSpeed;
        m_velocity.x = Mathf.SmoothDamp(m_velocity.x, targetXVelocity, ref m_velocityXSmoothing, (m_controller2D.collisionInfo.below)?m_accelerationTimeGrounded:m_accelerationTimeAirborne);
        
        bool wallSliding = false;

        if ((m_controller2D.collisionInfo.left || m_controller2D.collisionInfo.right) && !m_controller2D.collisionInfo.below && m_velocity.y<0)
        {
            wallSliding = true;
            if (m_velocity.y < -m_wallSlideSpeedMax)
            {
                m_velocity.y = -m_wallSlideSpeedMax;
            }

            if (m_timeToWallUnstick > 0)
            {
                m_velocityXSmoothing = 0;
                m_velocity.x = 0;
                if ((int)Mathf.Sign(m_axisValue.x) != wallDirX && m_axisValue.x != 0)
                {
                    m_timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    m_timeToWallUnstick = m_wallStickTime;
                }
            }
            else
            {
                m_timeToWallUnstick = m_wallStickTime;
            }
        }
        
        if (m_controller2D.collisionInfo.above || m_controller2D.collisionInfo.below)
        {
            m_velocity.y = 0;
        }

        if (m_isJumpPressed)
        {
            if (wallSliding)
            {
                if (wallDirX == (int)Mathf.Sign(m_axisValue.x))
                {
                    m_velocity.x = -wallDirX * m_wallJumpClimb.x;
                    m_velocity.y = m_wallJumpClimb.y;
                }
                else if (m_axisValue.x == 0)
                {
                    m_velocity.x = -wallDirX * m_wallJumpOff.x;
                    m_velocity.y = m_wallJumpOff.y;
                }
                else
                {
                    m_velocity.x = -wallDirX * m_wallLeap.x;
                    m_velocity.y = m_wallLeap.y;
                }
            }

            if (m_controller2D.collisionInfo.below)
            {
                m_velocity.y = m_jumpVelocity;
            }
            m_isJumpPressed = false;
        }
        
        m_velocity.y += m_gravity * Time.deltaTime;
        m_controller2D.Move(m_velocity * Time.deltaTime);
    }
}
