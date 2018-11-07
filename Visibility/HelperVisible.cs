using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// mean to be a bridge with some visual representation of the parent
/// lots of basic functions
/// </summary>

abstract public class HelperVisible
{
  protected Rect _bounds; // bounds expressed in local position
  protected Rect _wbounds; // bounds expressed in world position
  protected BoxCollider2D _collider;

  protected MonoBehaviour _owner;
  protected Transform _t;
  protected Transform _symbolCarry;

  protected Coroutine coFade;

  public HelperVisible(MonoBehaviour parent)
  {
    if (parent == null) Debug.LogError("no parent given ...");

    _owner = parent;

    if(_owner.transform == null)
    {
      Debug.LogError(" ? no transform for " + _owner.name);
    }

    _t = _owner.transform;

    _collider = _owner.GetComponent<BoxCollider2D>();
    if (_collider == null) _collider = _owner.GetComponentInChildren<BoxCollider2D>();
    
    setup();
  }

  /* called on build AND at engineobject setup (for symbol loaded) */
  virtual public void setup()
  {
    fetchRenders();
    _symbolCarry = fetchCarrySymbol();
    updateBounds();
  }

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

  virtual public void setAlpha(float newAlpha)
  {
    Color col = getColor();
    col.a = newAlpha;
    swapColor(col);
  }
  
  virtual public float getAlpha()
  {
    return getColor().a;
  }
  
  public void setColor(Color col)
  {
    col.a = getAlpha();
    swapColor(col);
  }
  
  public void show() { setVisibility(true); }
  public void hide() { setVisibility(false); }

  /// <summary>
  /// fade alpha toward given target value (using coroutine), callback returns target alpha on completion
  /// </summary>
  public void fadeByDuration(float targetAlpha, float duration, Action<float> onFadingDone = null, float? startingAlpha = null)
  {
    if(coFade != null)
    {
      _owner.StopCoroutine(coFade);
      Debug.Log("cancelling old fading routine");
    }
    
    coFade = _owner.StartCoroutine(processFadingDuration(targetAlpha, duration, onFadingDone, startingAlpha));
  }

  protected IEnumerator processFadingDuration(float target, float duration, Action<float> onFadingDone = null, float? startingAlpha = null)
  {
    float timer = 0f;
    
    if (startingAlpha != null) setAlpha(startingAlpha.Value);
    else startingAlpha = getAlpha();

    Debug.Log("starting alpha : " + startingAlpha+" , duration ? "+duration);

    if(duration > 0f)
    {
      while (timer < duration)
      {
        timer += GameTime.deltaTime;

        float lerp = timer / duration;
        Debug.Log("  L fading progress : "+lerp+" to "+target);
        setAlpha(Mathf.InverseLerp(startingAlpha.Value, target, lerp));

        yield return null;
      }

    }

    if (onFadingDone != null) onFadingDone(target);

    coFade = null;
  }

  public void fadeBySpeed(float targetAlpha, float speed, Action<float> onFadingDone = null, float? startingAlpha = null)
  {
    if (coFade != null)
    {
      _owner.StopCoroutine(coFade);
      Debug.Log("cancelling old fading routine");
    }

    coFade = _owner.StartCoroutine(processFadingSpeed(targetAlpha, speed, onFadingDone, startingAlpha));
  }

  protected IEnumerator processFadingSpeed(float target, float speed, Action<float> onFadingDone = null, float? startingAlpha = null)
  {
    Debug.Log("start fading " + _owner.name+" to "+target+" at speed "+speed);
    
    if(startingAlpha != null)
    {
      setAlpha(startingAlpha.Value);
    }

    while (getAlpha() != target)
    {
      setAlpha(Mathf.MoveTowards(getAlpha(), target, (speed * Time.deltaTime)));
      yield return null;
    }

    if (onFadingDone != null) onFadingDone(target);

    coFade = null;
  }

  /* ABSTRACTS, specific case based on nature of visual element */

  abstract protected void fetchRenders();
  abstract protected Transform fetchCarrySymbol();

  abstract public Color getColor();
  abstract protected void swapColor(Color col);

  /// <summary>
  /// use hide/show to toggle visibility if possible
  /// </summary>
  abstract public void setVisibility(bool flag);
  abstract public bool isVisible();



  /* local bounds */
  public Rect getRect() { return _bounds; }
  abstract public Bounds getSymbolBounds();
  
  public Bounds getColliderBounds() {
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
    //Debug.Log(_t.localScale);
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
