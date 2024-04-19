using System;
using Codebase.Core;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
   private const string ANIM_IDLE = "idle-Animation";
   private const string ANIM_RUNNING = "Run_Animation";
   private const string ANIM_JUMP = "Jump-Animation";
   
   enum PlayerState
   {
      Idle,
      Running,
      Airborne
   }
   
   [SerializeField] private float m_maxAcceleration = 0.5f;
   [FormerlySerializedAs("m_groundSpeed")] [SerializeField] private float m_maxXSpeed = 2.5f;
   [SerializeField] private float m_jumpSpeed = 3.5f;
   [SerializeField] private float m_deadZoneThreshhold = 0.1f;
   [SerializeField] private float m_jumpForce = 1;
   [Range(0f,1f)] [SerializeField] private float m_groundDrag = 0.9f;
   [SerializeField] private Rigidbody2D m_rigidBody;
   [SerializeField] private BoxCollider2D m_groundCollider;
   [SerializeField] private LayerMask m_groundLayer;
   [SerializeField] private Animator m_animator;

   private bool m_canMove;
   private Vector2 m_axisValue;
   private bool m_isGrounded;
   private PlayerState m_playerState;
   private bool m_stateComplete;

   private void OnEnable()
   {
      GameplayEvents.OnMovement += HandleOnMovement;
      GameplayEvents.OnJump += HandleOnJump;
   }

   private void HandleOnMovement(Vector2 axisValue, bool canMove)
   {
      m_axisValue = axisValue.normalized;
      m_canMove = canMove;
   }

   private void Update()
   {
      if (m_stateComplete)
      {
         SelectState();
      }
      UpdateState();
   }

   private void SelectState()
   {
      m_stateComplete = false;
      if (m_isGrounded)
      {
         if (m_axisValue.x == 0)
         {
            m_playerState = PlayerState.Idle;
            StartIdle();
         }
         else
         {
            m_playerState = PlayerState.Running;
            StartRunning();
         }
      }
      else
      {
         m_playerState = PlayerState.Airborne;
         StartAirborne();
      }
   }

   private void StartIdle()
   {
      m_animator.Play(ANIM_IDLE);
   }

   private void StartRunning()
   {
      m_animator.Play(ANIM_RUNNING);
   }

   private void StartAirborne()
   {
      m_animator.Play(ANIM_JUMP);
   }

   private void UpdateState()
   {
      switch (m_playerState)
      {
         case PlayerState.Idle :
            UpdateIdle();
            break;
         case PlayerState.Running :
            UpdateRunning();
            break;
         case PlayerState.Airborne :
            UpdateAirborne(); 
            break;
      }
   }

   private void UpdateIdle()
   {
      if (!m_isGrounded ||m_axisValue.x != 0)
      {
         m_stateComplete = true;
      }
   }

   private void UpdateRunning()
   {
      m_animator.speed = Mathf.Abs(m_axisValue.x) / m_maxXSpeed;
      
      if (!m_isGrounded || m_axisValue.x == 0)
      {
         m_stateComplete = true;
      }
   }

   private void UpdateAirborne()
   {
      float time = Map(m_rigidBody.velocity.y, m_jumpSpeed * m_jumpForce, -m_jumpSpeed * m_jumpForce, 0, 1, true);
      m_animator.Play(ANIM_JUMP,0,time);
      m_animator.speed = 0;
      if (m_isGrounded)
      {
         m_stateComplete = true;
      }
   }

   private void FixedUpdate()
   {
      CheckGround();
      Move();
      ApplyFriction();
   }

   private void HandleOnJump()
   {
      if (m_isGrounded)
      {
         Jump();
      }
   }

   private void ApplyFriction()
   {
      if (m_isGrounded && m_rigidBody.velocity.y<=0f)
      {
         m_rigidBody.velocity *= m_groundDrag;
      }
   }

   private void Jump()
   {
      m_rigidBody.velocity = new Vector2(m_rigidBody.velocity.x, m_jumpForce * m_jumpSpeed);
   }

   private void Move()
   {
      if (m_canMove && Math.Abs(m_axisValue.x)>m_deadZoneThreshhold)
      {
         float increment = m_axisValue.x * m_maxAcceleration;
         float newSpeed = Mathf.Clamp(m_rigidBody.velocity.x + increment, -m_maxXSpeed, m_maxXSpeed);
         m_rigidBody.velocity = new Vector2(newSpeed, m_rigidBody.velocity.y);
         FaceDirection();
      }
   }

   private void FaceDirection()
   {
      float direction = Mathf.Sign(m_axisValue.x);
      transform.localScale = new Vector3(direction, 1, 1);
   }

   private void CheckGround()
   {
      m_isGrounded =
         Physics2D.OverlapAreaAll(m_groundCollider.bounds.min, m_groundCollider.bounds.max, m_groundLayer.value)
            .Length > 0;
   }
   
   private void OnDisable()
   {
      GameplayEvents.OnMovement -= HandleOnMovement;
      GameplayEvents.OnJump -= HandleOnJump;
   }

   public static float Map(float value, float min1, float max1, float min2, float max2, bool clamp = false)
   {
      float val = min2 + (max2 - min2) * ((value - min1) / (max1 - min1));
      return clamp?Mathf.Clamp(val,Mathf.Min(min2,max2),Mathf.Max(min2,max2)):val;
   }
}
