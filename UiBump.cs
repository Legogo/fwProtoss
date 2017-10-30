using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiBump : UiAnimation {

  public float bumpStr = 1f;
  protected Vector2 scaleBase;

  protected override void animStart()
  {
    base.animStart();
    scaleBase = owner.localScale;
  }

  protected override void animUpdate()
  {
    base.animUpdate();
    owner.localScale = Vector2.Lerp(scaleBase, scaleBase * bumpStr, getProgress());
  }

  static public void callBump(GameObject obj) {
    
    UiBump bump = obj.GetComponent<UiBump>();
    if (bump != null) bump.play();

  }
}
