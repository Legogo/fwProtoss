﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://docs.unity3d.com/Manual/InverseKinematics.html

[RequireComponent(typeof(Animator))]
public class IkFollowTransform : MonoBehaviour
{
  protected Animator animator;

  public bool ikActive = false;
  float weight = 1f;

  public AvatarIKGoal ikGoal;
  public Transform followObject = null;

  void Start()
  {
    animator = GetComponent<Animator>();
  }

  protected void moveWeight(float speed)
  {
    weight += Time.deltaTime * speed;
    weight = Mathf.Clamp01(weight);
  }
  
  void OnAnimatorIK()
  {
    if (!enabled) return;
    if (followObject == null) return;

    moveWeight(ikActive ? 1f : -1f);
    
    animator.SetIKPositionWeight(ikGoal, weight);
    animator.SetIKRotationWeight(ikGoal, weight);
    animator.SetIKPosition(ikGoal, followObject.position);
    animator.SetIKRotation(ikGoal, followObject.rotation);
  }
  
}
