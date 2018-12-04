using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiBump : UiAnimation {

  public float bumpStr = 2f;
  protected Vector3 scaleBase;

  protected override void build()
  {
    base.build();
    scaleBase = rec.transform.localScale;
    if (scaleBase.magnitude == 0f) Debug.LogError("scale is zero");
  }
  
  protected override void animStart()
  {
    base.animStart();
    rec.localScale = scaleBase;
  }

  protected override void animUpdate()
  {
    base.animUpdate();

    rec.transform.localScale = Vector3.Lerp(scaleBase, scaleBase * bumpStr, getProgress());

    //Debug.Log(scaleBase+" , "+ rec.transform.localScale);
  }

  static public void callBump(GameObject obj) {
    
    UiBump bump = obj.GetComponent<UiBump>();
    if (bump != null) bump.play();

  }
}
