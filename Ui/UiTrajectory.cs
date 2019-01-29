using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// https://answers.unity.com/questions/606720/drawing-projectile-trajectory.html
/// </summary>

public class UiTrajectory : EngineObject {
  
  LineRenderer lineRender;
  float gapFactor = 5f;

  //Vector3 origin;
  Vector3 pt = Vector3.zero;
  List<Vector3> pts = new List<Vector3>();
  Material mat;

  //FthArena fthArena;

  public LayerMask raycastLayer;

  protected override void build()
  {
    base.build();
    lineRender = gameObject.GetComponent<LineRenderer>();
    mat = lineRender.material;
    lineRender.material = mat;
    //lineRender.positionCount = maxVerts;
  }

  protected override void setup()
  {
    base.setup();
    //fthArena = ArenaManager.get<FthArena>();
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
    lineRender.transform.position = startPosition;

    solveAllPoints(startVelocity);

    //Debug.Log(pts.Count);

    lineRender.positionCount = pts.Count;
    for (int i = 0; i < pts.Count; i++)
    {
      lineRender.SetPosition(i, pts[i]);
    }
    
    show();
  }

  public override void updateEngine()
  {
    base.updateEngine();

    if (!lineRender.enabled) return;

    Vector2 offset = mat.GetTextureOffset("_MainTex");
    offset.x -= Time.deltaTime * 1.5f;
    mat.SetTextureOffset("_MainTex", offset);
  }

  protected void solveAllPoints(Vector3 startVelocity)
  {
    //origin = startPosition;

    Vector3 pos = Vector3.zero; // position are local not world based
    Vector3 vel = startVelocity;
    Vector3 grav = getGravityFactor();
    Vector3 prev = pos;
    //Vector3 diff;

    float totalMagnitude = 0f;
    float magn = 0f;

    pt = pos;

    pts.Clear();
    pts.Add(pt);

    int count = 0;

    //lineRender.SetPosition(0, pos);

    //Debug.Log("starting at " + pos);

    int safe = 999;
    bool foundEnd = false;
    while (safe > 0 && !foundEnd)
    {
      //next velocity
      vel = vel + grav * Time.fixedDeltaTime * gapFactor;

      //next position based on new velocity
      pos = pos + vel * Time.fixedDeltaTime * gapFactor;

      //diff = pos - prev;

      pts.Add(pos);
      
      Vector3 diff = pos - prev;
      magn = diff.magnitude;

      totalMagnitude += magn;

      count++;

      //seulement en descente
      if (prev.y > pos.y)
      {
        foundEnd = isEndOfLine(prev, pos, magn);
        //Debug.Log("eol : " + count + " at " + pos);
      }
      
      prev = pos;

      safe--;
    }

    //mat.SetTextureOffset("_MainTex", new Vector2(totalMagnitude * 0.5f, 1f));

    if (safe <= 0) Debug.LogError("safe!");

  }

  virtual protected bool isEndOfLine(Vector3 from, Vector3 to, float magn)
  {
    if (to.y < -40f)
    {
      //Debug.Log("line went under world");
      return true;
    }
    
    RaycastHit hit;

    //Debug.DrawLine(from, to, Color.black, 0.5f);

    //local -> global
    from += transform.position;
    to += transform.position;

    //if (Physics.Raycast(point, Vector3.down, out hit, 100f, raycastLayer))
    if (Physics.SphereCast(from, 0.05f, Vector3.down, out hit, magn, raycastLayer))
    {
      Debug.DrawLine(from, hit.point, Color.red, 0.5f);
      //Debug.Log(from + " | pt : " + hit.point + " | distance : " + hit.distance);
      return true;
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

#if UNITY_EDITOR
  private void OnDrawGizmos()
  {
    if (pts == null) return;
    if (pts.Count <= 0) return;

    for (int i = 0; i < pts.Count; i++)
    {
      Gizmos.DrawSphere(transform.position + pts[i], 0.1f);
    }

    Debug.DrawLine(transform.position + pts[0], transform.position + pts[pts.Count - 1], Color.black);
  }
#endif

}
