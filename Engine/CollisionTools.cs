using UnityEngine;
using System.Collections;

public class CollisionTools {
	
  static public Rect getRect(Rect bounds, BoxCollider c){
    bounds.width = c.bounds.extents.x * 2f;
    bounds.height = c.bounds.extents.z * 2f;
    bounds.x = c.transform.position.x - bounds.width * 0.5f;
    bounds.y = c.transform.position.z - bounds.height * 0.5f;
    return bounds;
  }

  static public Rect getRect(Rect bounds, Transform tr){

    bounds.width = tr.localScale.x;
    bounds.height = tr.localScale.z;

    bounds.x = tr.position.x - bounds.width * 0.5f;
    bounds.y = tr.position.z - bounds.height * 0.5f;

    return bounds;
  }

	//permet de savoir si deux objets se croisent
	static public bool touchXY(Rect a, Rect b){

    if(a.width == 0f || a.height == 0f){
      Debug.LogWarning("<CollisionTools> object A has a size of 0f");
      return false;
    }
    if(b.width == 0f || b.height == 0f){
      Debug.LogWarning("<CollisionTools> object B has a size of 0f");
      return false;
    }

		Vector2 gap = (b.center - a.center);
		gap.x = Mathf.Abs(gap.x);
		gap.y = Mathf.Abs(gap.y);
		
		if(gap.x < a.width * 0.5f + b.width * 0.5f){
			if(gap.y < a.height * 0.5f + b.height * 0.5f){
				return true;
			}
		}
		
		return false;
	}
	
	static public float rayX(Rect a, Rect b){
		float gap = b.center.x - a.center.x;
		float size = (a.width * 0.5f + b.width * 0.5f);
		if(Mathf.Abs(gap) > size)	return 0f;
		return (Mathf.Abs(gap) - size) * Mathf.Sign(gap);
	}

	static public float rayY(Rect a, Rect b){
		float gap = b.center.y - a.center.y;
		float size = (a.height * 0.5f + b.height * 0.5f);
    
		if(Mathf.Abs(gap) > size)	return 0f;
		return (Mathf.Abs(gap) - size) * Mathf.Sign(gap);
	}
}
