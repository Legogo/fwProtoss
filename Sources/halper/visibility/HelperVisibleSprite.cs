using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace halper.visibility
{

  public class HelperVisibleSprite : HelperVisible
  {
    SpriteRenderer _spriteRenderDefault;
    //SpriteRenderer[] _spriteRenders;

    public string sortingLayerName = "";

    public HelperVisibleSprite(MonoBehaviour parent) : base(parent.transform, parent)
    { }

    public HelperVisibleSprite(Transform pivot) : base(pivot, null)
    { }

    protected override void fetchRenders()
    {
      _spriteRenderDefault = _t.GetComponent<SpriteRenderer>();
      if (_spriteRenderDefault == null) _spriteRenderDefault = _t.GetComponentInChildren<SpriteRenderer>();
      //_spriteRenders = HalperComponentsGenerics.getComponents<SpriteRenderer>(_t);
    }

    protected override Transform fetchCarrySymbol()
    {
      //if (_renderSprite == null) Debug.LogError(_owner.GetType()+" no render sprite for " + _owner.name, _owner.gameObject);
      if (_spriteRenderDefault == null) return null;
      return _spriteRenderDefault.transform;
    }

    public override void setup()
    {
      base.setup();

      if (sortingLayerName.Length > 0)
      {
        _spriteRenderDefault.sortingLayerName = sortingLayerName;
      }
    }

    public Sprite getSprite() { return _spriteRenderDefault.sprite; }

    public void setSprite(Sprite newSprite)
    {
      if (_spriteRenderDefault == null)
      {
        Debug.LogWarning(GetType() + " trying to assign a sprite to " + _t.name + " but no renderer found", _t);
        return;
      }

      _spriteRenderDefault.sprite = newSprite;
    }

    override public Color getColor()
    {
      return _spriteRenderDefault.color;
    }

    override protected void swapColor(Color col)
    {
      _spriteRenderDefault.color = col;
    }

    override public void setVisibility(bool flag)
    {
      if (_spriteRenderDefault == null)
      {
        Debug.LogWarning("no render sprite for " + _coroutineCarrier.name, _coroutineCarrier.gameObject);
        return;
      }
      _spriteRenderDefault.enabled = flag;
    }

    /* dans le cas d'une animation il vaut mieux inverser le scale plutôt que de taper sur le flip sprite */
    public override void flipHorizontal(int dir)
    {
      //base.flipHorizontal(dir);
      Vector3 flipScale = _spriteRenderDefault.transform.localScale;
      flipScale.x = Mathf.Abs(flipScale.x) * Mathf.Sign(dir);
      _spriteRenderDefault.transform.localScale = flipScale;

      // _render.flipX = dir < 0;
    }

    override public bool isVisible()
    {
      if (_spriteRenderDefault != null) return _spriteRenderDefault.enabled;
      return false;
    }

    public override Bounds getSymbolBounds()
    {
      if (_spriteRenderDefault == null) Debug.LogWarning("no render sprite for <b>" + _coroutineCarrier.name + "</b>", _coroutineCarrier);
      return _spriteRenderDefault.bounds;
    }

    public SpriteRenderer getSprRender()
    {
      return _spriteRenderDefault;
    }
  }

}
