﻿﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformController : RaycastController
{
	public LayerMask passengerMask;
	public Vector3 move;
	
	protected override void Start ()
	{
		base.Start ();
	}

	void Update ()
	{
		UpdateRaycastOrigins ();

		Vector3 velocity = move * Time.deltaTime;

		MovePassengers(velocity);
		transform.Translate (velocity);
	}

	void MovePassengers(Vector3 velocity)
	{
		HashSet<Transform> movedPassengers = new HashSet<Transform>();

		float directionX = Mathf.Sign (velocity.x);
		float directionY = Mathf.Sign (velocity.y);

		// Vertically moving platform
		if (velocity.y != 0)
		{
			float rayLength = Mathf.Abs (velocity.y) + SkinWidth;
			
			for (int i = 0; i < verticalRayCount; i ++)
			{
				Vector2 rayOrigin = (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
				rayOrigin += Vector2.right * (_verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

				if (hit)
				{
					if (!movedPassengers.Contains(hit.transform))
					{
						movedPassengers.Add(hit.transform);
						float pushX = directionY == 1 ? velocity.x : 0;
						float pushY = velocity.y - (hit.distance - SkinWidth) * directionY;

						hit.transform.Translate(new Vector3(pushX,pushY));
					}
				}
			}
		}

		// Horizontally moving platform
		if (velocity.x != 0)
		{
			float rayLength = Mathf.Abs (velocity.x) + SkinWidth;
			
			for (int i = 0; i < horizontalRayCount; i ++)
			{
				Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
				rayOrigin += Vector2.up * (_horizontalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

				if (hit)
				{
					if (!movedPassengers.Contains(hit.transform))
					{
						movedPassengers.Add(hit.transform);
						float pushX = velocity.x - (hit.distance - SkinWidth) * directionX;
						float pushY = 0;
						
						hit.transform.Translate(new Vector3(pushX,pushY));
					}
				}
			}
		}

		// Passenger on top of a horizontally or downward moving platform
		if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
		{
			float rayLength = SkinWidth * 2;
			
			for (int i = 0; i < verticalRayCount; i ++)
			{
				Vector2 rayOrigin = _raycastOrigins.topLeft + Vector2.right * (_verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);
				
				if (hit)
				{
					if (!movedPassengers.Contains(hit.transform))
					{
						movedPassengers.Add(hit.transform);
						float pushX = velocity.x;
						float pushY = velocity.y;
						
						hit.transform.Translate(new Vector3(pushX,pushY));
					}
				}
			}
		}
	}
}