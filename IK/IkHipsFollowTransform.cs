using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://docs.unity3d.com/Manual/InverseKinematics.html

public class IkHipsFollowTransform : MonoBehaviour
{
  public bool ikActive = false;

  public Transform hips = null;
  public Transform followObject = null;

  private void Update()
  {
    if (!ikActive) return;
    if (followObject == null || hips == null) return;

    hips.position = followObject.position;
    hips.rotation = followObject.rotation;

  }
  
}
