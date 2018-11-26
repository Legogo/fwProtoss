using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperVisibleMesh : HelperVisible
{
  Material _mat;

  MeshRenderer _render;
  TextMesh _label;
  
  public HelperVisibleMesh(EngineObject parent) : base(parent.transform, parent)
  { }

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

    //Debug.Log(_owner.name + " fetching ...");

    _collider = _t.GetComponent<Collider>();
    if (_collider == null) _collider = _t.GetComponentInChildren<Collider>();

    //no use of module visible if there is nothing to show
    if (_render == null)
    {
      Debug.LogWarning("no render for <b>" + _coroutineCarrier.name+"</b>", _coroutineCarrier.gameObject);
      return;
    }

    if (_render != null)
    {
      //isolate material
      _mat = _render.material;
      _render.material = _mat;
    }
    
    ///// text

    _label = _t.GetComponent<TextMesh>();
    if (_label != null)
    {
      _render = _label.GetComponent<MeshRenderer>();
    }
  }
  
  // on peut pas utiliser transform.localScale a cause de la valeur qui varie quand on change de parent
  public float getRadius() { return _render.bounds.extents.x; }
  
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
      Debug.LogWarning("asking to swap color on null material for "+_coroutineCarrier.name, _coroutineCarrier.gameObject);
      return;
    }

    _mat.SetColor("_EmissionColor", col);
    
  }
  
  public void setTextMeshLabel(string content)
  {
    if (_label != null) _label.text = content;
  }
  
  override public void setVisibility(bool flag)
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

  public override Bounds getSymbolBounds()
  {
    return _render.bounds;
  }

#if UNITY_EDITOR
  private void OnDrawGizmosSelected()
  {
    if (Application.isPlaying) return;
  
    //...
  }

#endif

}
