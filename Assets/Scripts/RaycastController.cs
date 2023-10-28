using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    public LayerMask collisionMask;
    
    protected const float SkinWidth = 0.015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;
    
    protected float _horizontalRaySpacing;
    protected float _verticalRaySpacing;
    
    protected BoxCollider2D _collider;
    protected RaycastOrigins _raycastOrigins;
    
    protected virtual void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing ();
    }
    
    protected void UpdateRaycastOrigins()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(SkinWidth * -2);

        _raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        _raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        _raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        _raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    protected void CalculateRaySpacing()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(SkinWidth * -2);
        
        horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

        _horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        _verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }
    
    protected struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
