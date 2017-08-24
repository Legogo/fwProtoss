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
  }
  
  override public void reset()
  {
    base.reset();
    
    _owner.localPosition = originalPosition;
  }

  protected override void updateUiAnimation()
  {
    base.updateUiAnimation();

    //Debug.Log(getProgress());
    _owner.localPosition = Vector3.Lerp(originalPosition, target.localPosition, getProgress());

    //transform.localPosition = Vector3.MoveTowards(transform.localPosition, target.transform.position, Time.deltaTime * speed);
  }
}
