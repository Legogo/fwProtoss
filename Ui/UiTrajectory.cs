using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// https://answers.unity.com/questions/606720/drawing-projectile-trajectory.html
/// </summary>

public class UiTrajectory : EngineObject {
  
  LineRenderer lineRender;
  int verts = 200;

  Vector3 pt = Vector3.zero;

  protected override void build()
  {
    base.build();
    lineRender = gameObject.GetComponent<LineRenderer>(); 
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

    lineRender.positionCount = verts;
    
    Vector3 pos = startPosition;
    Vector3 vel = startVelocity;
    Vector3 grav = getGravityFactor();

    Vector3 prev = pos;

    for(int i = 0; i < verts; i++)
    {
      pt.x = pos.x;
      pt.y = pos.y;
      lineRender.SetPosition(i, pt);
      vel = vel + grav * Time.fixedDeltaTime;
      pos = pos + vel * Time.fixedDeltaTime;

      //traitement spécifique sur les points quand ça se dirige vers le bas
      if(prev.y > pos.y)
      {
        solvePositionGoingDown(pos);
      }

    }

    show();
  }

  virtual public void solvePositionGoingDown(Vector3 pos)
  { }

  virtual public Vector3 getGravityFactor()
  {
    return Physics.gravity;
  }
}
