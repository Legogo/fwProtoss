using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Follow an object with the original local position offset
/// </summary>

public class CameraFollowStatic : MonoBehaviour {

  public Transform target;
  public Vector3 offsetPos;
  
  public void setTarget(Transform newTarget)
  {
    target = newTarget;
    
    LateUpdate();
  }

  private void LateUpdate()
  {
    if (target == null) return;

    //transform.position = target.TransformPoint(offsetPos);
    transform.position = target.position + offsetPos;
    transform.LookAt(target);
  }

  [ContextMenu("align to camlook anchor")]
  protected void ctxm_align()
  {
    setTarget(HalperFramework.findAnchor("camlook"));
  }
}
