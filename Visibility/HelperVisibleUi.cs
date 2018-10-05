using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// this manage also Text elements
/// </summary>

public class HelperVisibleUi : HelperVisible
{
  CanvasGroup _group;
  MaskableGraphic _render;
  Text _label;

  public HelperVisibleUi(EngineObject parent) : base(parent)
  {
  }

  protected override void fetchRenders()
  {
    _render = _t.GetComponent<MaskableGraphic>();
    if (_render == null) _render = _t.GetComponentInChildren<MaskableGraphic>();
  }

  protected override Transform fetchCarrySymbol()
  {
    if (_render == null) Debug.LogError("no render ?", _owner.gameObject);
    return _render.transform;
  }

  public override void setup()
  {
    base.setup();

    _group = _t.GetComponent<CanvasGroup>();
    
    _label = _t.GetComponent<Text>();
    if (_label == null) _label = _t.GetComponentInChildren<Text>();

    //override image when label
    if (_label != null) _render = _label;
  }

  public void setTextLabel(string content) {
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
    if(_group != null)
    {
      _group.alpha = flag ? 1f : 0f;
      return;
    }

    _render.enabled = flag;
  }

  public override float getAlpha()
  {
    if(_group != null) return _group.alpha;
    return _render.color.a;
  }
  
  public override Color getColor()
  {
    return _render.color;
  }

  public void setSprite(Sprite spr)
  {
    Image img = _render as Image;
    if (img != null) img.sprite = spr;
  }

  public Image getImage() { return _render as Image; }
  public Text getText() { return _label; }

  /* no bounds for ui */
  public override Bounds getSymbolBounds()
  {
    throw new NotImplementedException();
  }
}
