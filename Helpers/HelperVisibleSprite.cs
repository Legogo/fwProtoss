using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelperVisibleSprite : HelperVisible
{

  SpriteRenderer _render;
  public string sortingLayerName = "";

  protected override Transform fetchCarrySymbol()
  {
    return _render.transform;
  }

  public override void setup(EngineObject parent)
  {
    base.setup(parent);

    _render = _t.GetComponent<SpriteRenderer>();
    if (_render == null) _render = _t.GetComponentInChildren<SpriteRenderer>();
    
    if (sortingLayerName.Length > 0)
    {
      _render.sortingLayerName = sortingLayerName;
    } 
  }
  
  public Sprite getSprite() { return _render.sprite; }

  public void setSprite(Sprite newSprite)
  {
    _render.sprite = newSprite;
  }
  
  override public Color getColor()
  {
    return _render.color;
  }
  
  override protected void swapColor(Color col)
  {
    _render.color = col;
  }
  
  override protected void setVisibility(bool flag)
  {
    _render.enabled = flag;
  }

  override public bool isVisible()
  {
    if (_render != null) return _render.enabled;
    return false;
  }
  
}
