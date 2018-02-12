using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : EngineObject
{

  [Header("tweakable")]
  public Vector2 camera_speed_movement = Vector2.one;
  
  Camera followCamera;
  Transform target;

  protected float starting_zoom_level = 0f;
  public Vector3 offset = Vector3.zero;
  protected Vector3 aimPosition;

  protected float zoomOffset = 0f;
  
  protected override void setup()
  {
    base.setup();
    
    followCamera = GetComponent<Camera>();

    starting_zoom_level = getZoomLevel();
  }
  
  public void restart()
  {
    setZoomLevel(starting_zoom_level);
    zoomOffset = 0f;

    solveAimPosition();
    transform.position = aimPosition; // force on target
  }

  public override void updateEngine()
  {
    base.updateEngine();

    update_camera_position();
  }

  void update_camera_position()
  {
    if (target == null) return;

    solveAimPosition();

    float solvedSpeed = Mathf.Lerp(camera_speed_movement.x, camera_speed_movement.y, Mathf.InverseLerp(0f, 10f, Vector2.Distance(transform.position, aimPosition)));

    transform.position = Vector3.MoveTowards(transform.position, aimPosition, solvedSpeed * GameTime.deltaTime);

    updateZoomLevel();
  }

  void solveAimPosition()
  {

    aimPosition = target.position + offset;
    aimPosition.z = transform.position.z; // override z

  }

  void setAtDestination()
  {
    update_camera_position();
    transform.position = aimPosition;
  }

  protected float getZoomLevel()
  {
    if (followCamera.orthographic)
    {
      return followCamera.orthographicSize + zoomOffset;
    }
    else
    {
      return followCamera.fieldOfView + zoomOffset;
    }
  }
  protected void setZoomLevel(float newLevel)
  {
    if (followCamera.orthographic)
    {
      followCamera.orthographicSize = newLevel;
    }
    else
    {
      followCamera.fieldOfView = newLevel;
    }
  }
  protected void updateZoomLevel()
  {

    if (followCamera.orthographic)
    {
      followCamera.orthographicSize = Mathf.MoveTowards(followCamera.orthographicSize, starting_zoom_level + zoomOffset, GameTime.deltaTime);
    }
    else
    {
      followCamera.fieldOfView = Mathf.MoveTowards(followCamera.fieldOfView, starting_zoom_level + zoomOffset, GameTime.deltaTime);
    }
  }


  public void addZoom(float step)
  {
    zoomOffset += step;
  }

  public void setTarget(Transform newTarget)
  {
    target = newTarget;
  }

  public override string toString()
  {
    string ct = base.toString();
    ct += "\n └ offset : " + offset;
    return ct;
  }

  private void OnDrawGizmos()
  {
    if (Application.isPlaying)
    {
      Gizmos.DrawSphere(aimPosition, 0.1f);
    }
    
    Debug.DrawLine(transform.position, aimPosition);
  }
}
