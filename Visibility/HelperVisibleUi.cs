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
  Image _img;
  Text _label;

  public HelperVisibleUi(MonoBehaviour parent) : base(parent)
  {
  }

  public HelperVisibleUi(CanvasGroup group, MonoBehaviour carry) : base(carry)
  {
    _group = group;
  }

  protected override void fetchRenders()
  {
    _render = _t.GetComponent<MaskableGraphic>();
    if (_render == null) _render = _t.GetComponentInChildren<MaskableGraphic>();
    if (_render as Image) _img = _render as Image;
  }

  protected override Transform fetchCarrySymbol()
  {
    if (_render == null) Debug.LogError("no render ?", _owner.gameObject);
    return _render.transform;
  }

  public override void setup()
  {
    base.setup();

    if(_group == null) _group = _t.GetComponent<CanvasGroup>();
    
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

  override public void setVisibility(bool flag)
  {
    if(_group != null)
    {
      _group.alpha = flag ? 1f : 0f;
      return;
    }

    _render.enabled = flag;
  }

  public override void setAlpha(float newAlpha)
  {
    if (_group != null)
    {
      _group.alpha = newAlpha;
      return;
    }
    base.setAlpha(newAlpha);
  }
  public override float getAlpha()
  {
    if(_group != null) return _group.alpha;
    return _render.color.a;
  }
  
  public override Color getColor()
  {
    if (_group != null) return Color.white;
    if (_render == null) return Color.white;
    return _render.color;
  }

  public void setSymbol(Sprite newFrame)
  {
    Image img = _render as Image;
    if (img != null) img.sprite = newFrame;
  }

  public Image getImage() { return _render as Image; }
  public Text getText() { return _label; }

  /* no bounds for ui */
  public override Bounds getSymbolBounds()
  {
    return _img.sprite.bounds;
  }
}
