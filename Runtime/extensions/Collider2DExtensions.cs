using UnityEngine;


static public class Collider2DExtensions
{
  static public Vector3 randomPositionWithinBounds(this BoxCollider2D target, float depth = 0f) {
    return new Vector3(Random.Range(target.bottomLeft().x, target.topRight().x), Random.Range(target.bottomLeft().y, target.topRight().y), depth);
  }

  static public Vector2 bottomLeft(this BoxCollider2D target) {
    Vector2 output;
    output.x = target.bounds.center.x - target.bounds.extents.x;
    output.y = target.bounds.center.y - target.bounds.extents.y;
    return output;
  }
  static public Vector2 topRight(this BoxCollider2D collider)
  {
    Vector2 output;
    output.x = collider.bounds.center.x + collider.bounds.extents.x;
    output.y = collider.bounds.center.y + collider.bounds.extents.y;
    return output;
  }

  static public Collider2D boundsIntersects(this Collider2D collider, Collider2D[] colliders, bool skipDisabled = true)
	{
		if(skipDisabled)
		{
			if(!collider.enabled) return null;

			for(int i = 0; i < colliders.Length; i++)
			{
				if(colliders[i].enabled && colliders[i].bounds.Intersects(collider.bounds))
				{
					return colliders[i];
				}
			}
		}
		else
		{
			for(int i = 0; i < colliders.Length; i++)
			{
				if(collider.bounds.Intersects(colliders[i].bounds))
				{
					return colliders[i];
				}
			}
		}

		return null;
			
	}// intersectWith()

	static public bool boundsIntersects(this Collider2D[] collidersFrom, Collider2D[] collidersTo, out Collider2D colliderFrom, out Collider2D colliderTo, bool skipDisabled = true)
	{
		for(int i = 0; i < collidersFrom.Length; i++)
		{
			colliderFrom = collidersFrom[i];

			colliderTo = colliderFrom.boundsIntersects(collidersTo, skipDisabled);
			
			if(colliderTo != null)
			{
				return true;
			}
		}

		colliderFrom = colliderTo = null;

		return false;

	}// boundsIntersects()

	static public bool boundsIntersects(this Collider2D[] collidersFrom, Collider2D[] collidersTo, bool skipDisabled = true)
	{
		for(int i = 0; i < collidersFrom.Length; i++)
		{
			if(collidersFrom[i].boundsIntersects(collidersTo, skipDisabled) != null)
			{
				return true;
			}
		}

		return false;

	}// boundsIntersects()

}// class Collider2DExtensions
