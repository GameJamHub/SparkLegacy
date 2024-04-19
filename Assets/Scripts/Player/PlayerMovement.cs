using System;
using Codebase.Core;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
   [SerializeField] private float m_maxAcceleration = 0.5f;
   [FormerlySerializedAs("m_groundSpeed")] [SerializeField] private float m_maxXSpeed = 2.5f;
   [SerializeField] private float m_jumpSpeed = 3.5f;
   [SerializeField] private float m_deadZoneThreshhold = 0.1f;
   [SerializeField] private float m_jumpForce = 1;
   [Range(0f,1f)] [SerializeField] private float m_groundDrag = 0.9f;
   [SerializeField] private Rigidbody2D m_rigidBody;
   [SerializeField] private BoxCollider2D m_groundCollider;
   [SerializeField] private LayerMask m_groundLayer;

   private bool m_canMove;
   private Vector2 m_axisValue;
   private bool m_isGrounded;

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
   
   private void FixedUpdate()
   {
      CheckGround();
      ApplyFriction();
      Move();
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
}
