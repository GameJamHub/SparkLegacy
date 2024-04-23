using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{
    [Serializable]
    public struct PassengerMovement
    {
        public Transform passengerTransform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform _passengerTransform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform)
        {
            passengerTransform = _passengerTransform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }
    }
    
    [SerializeField] private LayerMask m_passengerMask;
    [SerializeField] private float m_speed;
    [SerializeField] private float m_waitTime;
    [Range(0,2)]
    [SerializeField] private float m_easeAmount;

    public List<PassengerMovement> m_passengerMovements;
    private Dictionary<Transform, Controller2D> m_passengerDictionary = new Dictionary<Transform, Controller2D>();
    private Vector3[] m_globalWaypoints;
    private int m_fromWaypointIndex;
    private float m_percentBetweenWayPoints;
    private float nextMoveTime;
    
    public Vector3[] localWayPoints;
    public bool cyclic;
    
    
    public override void Start()
    {
        base.Start();

        m_globalWaypoints = new Vector3[localWayPoints.Length];
        for (int ind = 0; ind < localWayPoints.Length; ind++)
        {
            m_globalWaypoints[ind] = localWayPoints[ind] + transform.position;
        }
    }

    private void Update()
    {
        UpdateRaycastOrigins();
        Vector3 velocity = CalculatePlatformMovement();
        CalculatePassengerMovement(velocity);
        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
    }

    private float Ease(float x)
    {
        float a = m_easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }
    
    private Vector3 CalculatePlatformMovement()
    {
        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }
        m_fromWaypointIndex %= m_globalWaypoints.Length;
        int toWaypointIndex = (m_fromWaypointIndex + 1)%m_globalWaypoints.Length;
        float distanceBetweenWaypoints =
            Vector3.Distance(m_globalWaypoints[m_fromWaypointIndex], m_globalWaypoints[toWaypointIndex]);
        m_percentBetweenWayPoints += Time.deltaTime * m_speed/distanceBetweenWaypoints;
        m_percentBetweenWayPoints = Mathf.Clamp01(m_percentBetweenWayPoints);
        float easedPercentageBetweenWaypoints = Ease(m_percentBetweenWayPoints);
        Vector3 newPos = Vector3.Lerp(m_globalWaypoints[m_fromWaypointIndex], m_globalWaypoints[toWaypointIndex], easedPercentageBetweenWaypoints);

        if (m_percentBetweenWayPoints >= 1)
        {
            m_percentBetweenWayPoints = 0;
            m_fromWaypointIndex++;
            if (!cyclic)
            {
                if (m_fromWaypointIndex >= m_globalWaypoints.Length - 1)
                {
                    m_fromWaypointIndex = 0;
                    Array.Reverse(m_globalWaypoints);
                }
            }

            nextMoveTime = Time.time + m_waitTime;
        }
        
        return newPos - transform.position;
    }
    
    private void MovePassengers(bool beforeMovePlatform)
    {
        foreach (PassengerMovement passengerMovement in m_passengerMovements)
        {
            if (!m_passengerDictionary.ContainsKey(passengerMovement.passengerTransform))
            {
                m_passengerDictionary.Add(passengerMovement.passengerTransform,passengerMovement.passengerTransform.GetComponent<Controller2D>());
            }
            if (passengerMovement.moveBeforePlatform == beforeMovePlatform)
            {
                m_passengerDictionary[passengerMovement.passengerTransform].Move(passengerMovement.velocity,passengerMovement.standingOnPlatform);
            }
        }
    }

    private void CalculatePassengerMovement(Vector3 velocity)
    {
        m_passengerMovements = new List<PassengerMovement>();
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        // vertically moving transform
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + SKIN_WIDTH * transform.localScale.y;

            for (int vertIndex = 0; vertIndex < m_verticalRayCount; vertIndex++)
            {
                Vector2 rayOrigin = (directionY == -1) ? m_raycastOrigins.bottomLeft : m_raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (m_verticalRaySpacing * vertIndex);
                RaycastHit2D hit =
                    Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, m_passengerMask.value);
                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushY = velocity.y - (hit.distance - SKIN_WIDTH * hit.transform.localScale.y) * directionY;
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        m_passengerMovements.Add(new PassengerMovement(hit.transform,
                            new Vector3(pushX,pushY),
                            directionY == 1,
                            true)
                        );
                    }
                }
            }
        }
        
        //horizontally moving transform

        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + SKIN_WIDTH * transform.localScale.x;

            for (int horizontalIndex = 0; horizontalIndex < m_horizontalRayCount; horizontalIndex++)
            {
                Vector2 rayOrigin = (directionX == -1) ? m_raycastOrigins.bottomLeft : m_raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (m_horizontalRaySpacing * horizontalIndex);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength,
                    m_passengerMask.value);
                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - SKIN_WIDTH * hit.transform.localScale.x) * directionX;
                        float pushY = -SKIN_WIDTH * hit.transform.localScale.y;
                        m_passengerMovements.Add(new PassengerMovement(hit.transform,
                            new Vector3(pushX,pushY),
                            false,
                            true)
                        );
                    }
                }
            }
        }
        
        // passenger on top of a horizontally or downward moving platform

        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = SKIN_WIDTH * transform.localScale.y * 2;

            for (int vertIndex = 0; vertIndex < m_verticalRayCount; vertIndex++)
            {
                Vector2 rayOrigin = m_raycastOrigins.topLeft + Vector2.right * (m_verticalRaySpacing * vertIndex);
                RaycastHit2D hit =
                    Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, m_passengerMask.value);
                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushY = velocity.y;
                        float pushX = velocity.x;
                        m_passengerMovements.Add(new PassengerMovement(hit.transform,
                            new Vector3(pushX,pushY),
                            true,
                            false)
                        );
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (localWayPoints != null)
        {
            Gizmos.color = Color.red;
            float size = 0.3f;

            for (int index = 0; index < localWayPoints.Length; index++)
            {
                Vector3 globalWaypointPosition = (Application.isPlaying)?m_globalWaypoints[index]:localWayPoints[index] + transform.position;
                Gizmos.DrawLine(globalWaypointPosition - Vector3.up * size, globalWaypointPosition + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPosition - Vector3.left * size, globalWaypointPosition + Vector3.left * size);
            }
        }
    }
}
