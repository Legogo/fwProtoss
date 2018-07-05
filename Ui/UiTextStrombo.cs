﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiTextStrombo : EngineObject {

  protected Text txt;

  public Color originColor;
  public Color targetColor;

  protected float frame = 0;

  protected override void build()
  {
    base.build();

    txt = GetComponent<Text>();
  }

  public override void updateEngine()
  {
    base.updateEngine();

    if(frame > 0)
    {
      frame--;
      return;
    }

    txt.color = txt.color == originColor ? targetColor : originColor;
    frame = Random.Range(0, 4);
  }
}
