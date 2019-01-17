using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://docs.unity3d.com/Manual/InverseKinematics.html

  /// <summary>
  /// ik
  /// </summary>

public enum IkFollowGroup { Hand, Foot };

[RequireComponent(typeof(Animator))]
public class IkFollowTransform : MonoBehaviour
{
  protected Animator animator;

  public bool ikActive = false;
  float weight = 1f;
  float speed = 3f;

  public AvatarIKGoal ikGoal;
  public Transform followObject = null;

  private void Awake()
  {
    animator = GetComponent<Animator>();
    if (animator == null) Debug.LogWarning("no animator ? ik won't work");

    setup();
  }
  
  virtual protected void setup()
  {
  }

  protected void moveWeight(float stepSpeed)
  {
    weight += Time.deltaTime * stepSpeed;
    weight = Mathf.Clamp01(weight);
  }
  
  void OnAnimatorIK()
  {
    if (!enabled) return;
    if (followObject == null) return;
    if (animator == null) return;

    moveWeight(ikActive ? speed : -speed);

    //Debug.Log(followObject.name+" , "+weight+" , "+speed, transform);

    animator.SetIKPositionWeight(ikGoal, weight);
    animator.SetIKRotationWeight(ikGoal, weight);
    animator.SetIKPosition(ikGoal, followObject.position);
    animator.SetIKRotation(ikGoal, followObject.rotation);
  }
  
}
