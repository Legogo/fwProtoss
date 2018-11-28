using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelperVisibleSprite : HelperVisible
{

  SpriteRenderer _renderSprite;
  public string sortingLayerName = "";

  public HelperVisibleSprite(EngineObject parent) : base(parent.transform, parent)
  { }

  public HelperVisibleSprite(Transform pivot) : base(pivot, null)
  { }

  protected override Transform fetchCarrySymbol()
  {
    //if (_renderSprite == null) Debug.LogError(_owner.GetType()+" no render sprite for " + _owner.name, _owner.gameObject);
    if (_renderSprite == null) return null;
    return _renderSprite.transform;
  }

  protected override void fetchRenders()
  {
    _renderSprite = _t.GetComponent<SpriteRenderer>();
    if (_renderSprite == null) _renderSprite = _t.GetComponentInChildren<SpriteRenderer>();
  }

  public override void setup()
  {
    base.setup();

    if (sortingLayerName.Length > 0)
    {
      _renderSprite.sortingLayerName = sortingLayerName;
    } 
  }
  
  public Sprite getSprite() { return _renderSprite.sprite; }

  public void setSprite(Sprite newSprite)
  {
    _renderSprite.sprite = newSprite;
  }
  
  override public Color getColor()
  {
    return _renderSprite.color;
  }
  
  override protected void swapColor(Color col)
  {
    _renderSprite.color = col;
  }
  
  override public void setVisibility(bool flag)
  {
    if (_renderSprite == null)
    {
      Debug.LogWarning("no render sprite for " + _coroutineCarrier.name, _coroutineCarrier.gameObject);
      return;
    }
    _renderSprite.enabled = flag;
  }

  /* dans le cas d'une animation il vaut mieux inverser le scale plutôt que de taper sur le flip sprite */
  public override void flipHorizontal(int dir)
  {
    //base.flipHorizontal(dir);
    Vector3 flipScale = _renderSprite.transform.localScale;
    flipScale.x = Mathf.Abs(flipScale.x) * Mathf.Sign(dir);
    _renderSprite.transform.localScale = flipScale;

    // _render.flipX = dir < 0;
  }
  
  override public bool isVisible()
  {
    if (_renderSprite != null) return _renderSprite.enabled;
    return false;
  }

  public override Bounds getSymbolBounds()
  {
    if (_renderSprite == null) Debug.LogWarning("no render sprite for <b>" + _coroutineCarrier.name+"</b>", _coroutineCarrier);
    return _renderSprite.bounds;
  }
}
