using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    private const float SKIN_WIDTH = 0.05f;
    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
    
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public float slopeAngle, slopeAngleOld;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
    
    [SerializeField] private BoxCollider2D m_boxCollider;
    [SerializeField] private int m_horizontalRayCount = 4;
    [SerializeField] private int m_verticalRayCount = 4;
    [SerializeField] private LayerMask m_groundLayer;
    [SerializeField] private float m_maxClimbAngle = 80f;

    private RaycastOrigins m_raycastOrigins;
    private float m_horizontalRaySpacing;
    private float m_verticalRaySpacing;
    private Bounds m_bounds;

    public CollisionInfo collisionInfo;

    private void Start()
    {
        CalculateRaySpacing();
    }

    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        collisionInfo.Reset();
        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }

        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }
        transform.Translate(velocity);
    }

    private void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + SKIN_WIDTH * transform.localScale.y;
        
        for (int vertIndex = 0; vertIndex < m_verticalRayCount; vertIndex++)
        {
            Vector2 rayOrigin = (directionY == -1)? m_raycastOrigins.bottomLeft : m_raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (m_verticalRaySpacing * vertIndex + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength,m_groundLayer.value);
            Debug.DrawRay(rayOrigin, Vector3.up*directionY * rayLength, Color.red);
            if (hit)
            {
                velocity.y = (hit.distance - SKIN_WIDTH * transform.localScale.y) * directionY;
                rayLength = hit.distance;

                if (collisionInfo.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) *
                                 Mathf.Sign(velocity.x);
                }
                
                collisionInfo.above = directionY == 1;
                collisionInfo.below = directionY == -1;
            }
        }
    }

    private void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + SKIN_WIDTH * transform.localScale.x;
        
        for (int horizontalIndex = 0; horizontalIndex < m_horizontalRayCount; horizontalIndex++)
        {
            Vector2 rayOrigin = (directionX == -1)? m_raycastOrigins.bottomLeft : m_raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (m_horizontalRaySpacing * horizontalIndex);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength,m_groundLayer.value);
            Debug.DrawRay(rayOrigin, Vector3.right*directionX * rayLength, Color.red);
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (horizontalIndex == 0 && slopeAngle <= m_maxClimbAngle)
                {
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisionInfo.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - SKIN_WIDTH * transform.localScale.x;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }

                if (!collisionInfo.climbingSlope || slopeAngle > m_maxClimbAngle)
                {
                    velocity.x = (hit.distance - SKIN_WIDTH * transform.localScale.x) * directionX;
                    rayLength = hit.distance;
                    if (collisionInfo.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }
                    collisionInfo.left = directionX == -1;
                    collisionInfo.right = directionX == 1;
                }
            }
        }
    }

    private void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisionInfo.below = true;
            collisionInfo.climbingSlope = true;
            collisionInfo.slopeAngle = slopeAngle;
        }
    }
    
    private void UpdateRaycastOrigins()
    {
        CalculateBounds();
        m_raycastOrigins.bottomLeft = new Vector2(m_bounds.min.x, m_bounds.min.y);
        m_raycastOrigins.topLeft = new Vector2(m_bounds.min.x, m_bounds.max.y);
        m_raycastOrigins.topRight = new Vector2(m_bounds.max.x, m_bounds.max.y);
        m_raycastOrigins.bottomRight = new Vector2(m_bounds.max.x, m_bounds.min.y);
    }

    private void CalculateRaySpacing()
    {
        CalculateBounds();
        m_horizontalRayCount = Mathf.Clamp(m_horizontalRayCount, 2, int.MaxValue);
        m_verticalRayCount = Mathf.Clamp(m_verticalRayCount, 2, int.MaxValue);

        m_horizontalRaySpacing = m_bounds.size.y / (m_horizontalRayCount - 1);
        m_verticalRaySpacing = m_bounds.size.x / (m_verticalRayCount - 1);
    }

    private void CalculateBounds()
    {
         m_bounds = m_boxCollider.bounds;
        
        float expandXAmount = SKIN_WIDTH * transform.localScale.x;
        float expandYAmount = SKIN_WIDTH * transform.localScale.y;
        
        Vector3 boundsMin = m_bounds.min;
        Vector3 boundsMax = m_bounds.max;
        
        boundsMin += new Vector3(expandXAmount, expandYAmount, 0);
        boundsMax -= new Vector3(expandXAmount, expandYAmount, 0);
        
        m_bounds.SetMinMax(boundsMin, boundsMax);
    }
}
