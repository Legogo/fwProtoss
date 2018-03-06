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

    originalPosition = rec.localPosition;
    Debug.Log(name + " original ? " + originalPosition);
  }
  
  override public void reset()
  {
    base.reset();
    
    rec.localPosition = originalPosition;

    //Debug.Log(name + " reset to "+_owner.localPosition, gameObject);
  }

  protected override void animUpdate()
  {
    base.animUpdate();

    //Debug.Log(getProgress());
    rec.localPosition = Vector3.Lerp(originalPosition, target.localPosition, getProgress());
    
    //Debug.Log(name + " moved to " + _owner.localPosition, gameObject);
    //transform.localPosition = Vector3.MoveTowards(transform.localPosition, target.transform.position, Time.deltaTime * speed);
  }
}
