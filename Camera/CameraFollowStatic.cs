using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Follow an object with the original local position offset
/// </summary>

public class CameraFollowStatic : MonoBehaviour {

  public Transform target;

  protected Vector3 offsetPos;
  protected Quaternion offsetRot;

  private void Start()
  {
    offsetPos = transform.position;
    offsetRot = transform.rotation;
  }

  private void Update()
  {

    //transform.position = target.TransformPoint(offsetPos);
    transform.position = target.position + offsetPos;
    transform.LookAt(target);

  }
}
