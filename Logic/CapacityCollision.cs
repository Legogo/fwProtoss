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
  float cornerGap = 0.03f;

  //a l'updateLate on reset donc dans l'inspecteur on voit jamais la valeur
  public CollisionInfo info;
  public LayerMask rayLayer;

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
  
  public Vector2 checkCollisionRaycasts(Vector2 step)
  {
    solveBounds();
    
    //Debug.Log(destinationBounds.xMin + " , " + destinationBounds.xMax);

    //offset
    //destinationBounds.x += transform.position.x;
    //destinationBounds.y += transform.position.y;

    resetCollisionInfo();

    ///!\
    //comme le déplacement effectif de l'avatar est fait dans le checkRaycast SI il recontre pas d'obstacle
    //il FAUT d'abord check la direction dans laquelle il faut aller (ie : priority pour jump)

    checkRaycastVertical(step, step.y < 0f ? Vector2.down : Vector2.up);
    checkRaycastVertical(step, step.y > 0f ? Vector2.up: Vector2.down);

    checkRaycastHorizontal(step, step.x < 0f ? Vector2.left : Vector2.right);
    checkRaycastHorizontal(step, step.x > 0f ? Vector2.right : Vector2.left);

    //return recBound.center;
    return recBound.center - boxCollider.offset;
  }
  
  protected void checkRaycastVertical(Vector2 moveStep, Vector2 rayDir)
  {
    float absStep = Mathf.Abs(moveStep.y);
    
    bool touchedSomething = false;

    origin.x = recBound.xMin + (recBound.xMax - recBound.xMin) * 0.5f; // center
    origin.y = rayDir.y < 0f ? recBound.yMax : recBound.yMin;
    if (raycastCheck(origin, rayDir, absStep)) touchedSomething = true;

    origin.x = recBound.xMin + cornerGap; // left
    origin.y = rayDir.y < 0f ? recBound.yMax : recBound.yMin;
    if (raycastCheck(origin, rayDir, absStep)) touchedSomething = true;

    origin.x = recBound.xMax - cornerGap; // right
    origin.y = rayDir.y < 0f ? recBound.yMax : recBound.yMin;
    if (raycastCheck(origin, rayDir, absStep)) touchedSomething = true;

    // on avance que si on a rien touché, sinon ça veut dire qu'il y avait l'espace de se déplacer
    if (touchedSomething)
    {
      //Debug.Log(name+" collision vertical "+rayDir);
      if (rayDir.y < 0) info.touching_ground = true;
      else if (rayDir.y > 0) info.touching_ceiling = true;  
    }
    else if (moveStep.y != 0f && (Mathf.Sign(rayDir.y) == Mathf.Sign(moveStep.y)))  // on déplace que si c'est le sens du movement, sinon c'est fait au moment du raycast
    {
      Debug.DrawLine(recBound.center, recBound.center + rayDir * absStep, Color.magenta);
      recBound.center += rayDir * absStep;
    }
    
  }

  protected void checkRaycastHorizontal(Vector2 moveStep, Vector2 rayDir)
  {
    float absStep = Mathf.Abs(moveStep.x); // to be sure

    bool touchedSomething = false;

    origin.y = recBound.yMax + (recBound.yMin - recBound.yMax) * 0.5f; // center
    origin.x = rayDir.x < 0f ? recBound.xMin : recBound.xMax;
    if (raycastCheck(origin, rayDir, absStep)) touchedSomething = true;

    origin.y = recBound.yMin - cornerGap; // top
    origin.x = rayDir.x < 0f ? recBound.xMin : recBound.xMax;
    if (raycastCheck(origin, rayDir, absStep)) touchedSomething = true;

    origin.y = recBound.yMax + cornerGap; // bottom
    origin.x = rayDir.x < 0f ? recBound.xMin : recBound.xMax;
    if (raycastCheck(origin, rayDir, absStep)) touchedSomething = true;
    
    if (touchedSomething)
    {
      //Debug.Log("collision horizontal");
      if (rayDir.x < 0) info.touching_left = true;
      else if (rayDir.x > 0) info.touching_right = true;
    }
    else
    {
      if (moveStep.x != 0f && (Mathf.Sign(rayDir.x) == Mathf.Sign(moveStep.x)))
      {
        Debug.DrawLine(recBound.center, recBound.center + rayDir * absStep, Color.magenta);
        recBound.center += rayDir * absStep;
      }
    }

  }

  protected bool raycastCheck(Vector2 origin, Vector2 dir, float distance)
  {
    distance = Mathf.Abs(distance);

    //si a la base l'avatar ne se déplace pas du tout sur cet axe
    //if (distance == 0f) distance = 0.1f;

    Vector2 movement = Vector2.zero;
    Vector2 tmpOrigin = origin;
    Vector2 step = dir * distance;

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
      Debug.DrawLine(tmpOrigin, tmpOrigin + dir * distance, Color.blue, 0.1f);
      safe--;
    }

    if (safe <= 0) Debug.LogWarning(name+" safe! "+tmpOrigin+" , "+movement+" , "+dir+" , "+step);

    //si j'ai qq chose en face
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

    drawBox(recBound, Color.white);

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
    
    if (info.touching_left) Debug.DrawLine(new Vector2(recBound.xMin, recBound.yMin), new Vector2(recBound.xMin, recBound.yMax), Color.red);
    if (info.touching_right) Debug.DrawLine(new Vector2(recBound.xMax, recBound.yMin), new Vector2(recBound.xMax, recBound.yMax), Color.red);

    if (info.touching_ceiling) Debug.DrawLine(new Vector2(recBound.xMin, recBound.yMin), new Vector2(recBound.xMax, recBound.yMin), Color.red);
    if (info.touching_ground) Debug.DrawLine(new Vector2(recBound.xMin, recBound.yMax), new Vector2(recBound.xMax, recBound.yMax), Color.red);
    
    //Gizmos.DrawSphere(recBound.center, 0.05f);
  }

  public override string toString()
  {
    string ct = base.toString();
    ct += "\n"+info.touching_ceiling;
    ct += "\n" + info.touching_left + " " + info.touching_right;
    ct += "\n" + info.touching_ground;

    if (!isCollidable())
    {
      ct += "\n  ~notCollidable~";
      ct += "\n  " + iStringFormatBool("enabled", enabled);
      ct += "\n  " + iStringFormatBool("boxCollider", boxCollider != null);
      if(boxCollider != null) ct += "\n  " + iStringFormatBool("boxCollider.enabled", boxCollider.enabled);
    }

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
