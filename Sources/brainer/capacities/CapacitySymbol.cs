using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using halper.visibility;
using System.Linq;

abstract public class CapacitySymbol : brainer.BrainerLogicCapacity, iVisibilityCandidate
{
  abstract public halper.visibility.VisibilityMode getVisibilityMode();

  public HelperVisible visir;

  protected Animator _anim;

  public override void setupCapacity()
  {
    base.setupCapacity();

    assignSymbolPivot(brain.owner.transform);
  }

  public void assignSymbolPivot(Transform pivot)
  {
    visir = HelperVisible.createVisibility(brain.owner, getVisibilityMode());
    _anim = brain.owner.GetComponentsInChildren<Animator>().FirstOrDefault();
  }

  public override void updateCapacity()
  {
    base.updateCapacity();

    if(_anim != null) updateAnimation();
  }

  virtual protected void updateAnimation()
  { }

  public void playAnimOfName(string animName)
  {
    //Debug.Log(name+" playing " + animName);

    if (_anim == null) return;

    _anim.Play(animName);
  }

  public bool isPlaying(string animName)
  {
    if (_anim == null) return false;
    AnimatorStateInfo state = _anim.GetCurrentAnimatorStateInfo(0);
    if (state.IsName(animName)) return true;
    return false;
  }

  public void show()
  {
    visir.show();
  }

  public void hide()
  {
    visir.hide();
  }

  public Vector2 getFacingDirection()
  {
    return Vector2.zero;
  }
}
