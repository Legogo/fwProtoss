﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelperVisibleUi : HelperVisible
{
  CanvasGroup _group;

  MaskableGraphic _render;
  Text _label;

  protected override void fetchRenders()
  {
    _render = _t.GetComponent<Image>();
    if (_render == null) _render = _t.GetComponentInChildren<Image>();
  }

  protected override Transform fetchCarrySymbol()
  {
    return _render.transform;
  }

  public override void setup(EngineObject parent)
  {
    base.setup(parent);

    _group = _t.GetComponent<CanvasGroup>();
    
    _label = _t.GetComponent<Text>();
    if (_label == null) _label = _t.GetComponentInChildren<Text>();

    //override image when label
    if (_label != null) _render = _label;
  }

  public void updateLabelText(string content)
  {
    if (_label != null) _label.text = content;
  }

  protected override void swapColor(Color col)
  {
    _render.color = col;
  }

  public override bool isVisible()
  {
    return _render.enabled;
  }

  protected override void setVisibility(bool flag)
  {
    _render.enabled = flag;
  }

  public override float getAlpha()
  {
    if(_group != null) return _group.alpha;
    return base.getAlpha();
  }

  public override Color getColor()
  {
    return _render.color;
  }

}
