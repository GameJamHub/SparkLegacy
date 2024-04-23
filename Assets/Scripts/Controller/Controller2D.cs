using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastController
{
    [SerializeField] private float m_maxClimbAngle = 80f;
    [SerializeField] private float m_maxDescendAngle = 80f;
    
    public CollisionInfo collisionInfo;

    public override void Start()
    {
        base.Start();
    }

    public void Move(Vector3 velocity, bool standingOnThePlatform = false)
    {
        UpdateRaycastOrigins();
        collisionInfo.Reset();

        collisionInfo.velocityOld = velocity;
        
        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }
        
        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }

        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }
        transform.Translate(velocity);

        if (standingOnThePlatform)
        {
            collisionInfo.below = true;
        }
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

        if (collisionInfo.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + SKIN_WIDTH * transform.localScale.x;
            Vector2 rayOrigin = ((directionX == -1) ? m_raycastOrigins.bottomLeft : m_raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, m_groundLayer.value);
            if (hit)
            {
                float slopAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopAngle != collisionInfo.slopeAngle)
                {
                    velocity.x = (hit.distance - SKIN_WIDTH * transform.localScale.x) * directionX;
                    collisionInfo.slopeAngle = slopAngle; 
                }
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
                if (hit.distance == 0)
                {
                    continue;
                }
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (horizontalIndex == 0 && slopeAngle <= m_maxClimbAngle)
                {
                    if (collisionInfo.descendingSlope)
                    {
                        collisionInfo.descendingSlope = false;
                        velocity = collisionInfo.velocityOld;
                    }
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

    private void DescendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? m_raycastOrigins.bottomRight : m_raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, m_groundLayer.value);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= m_maxDescendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - SKIN_WIDTH * transform.localScale.y <=
                        Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        collisionInfo.slopeAngle = slopeAngle;
                        collisionInfo.below = true;
                        collisionInfo.descendingSlope = true;
                    }
                }
            }
        }
    }
}
