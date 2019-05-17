using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollowTransform : MonoBehaviour {

  public Transform target;

  public bool dontCopyRotation = false;

	void LateUpdate () {
    if (target == null) return;
    transform.position = target.position;
    if(!dontCopyRotation) transform.rotation = target.rotation;
	}
}
