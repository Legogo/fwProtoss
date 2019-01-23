using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// https://answers.unity.com/questions/606720/drawing-projectile-trajectory.html
/// </summary>

public class UiTrajectory : EngineObject {
  
  LineRenderer lineRender;
  float gapFactor = 2f;
  int maxVerts = 100;

  Vector3 pt = Vector3.zero;

  FthArena fthArena;

  public LayerMask raycastLayer;

  protected override void build()
  {
    base.build();
    lineRender = gameObject.GetComponent<LineRenderer>();
    lineRender.positionCount = maxVerts;
  }

  protected override void setup()
  {
    base.setup();

    fthArena = ArenaManager.get<FthArena>();
  }

  public void show()
  {
    lineRender.enabled = true;
  }

  public void hide()
  {
    lineRender.enabled = false;
  }
  
  public void drawTrajectory(Vector3 startPosition, Vector3 startVelocity)
  {
    transform.position = startPosition;
    
    Vector3 pos = startPosition;
    Vector3 vel = startVelocity;
    Vector3 grav = getGravityFactor();
    Vector3 prev = pos;
    Vector3 pt = pos;

    int count = 0;
    
    //lineRender.SetPosition(0, pos);

    Debug.Log("starting at " + pos);

    int safe = 999;
    bool foundEnd = false;
    while(safe > 0 && !foundEnd)
    {
      //lineRender.positionCount = count+1;
      pt.x = pos.x;
      pt.y = pos.y;
      lineRender.SetPosition(count, pt);

      vel = vel + grav * Time.fixedDeltaTime * gapFactor;
      pos = pos + vel * Time.fixedDeltaTime * gapFactor;
      count++;
      
      //seulement en descente
      if (prev.y > pos.y)
      {
        foundEnd = isEndOfLine(pos);
        Debug.Log("eol : " + count + " at " + pos);
      }
      prev = pos;

      safe--;
    }

    //force all last points to the same position
    for (int i = count; i < lineRender.positionCount; i++)
    {
      //lineRender.SetPosition(i, pos);
    }

    if (safe <= 0) Debug.LogError("safe!");

    Debug.Log(count);

    if(count < maxVerts)
    {
      solveLastPoint(pos);
    }
    
    show();
  }

  virtual protected bool isEndOfLine(Vector3 point)
  {
    if (point.y < -40f) return true;
    
    RaycastHit hit;
    if (Physics.Raycast(point, Vector3.down, out hit, 100f, raycastLayer))
    {
      Debug.DrawLine(point, hit.point, Color.black, 0.5f);

      if (hit.distance < 0.1f)
      {
        Debug.DrawLine(point, hit.point, Color.red, 0.5f);
        Debug.Log(point + " | pt : " + hit.point + " | distance : " + hit.distance);
        //Debug.Log(hit.collider.name, hit.collider.transform);
        return true;
      }
    }

    return false;
  }

  protected RaycastHit raycastFromPosition(Vector3 pt)
  {
    
    return default(RaycastHit);
  }

  virtual public void solveLastPoint(Vector3 pos)
  { }

  virtual public Vector3 getGravityFactor()
  {
    return Physics.gravity;
  }
}
