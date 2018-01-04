using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleVisibleUi : ModuleVisible {

  CanvasGroup _group;

  MaskableGraphic _render;
  Text _label;

  protected override void fetchRefs()
  {
    _group = GetComponent<CanvasGroup>();
    _render = GetComponent<Image>();
    if (_render == null) _render = GetComponentInChildren<Image>();

    _label = GetComponent<Text>();
    if (_label == null) _label = GetComponentInChildren<Text>();

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
