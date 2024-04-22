using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{
    struct PassengerMovement
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

    private List<PassengerMovement> m_passengerMovements;
    private Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();
    
    public Vector3 move;
    
    public override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        UpdateRaycastOrigins();
        Vector3 velocity = move * Time.deltaTime;
        CalculatePassengerMovement(velocity);
        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
    }

    private void MovePassengers(bool beforeMovePlatform)
    {
        foreach (PassengerMovement passengerMovement in m_passengerMovements)
        {
            if (!passengerDictionary.ContainsKey(passengerMovement.passengerTransform))
            {
                passengerDictionary.Add(passengerMovement.passengerTransform,passengerMovement.passengerTransform.GetComponent<Controller2D>());
            }
            if (passengerMovement.moveBeforePlatform == beforeMovePlatform)
            {
                passengerDictionary[passengerMovement.passengerTransform].Move(passengerMovement.velocity,passengerMovement.standingOnPlatform);
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
}
