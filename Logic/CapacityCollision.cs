using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// https://docs.unity3d.com/ScriptReference/Rect.html
/// 
/// xmin,ymin _
///          |_|
///             xmax,ymax
/// </summary>

namespace fwp
{

public class CapacityCollision : LogicCapacity
{
  [HideInInspector] public CapacityCollision[] all; // all other objects in scenes
  [HideInInspector] public BoxCollider2D boxCollider;
  [HideInInspector] public Rect recBound = new Rect(); // expressed in world coordinates

  protected Transform _t;
  
  //step tools
  protected float rayDistance = 5f;
  protected RaycastHit2D hit;
  Vector2 origin = Vector2.zero;
  float min;
  float cornerGap = 0.05f;

  //a l'updateLate on reset donc dans l'inspecteur on voit jamais la valeur
  public CollisionInfo info;
  public LayerMask rayLayer;

  // debug, pour savoir combien de déplacement il y a eu
  private Vector2 frame_h_step;
  private Vector2 frame_v_step;
  private Vector2 frame_last_step;

  protected override void build()
  {
    base.build();

    _t = transform;
  }

  public override void setupCapacity()
  {
    boxCollider = gameObject.GetComponent<BoxCollider2D>();
    if (boxCollider == null) boxCollider = gameObject.GetComponentInChildren<BoxCollider2D>();
    if (boxCollider == null) Debug.LogWarning("no collider for " + name, gameObject);

    if(boxCollider.transform.localScale != Vector3.one)
    {
      Debug.LogError(GetType() + " can't manage scale on collider !", gameObject);
      return;
    }

    recBound = new Rect();
    //destinationBounds = new Rect();

    all = GameObject.FindObjectsOfType<CapacityCollision>();
  }

  private void resetCollisionInfo()
  {

    // reset collision
    info.touching_left = info.touching_right = false;
    info.touching_ground = info.touching_ceiling = false;

  }
  



  /* called on moveStep of capacity movement */
  public Vector2 checkCollisionRaycasts(Vector2 step)
  {
    //debug data
    frame_h_step = frame_v_step = Vector2.zero;
    frame_last_step = step;

    solveBounds();
    
    //Debug.Log(destinationBounds.xMin + " , " + destinationBounds.xMax);

    //offset
    //destinationBounds.x += transform.position.x;
    //destinationBounds.y += transform.position.y;

    resetCollisionInfo();

    boxCollider.enabled = false;

    if(step.y != 0f)
    {
      if (step.y < 0f) checkRaycastVertical(step, Mathf.Abs(step.y), Vector2.down);
      if (step.y > 0f) checkRaycastVertical(step, Mathf.Abs(step.y), Vector2.up);
    }
    if(step.x != 0f)
    {
      if (step.x < 0f) checkRaycastHorizontal(step, Mathf.Abs(step.x), Vector2.left);
      if (step.x > 0f) checkRaycastHorizontal(step, Mathf.Abs(step.x), Vector2.right);
    }

    boxCollider.enabled = true;

    //return recBound.center;
    return recBound.center - boxCollider.offset;
  }
  
  protected void checkRaycastVertical(Vector2 moveStep, float rayDistance, Vector2 rayDir)
  {
    //float absStep = Mathf.Abs(moveStep.y);
    
    bool touchedSomething = false;

    origin.x = recBound.xMin + (recBound.width * 0.5f); // center
    origin.y = rayDir.y < 0f ? recBound.yMax : recBound.yMin;
    if (raycastCheck(origin, rayDir, rayDistance)) touchedSomething = true;

    origin.x = recBound.xMin + cornerGap; // left
    origin.y = rayDir.y < 0f ? recBound.yMax : recBound.yMin;
    if (raycastCheck(origin, rayDir, rayDistance)) touchedSomething = true;

    origin.x = recBound.xMax - cornerGap; // right
    origin.y = rayDir.y < 0f ? recBound.yMax : recBound.yMin;
    if (raycastCheck(origin, rayDir, rayDistance)) touchedSomething = true;

    // on avance que si on a rien touché, sinon ça veut dire qu'il y avait l'espace de se déplacer
    if (touchedSomething)
    {
      //Debug.Log(name+" collision vertical "+rayDir);
      if (rayDir.y < 0) info.touching_ground = true;
      else if (rayDir.y > 0) info.touching_ceiling = true;
    }
    else
    {
      // VERTICAL nothing was touched during step, and this is the raycast that is meant to move the transform
      if (moveStep.y != 0f && (Mathf.Sign(rayDir.y) == Mathf.Sign(moveStep.y)))
      {
        Debug.DrawLine(recBound.center, recBound.center + rayDir * rayDistance, (moveStep.y > 0f) ? Color.magenta : Color.yellow); // up/down
        recBound.center += rayDir * rayDistance;
        frame_v_step += (rayDir * rayDistance); // debug
      }
    }
  }

  protected void checkRaycastHorizontal(Vector2 moveStep, float rayDistance, Vector2 rayDir)
  {
    //float absStep = Mathf.Abs(moveStep.x); // to be sure

    bool touchedSomething = false;

    //Debug.Log(moveStep + " , " + rayDir);
    //Debug.Log(transform.position);

    origin.y = recBound.yMax + (recBound.yMin - recBound.yMax) * 0.5f; // center
    origin.x = rayDir.x < 0f ? recBound.xMin : recBound.xMax;
    if (raycastCheck(origin, rayDir, rayDistance)) touchedSomething = true;

    origin.y = recBound.yMin - cornerGap; // top
    origin.x = rayDir.x < 0f ? recBound.xMin : recBound.xMax;
    if (raycastCheck(origin, rayDir, rayDistance)) touchedSomething = true;

    origin.y = recBound.yMax + cornerGap; // bottom
    origin.x = rayDir.x < 0f ? recBound.xMin : recBound.xMax;
    if (raycastCheck(origin, rayDir, rayDistance)) touchedSomething = true;

    //Debug.Log(transform.position);

    if (touchedSomething)
    {
      //Debug.Log("collision horizontal");
      if (rayDir.x < 0) info.touching_left = true;
      else if (rayDir.x > 0) info.touching_right = true;
    }
    else
    {
      // HORIZONTAL nothing was touched during step, and this is the raycast that is meant to move the transform
      if (moveStep.x != 0f && (Mathf.Sign(rayDir.x) == Mathf.Sign(moveStep.x)))
      {
        Debug.DrawLine(recBound.center, recBound.center + rayDir * rayDistance, Color.yellow); // left/right
        recBound.center += rayDir * rayDistance;
        frame_h_step += (rayDir * rayDistance); // debug
        //Debug.Log(rayDir * absStep);
      }
    }

  }

  /* layer pour caster (utilisé par les horiz/vertic) */
  protected bool raycastCheck(Vector2 origin, Vector2 dir, float distance)
  {
    distance = Mathf.Abs(distance);

    //si a la base l'avatar ne se déplace pas du tout sur cet axe
    //if (distance == 0f) distance = 0.1f;

    Vector2 movement = Vector2.zero;
    Vector2 tmpOrigin = origin;
    Vector2 step = dir * distance;

    Debug.DrawLine(tmpOrigin, tmpOrigin + dir * distance, Color.green); // to show ray

    bool touch = false;
    bool noMove = distance == 0f; // cas specific
    if (dir.sqrMagnitude == 0f) Debug.LogError("dir ??");

    hit = Physics2D.Raycast(origin, dir, distance, rayLayer);

    int safe = 600;
    
    //l'origin est DANS un obstacle, faut reculer jusqu'à trouvé un endroit safe
    while (hit.collider != null && hit.distance == 0f && safe > 0)
    {
      movement -= (noMove ? dir : step) * 0.9f;
      tmpOrigin = origin + movement;

      touch = true;

      hit = Physics2D.Raycast(tmpOrigin, dir, distance, rayLayer);

      Debug.DrawLine(recBound.center, hit.point, Color.white); // to show where is the hit point

      safe--;
    }

    if (safe <= 0) Debug.LogWarning(name+" safe! "+tmpOrigin+" , "+movement+" , "+dir+" , "+step);

    //si j'ai qq chose en face je déplace
    if(hit.collider != null)
    {
      movement += dir * hit.distance;
      recBound.center += movement;
    }
    
    return touch;
  }
  
  public Rect solveBounds()
  {
    //Debug.Log(boxCollider.offset);

    float x = transform.position.x + boxCollider.offset.x;
    //float x = transform.position.x;
    recBound.xMin = x - boxCollider.bounds.extents.x;
    recBound.xMax = x + boxCollider.bounds.extents.x;
    

    float y = transform.position.y + boxCollider.offset.y;
    //float y = transform.position.y;
    recBound.yMin = y + boxCollider.bounds.extents.y;
    recBound.yMax = y - boxCollider.bounds.extents.y;

    //Debug.DrawLine(new Vector3(recBound.xMin, recBound.yMin, 0f), new Vector3(recBound.xMax, recBound.yMax), Color.white, 3f);

    //drawBox(recBound, Color.white);

    return recBound;
  }

  public int GetCollisionDirection
  {
    get { return info.touching_left ? -1 : info.touching_right ? 1 : 0; }
  }
  
  public bool isCollidable()
  {
    if (!enabled)
    {
      //Debug.Log("  not enabled ? "+enabled);
      return false;
    }

    if (boxCollider == null)
    {
      //Debug.Log("  no box collider ?");
      return false;
    }

    if (!boxCollider.enabled)
    {
      //Debug.Log("  box collider not enabled");
      return false;
    }
    return true;
  }
  
  public bool isGrounded()
  {
    return info.touching_ground;
  }

  public bool isTouchingSide()
  {
    return info.touching_left || info.touching_right;
  }

  public bool isRoofed()
  {
    return info.touching_ceiling;
  }

  public bool isTouchingSomething(bool butGround = false)
  {
    if (butGround) return isTouchingSide() || isRoofed();
    return isGrounded() || isTouchingSide() || isRoofed();
  }

  protected void drawBox(Rect rect, Color col)
  {
    Debug.DrawLine(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMax, rect.yMax), col);
    Debug.DrawLine(new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMax, rect.yMin), col);
  }

  void OnDrawGizmos() {
    if (!Application.isPlaying) return;

    //actual bounds
    //solveBounds();
    drawBox(recBound, Color.yellow); // yellow is destination

    // COLLISION
    // drawing vertical / horizontal lines along player collider to show what side was touched

    if (info.touching_left) Debug.DrawLine(new Vector2(recBound.xMin, recBound.yMin), new Vector2(recBound.xMin, recBound.yMax), Color.red);
    if (info.touching_right) Debug.DrawLine(new Vector2(recBound.xMax, recBound.yMin), new Vector2(recBound.xMax, recBound.yMax), Color.red);

    if (info.touching_ceiling) Debug.DrawLine(new Vector2(recBound.xMin, recBound.yMin), new Vector2(recBound.xMax, recBound.yMin), Color.red);
    if (info.touching_ground) Debug.DrawLine(new Vector2(recBound.xMin, recBound.yMax), new Vector2(recBound.xMax, recBound.yMax), Color.red);
    
    //Gizmos.DrawSphere(recBound.center, 0.05f);
  }

  public override string toString()
  {
    string ct = base.toString();

    ct += "\n~touching~";
    ct += "\n " + iStringFormatBool("touching_ceiling", info.touching_ceiling);
    ct += "\n " + iStringFormatBool("touching_left", info.touching_left);
    ct += "\n " + iStringFormatBool("touching_right", info.touching_right);
    ct += "\n " + iStringFormatBool("touching_ground", info.touching_ground);

    if (!isCollidable())
    {
      ct += "\n  ~notCollidable~";
      ct += "\n  " + iStringFormatBool("enabled", enabled);
      ct += "\n  " + iStringFormatBool("boxCollider", boxCollider != null);
      if(boxCollider != null) ct += "\n  " + iStringFormatBool("boxCollider.enabled", boxCollider.enabled);
    }

    ct += "\n~data frame~";
    ct += "\n  frame_step h : " + frame_h_step.x+" x "+frame_h_step.y;
    ct += "\n  frame_step v : " + frame_v_step.x + " x " + frame_v_step.y;
    ct += "\n  step : " + frame_last_step.x + " x " + frame_last_step.y;

    ct += "\n  ~box~";
    ct += "\n"+boxBoundsToString();

    return ct;
  }

  protected string boxBoundsToString()
  {
    if (boxCollider == null) return "no box collider";

    string ct = "";
    ct = recBound.xMin + " x " + recBound.yMin + " ┌ " + boxCollider.bounds.extents.x + " ┐ " + recBound.xMax + " - " + recBound.yMin;
    ct += "\n" + boxCollider.bounds.extents.y + " |     | ";
    ct += "\n" + recBound.xMin + " x " + recBound.yMax + " └ " + boxCollider.bounds.extents.x + " ┘ " + recBound.xMax + " - " + recBound.yMax;

    return ct;
  }

  public override void clean()
  {
  }
}

public struct CollisionInfo
{
  public bool touching_left;
  public bool touching_right;
  public bool touching_ground;
  public bool touching_ceiling;
}


  /*


    public Rect solveDestinationBounds(Vector2 step)
    {
      //solve bounds based on collider and position
      solveBounds();

      //copy
      destinationBounds.xMin = recBound.xMin;
      destinationBounds.xMax = recBound.xMax;
      destinationBounds.yMin = recBound.yMin;
      destinationBounds.yMax = recBound.yMax;

      //displace to have bounds at destination of next movement
      destinationBounds.center += step;

      return destinationBounds;
    }




    public Vector2 checkCollisionRectangle(Vector2 position)
    {
      //mine
      solveBounds();

      resetCollisionInfo();

      var collidedObjects = new List<GameObject>();

      for (int i = 0; i < all.Length; i++)
      {
        CapacityCollision other = all[i];

        if (other != this && other.isCollidable())
        {
          other.solveBounds();

          float gapX = CollisionTools.rayX(recBound, other.recBound);
          float gapY = CollisionTools.rayY(recBound, other.recBound);

          //touched something if x & y are inside the rectangle
          if (gapX != 0f && gapY != 0f)
          {

            //on prend comme ref le gap le plus grand des deux
            if (Mathf.Abs(gapX) < Mathf.Abs(gapY))
            {
              position.x = _t.position.x + gapX;

              info.touching_left = gapX < 0;
              info.touching_right = gapX > 0;
            }
            else
            {
              position.y = _t.position.y + gapY;

              //a inverser ?
              info.touching_ceiling = gapY < 0;
              info.touching_ground = gapY > 0;
            }

            collidedObjects.Add(other.gameObject);
          }
        }
      }
      _collidedObjects = collidedObjects;

      return position;
    }
  */

}