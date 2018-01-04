using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleVisibleSprite : ModuleVisible
{

  SpriteRenderer _render;
  public string sortingLayerName = "";

  protected override void fetchRefs()
  {
    _render = GetComponent<SpriteRenderer>();
    if (_render == null) _render = GetComponentInChildren<SpriteRenderer>();
    
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
