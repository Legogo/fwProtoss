using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiSlide : UiAnimation {
  
  Vector3 originalPosition;

  public RectTransform target;
  public float speed = 3f;

  protected override void build()
  {
    base.build();

    originalPosition = _owner.transform.position;
  }
  
  override public void reset()
  {
    base.reset();
    
    _owner.transform.position = originalPosition;
  }

  protected override void updateUiAnimation()
  {
    base.updateUiAnimation();
    
    transform.localPosition = Vector3.MoveTowards(transform.localPosition, target.transform.position, Time.deltaTime * speed);
  }
}
