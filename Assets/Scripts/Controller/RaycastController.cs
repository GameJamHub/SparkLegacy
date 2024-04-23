using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    public const float SKIN_WIDTH = 0.05f;
    
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
        public bool descendingSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector3 velocityOld;
        public int faceDir;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
    
    [SerializeField] public BoxCollider2D m_boxCollider;
    [SerializeField] public int m_horizontalRayCount = 4;
    [SerializeField] public int m_verticalRayCount = 4;
    [SerializeField] public LayerMask m_groundLayer;

    [HideInInspector] public float m_horizontalRaySpacing;
    [HideInInspector] public float m_verticalRaySpacing;
    public RaycastOrigins m_raycastOrigins;
    private Bounds m_bounds;
    
    public virtual void Start()
    {
        CalculateRaySpacing();
    }
    
    public void UpdateRaycastOrigins()
    {
        CalculateBounds();
        m_raycastOrigins.bottomLeft = new Vector2(m_bounds.min.x, m_bounds.min.y);
        m_raycastOrigins.topLeft = new Vector2(m_bounds.min.x, m_bounds.max.y);
        m_raycastOrigins.topRight = new Vector2(m_bounds.max.x, m_bounds.max.y);
        m_raycastOrigins.bottomRight = new Vector2(m_bounds.max.x, m_bounds.min.y);
    }

    public void CalculateRaySpacing()
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
