using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://docs.unity3d.com/Manual/InverseKinematics.html

public enum IkFollowGroup { Hand, Foot };

[RequireComponent(typeof(Animator))]
public class IkFollowTransform : MonoBehaviour, FthHumanControllerSub
{
  protected Animator animator;

  public bool ikActive = false;
  float weight = 1f;

  public AvatarIKGoal ikGoal;
  public HumanPart humanPart;
  public Transform followObject = null;
  
  protected void moveWeight(float speed)
  {
    weight += Time.deltaTime * speed;
    weight = Mathf.Clamp01(weight);
  }
  
  void OnAnimatorIK()
  {
    if (!enabled) return;
    if (followObject == null) return;
    if (animator == null) return;

    moveWeight(ikActive ? 1f : -1f);
    
    animator.SetIKPositionWeight(ikGoal, weight);
    animator.SetIKRotationWeight(ikGoal, weight);
    animator.SetIKPosition(ikGoal, followObject.position);
    animator.SetIKRotation(ikGoal, followObject.rotation);
  }

  public void eventChangedSkin(Transform skinRoot)
  {
    animator = skinRoot.GetComponentInChildren<Animator>();
    followObject = HalperTransform.findChild(skinRoot, humanPart.ToString());

    if (animator == null) Debug.LogError("no animator ??");
    if (followObject == null) Debug.LogError("no follow ??");
  }
}
