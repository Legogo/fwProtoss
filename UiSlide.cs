using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiSlide : UiAnimation {
  
  Vector3 originalPosition;
  
  public RectTransform target;

  protected override void build()
  {
    base.build();

    originalPosition = _owner.localPosition;
    Debug.Log(name + " original ? " + originalPosition);
  }
  
  override public void reset()
  {
    base.reset();
    
    _owner.localPosition = originalPosition;

    //Debug.Log(name + " reset to "+_owner.localPosition, gameObject);
  }

  protected override void updateUiAnimation()
  {
    base.updateUiAnimation();

    //Debug.Log(getProgress());
    _owner.localPosition = Vector3.Lerp(originalPosition, target.localPosition, getProgress());
    
    //Debug.Log(name + " moved to " + _owner.localPosition, gameObject);
    //transform.localPosition = Vector3.MoveTowards(transform.localPosition, target.transform.position, Time.deltaTime * speed);
  }
}
