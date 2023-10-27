using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    public LayerMask collisionMask;
    
    const float SkinWidth = 0.015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    float _horizontalRaySpacing;
    float _verticalRaySpacing;
    
    BoxCollider2D _collider;
    RaycastOrigins _raycastOrigins;
    public CollisionInfo collisions;
    
    void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing ();
    }
    
    public void Move(Vector3 velocity) {
        UpdateRaycastOrigins ();
        collisions.Reset ();
        
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

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength,Color.red);

            if (hit)
            {
                velocity.x = (hit.distance - SkinWidth) * directionX;
                rayLength = hit.distance;

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
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

            if (hit)
            {
                velocity.y = (hit.distance - SkinWidth) * directionY;
                rayLength = hit.distance;

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(SkinWidth * -2);

        _raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        _raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        _raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        _raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(SkinWidth * -2);
        
        horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

        _horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        _verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }
    
    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
    
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void Reset() {
            above = below = false;
            left = right = false;
        }
    }
}
