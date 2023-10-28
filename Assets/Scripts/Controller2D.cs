using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastController
{
    float maxClimbAngle = 80;
    float maxDescendAngle = 80;

    public CollisionInfo collisions;

    protected override void Start()
    {
        base.Start();
    }
    
    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        collisions.Reset ();
        collisions.velocityOld = velocity;
        
        if (velocity.y < 0)
            DescendSlope(ref velocity);
        if (velocity.x != 0)
            HorizontalCollisions (ref velocity);
        if (velocity.y != 0)
            VerticalCollisions (ref velocity);

        transform.Translate (velocity);
    }

    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign (velocity.x);
        float rayLength = Mathf.Abs (velocity.x) + SkinWidth;
		
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1)?_raycastOrigins.bottomLeft:_raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (_horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * (directionX * rayLength),Color.red);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxClimbAngle)
                {
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance-SkinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }

                if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    velocity.x = (hit.distance - SkinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign (velocity.y);
        float rayLength = Mathf.Abs (velocity.y) + SkinWidth;

        for (int i = 0; i < verticalRayCount; i ++)
        {
            Vector2 rayOrigin = (directionY == -1)?_raycastOrigins.bottomLeft:_raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (_verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * (directionY * rayLength),Color.red);

            if (hit) {
                velocity.y = (hit.distance - SkinWidth) * directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
        
        if (collisions.climbingSlope) {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + SkinWidth;
            Vector2 rayOrigin = (directionX == -1 ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin,Vector2.right * directionX,rayLength,collisionMask);

            if (hit) {
                float slopeAngle = Vector2.Angle(hit.normal,Vector2.up);
                if (slopeAngle != collisions.slopeAngle) {
                    velocity.x = (hit.distance - SkinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

    void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    void DescendSlope(ref Vector3 velocity) {
        float directionX = Mathf.Sign (velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit) {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
                if (Mathf.Sign(hit.normal.x) == directionX) {
                    if (hit.distance - SkinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
                        velocity.y -= descendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector3 velocityOld;

        public void Reset() {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
