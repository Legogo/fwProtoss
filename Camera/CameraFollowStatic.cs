using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Follow an object with the original local position offset
/// </summary>

public class CameraFollowStatic : MonoBehaviour {

  public Transform target;
  public Vector3 offsetPos;
  
  public Vector2 minMaxYPos, minMaxXPos;

  public void setTarget(Transform newTarget)
  {
    target = newTarget;
    
    LateUpdate();
  }

  private void LateUpdate()
  {
    if (target == null) return;

        //transform.position = target.TransformPoint(offsetPos);

        Vector3 targetPos = target.position + offsetPos;

        if (targetPos.y < minMaxYPos.x)
            targetPos.y = minMaxYPos.x;
        else if (targetPos.y > minMaxYPos.y)
            targetPos.y = minMaxYPos.y;

        if (targetPos.x < minMaxXPos.x)
            targetPos.x = minMaxXPos.x;
        else if (targetPos.x > minMaxXPos.y)
            targetPos.x = minMaxXPos.y;


        transform.position = targetPos;
        //transform.LookAt(target);
    }

  [ContextMenu("align to camlook anchor")]
  protected void ctxm_align()
  {
    setTarget(HalperFramework.findAnchor("camlook"));
  }
}
