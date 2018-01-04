using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class ModuleVisible : EngineObjectModule
{
  public bool dontHideOnStartup = false;

  protected Rect _bounds;
  protected Rect _wbounds;
  protected BoxCollider2D _collider;

  private void Awake()
  {
    _collider = GetComponent<BoxCollider2D>();
    if (_collider == null) _collider = GetComponentInChildren<BoxCollider2D>();

    fetchRefs();

    if (!dontHideOnStartup)
    {
      hide();
    }
  }

  abstract protected void fetchRefs();

  virtual public void updateBounds()
  {

    //use the collider to solve bounds
    if (_collider != null)
    {
      _bounds.center = _collider.bounds.center;
      _bounds.size = _collider.bounds.size;
    }

    _wbounds.width = _bounds.width;
    _wbounds.height = _bounds.height;
  }

  public void setAlpha(float newAlpha)
  {
    Color col = getColor();
    col.a = newAlpha;
    setColor(col);
  }
  
  virtual public float getAlpha()
  {
    return getColor().a;
  }
  
  abstract public Color getColor();
  abstract protected void swapColor(Color col);

  public void setColor(Color col)
  {
    col.a = getAlpha();
    swapColor(col);
  }
  
  public Transform getSymbol() { return transform.GetChild(0); }

  public void show() { setVisibility(true); }
  public void hide(){ setVisibility(false); }

  abstract protected void setVisibility(bool flag);
  abstract public bool isVisible();

  /* local bounds */
  public Rect getBounds() { return _bounds; }

  public Rect getWorldBounds()
  {
    _wbounds.x = transform.position.x - _bounds.width * 0.5f;
    _wbounds.y = transform.position.y - _bounds.height * 0.5f;
    return _wbounds;
  }
  

  /*(legacy) try not using these (overhead) */
  protected bool isSprite(){ return this as ModuleVisibleSprite != null;}
  protected bool isUi() { return this as ModuleVisibleUi != null; }

}
