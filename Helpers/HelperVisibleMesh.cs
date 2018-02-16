using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperVisibleMesh : HelperVisible
{
  Material _mat;

  MeshRenderer _render;
  TextMesh _label;
  
  public float scaleSize = 1f;

  public HelperVisibleMesh(EngineObject parent) : base(parent)
  {
  }

  protected override Transform fetchCarrySymbol()
  {
    return _t.GetChild(0);
  }
  protected override void fetchRenders()
  {

    ///// render
    _render = _t.GetComponent<MeshRenderer>();
    if (_render == null) _render = _t.GetComponentInChildren<MeshRenderer>();

  }

  public override void setup()
  {
    base.setup();

    Debug.Log(_owner.name + " fetching ...");

    _collider = _t.GetComponent<BoxCollider2D>();
    if (_collider == null) _collider = _t.GetComponentInChildren<BoxCollider2D>();

    //no use of module visible if there is nothing to show
    if (_render == null)
    {
      Debug.LogWarning("no render for <b>" + _owner.name+"</b>", _owner.gameObject);
      return;
    }

    if (_render != null)
    {
      //isolate material
      _mat = _render.material;
      _render.material = _mat;
    }

    rescale(scaleSize);

    ///// text

    _label = _t.GetComponent<TextMesh>();
    if (_label != null)
    {
      _render = _label.GetComponent<MeshRenderer>();
    }
  }
  
  public void rescale(float newScale)
  {
    scaleSize = newScale;

    _t.localScale = Vector3.one * scaleSize;

    //use the collider to solve bounds
    if (_collider != null)
    {
      _bounds.center = _collider.bounds.center;
      _bounds.size = _collider.bounds.size;
    }
    else
    {
      //bounds is based on scaleSize (square)
      _bounds.x = -scaleSize * 0.5f;
      _bounds.y = -scaleSize * 0.5f;
      _bounds.width = scaleSize;
      _bounds.height = scaleSize;
    }

    _wbounds.width = _bounds.width;
    _wbounds.height = _bounds.height;
  }

  // on peut pas utiliser transform.localScale a cause de la valeur qui varie quand on change de parent
  public float getRadius() { return scaleSize * 0.5f; }
  
  override public Color getColor()
  {
    if (_mat != null && _mat.HasProperty("_EmissionColor"))
    {
      return _mat.GetColor("_EmissionColor");
    }

    //fallback
    return Color.white;
  }
  
  override protected void swapColor(Color col)
  {
    if (_mat == null) {
      Debug.LogWarning("asking to swap color on null material for "+_owner.name, _owner.gameObject);
      return;
    }

    _mat.SetColor("_EmissionColor", col);
    
  }
  
  public void updateLabelText(string content)
  {
    if (_label != null) _label.text = content;
  }
  
  override protected void setVisibility(bool flag)
  {
    if (_render != null)
    {
      _render.enabled = flag;
    }
    
  }

  override public bool isVisible()
  {
    if (_render != null) return _render.enabled;
    return false;
  }

  public Vector2 getObjectPointNearestToTransform(Transform tr)
  {
    //choppe le point au bord du cercle dans la direction du block
    Vector2 dir = tr.position - _t.position;
    dir = dir.normalized * (getRadius() * 0.6f);
    dir = (Vector2)_t.position + dir;

    Debug.DrawLine(dir, _t.position, Color.green);
    return dir;
  }

#if UNITY_EDITOR
  private void OnDrawGizmosSelected()
  {
    if (Application.isPlaying) return;

    _t.localScale = Vector3.one;

    //draw symbol
    if (_t.childCount > 0)
    {
      Transform tr = _t.GetChild(0);
      tr.localScale = Vector3.one * scaleSize;
    }

  }

  public override Bounds getSymbolBounds()
  {
    return _render.bounds;
  }
#endif

}
