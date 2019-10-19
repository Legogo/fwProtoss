using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generic maskable graphic
///   image
///   text (fetched also in children)
/// also manage CanvasGroup
/// </summary>

public class HelperVisibleUi : HelperVisible
{
  Canvas _canvas;
  CanvasGroup _group;
  MaskableGraphic _render;
  Image _img;
  Text _label;

  public enum HelperVisibileUiMode { render, canvas, group }
  HelperVisibileUiMode mode;

  public HelperVisibleUi(MonoBehaviour parent) : base(parent.transform, parent)
  {
  }

  public HelperVisibleUi(CanvasGroup group, MonoBehaviour carry) : base(group.transform, carry)
  {
    _group = group;
  }

  protected override void fetchRenders()
  {
    _canvas = _t.GetComponentInParent<Canvas>();

    if (_group == null) _group = _t.GetComponent<CanvasGroup>();

    //generic
    _render = _t.GetComponent<MaskableGraphic>();
    if (_render == null) _render = _t.GetComponentInChildren<MaskableGraphic>();

    //fetch label
    _label = _t.GetComponent<Text>();
    if (_label == null) _label = _t.GetComponentInChildren<Text>();

    //specific
    _img = _render as Image;
  }

  public HelperVisibleUi setRenderMode(HelperVisibileUiMode mode)
  {
    this.mode = mode;
    return this;
  }

  protected override Transform fetchCarrySymbol()
  {
    if (_render == null) Debug.LogError("no render ?", _coroutineCarrier.gameObject);
    return _render.transform;
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
    switch(mode)
    {
      case HelperVisibileUiMode.canvas: return _canvas.enabled;
      case HelperVisibileUiMode.render: return _render.enabled;
      case HelperVisibileUiMode.group:  return _group.enabled;
      default: return false;
    }
  }

  override public void setVisibility(bool flag)
  {
    switch(mode)
    {
      case HelperVisibileUiMode.group: _group.alpha = flag ? 1f : 0f;break;
      case HelperVisibileUiMode.canvas: _canvas.enabled = flag;break;
      case HelperVisibileUiMode.render: _render.enabled = flag;break;
    }
    if (_label != null) _label.enabled = flag;

    //Debug.Log(_t.name+" -> "+flag+" (m "+mode+") ? c "+_canvas.enabled + " , r " + _render.enabled+" , l "+_label.enabled);
    //Debug.Log(_canvas.name + " , " + _render.name + " , " + _label.name);
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

  public Rect getSymbolDimensions()
  {
    return _img.sprite.rect;
  }
}
