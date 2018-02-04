using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// mean to be a bridge with some visual representation of the parent
/// lots of basic functions
/// </summary>

abstract public class HelperVisible
{
  public bool dontHideOnStartup = false;

  protected Rect _bounds; // bounds expressed in local position
  protected Rect _wbounds; // bounds expressed in world position
  protected BoxCollider2D _collider;

  protected EngineObject _owner;
  protected Transform _t;
  protected Transform _symbolCarry;

  virtual public void setup(EngineObject parent)
  {
    _owner = parent;
    _t = _owner.transform;

    _collider = _owner.GetComponent<BoxCollider2D>();
    if (_collider == null) _collider = _owner.GetComponentInChildren<BoxCollider2D>();

    fetchRenders();
    _symbolCarry = fetchCarrySymbol();

    updateBounds();

    if (!dontHideOnStartup)
    {
      hide();
    }
  }

  abstract protected void fetchRenders();
  abstract protected Transform fetchCarrySymbol();

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
  
  public void show() { setVisibility(true); }
  public void hide(){ setVisibility(false); }

  abstract protected void setVisibility(bool flag);
  abstract public bool isVisible();

  /* local bounds */
  public Rect getRect() { return _bounds; }
  public Bounds getBounds() {
    if(_collider == null) Debug.LogError("no collider for "+_owner.gameObject, _owner.gameObject);
    return _collider.bounds;
  }

  public Rect getWorldBounds()
  {
    _wbounds.x = _t.position.x - _bounds.width * 0.5f;
    _wbounds.y = _t.position.y - _bounds.height * 0.5f;
    return _wbounds;
  }
  
  public Transform getSymbolTransform()
  {
    return _t;
  }

  public Transform getSymbol() { return _symbolCarry; }

  public void setSymbolTransformScale(Vector3 newScale)
  {
    _t.localScale = newScale;
  }

  virtual public void flipHorizontal(int dir)
  {
    Vector3 flipScale = _t.localScale;
    flipScale.x = Mathf.Abs(flipScale.x) * Mathf.Sign(dir);
    _t.localScale = flipScale;
  }

  /*(legacy) try not using these (overhead) */
  protected bool isSprite(){ return this as HelperVisibleSprite!= null;}
  protected bool isUi() { return this as HelperVisibleUi != null; }

}
