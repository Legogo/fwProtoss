using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleVisibleMesh : ModuleVisible
{
  public Renderer[] additionnal;
  Material _mat;

  MeshRenderer _render;
  TextMesh _label;
  
  public float scaleSize = 1f;

  protected override void fetchRefs()
  {
    ///// render
    _render = GetComponent<MeshRenderer>();
    if (_render == null) _render = GetComponentInChildren<MeshRenderer>();

    _collider = GetComponent<BoxCollider2D>();
    if (_collider == null) _collider = GetComponentInChildren<BoxCollider2D>();

    //isolate material
    _mat = _render.material;
    _render.material = _mat;

    rescale(scaleSize);

    ///// text

    _label = transform.GetComponent<TextMesh>();
    if (_label != null)
    {
      _render = _label.GetComponent<MeshRenderer>();
    }
  }
  
  public void rescale(float newScale)
  {
    scaleSize = newScale;

    transform.localScale = Vector3.one * scaleSize;

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
    _mat.SetColor("_EmissionColor", col);
    
    for (int i = 0; i < additionnal.Length; i++)
    {
      additionnal[i].sharedMaterial.SetColor("_EmissionColor", col);
    }
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

    for (int i = 0; i < additionnal.Length; i++)
    {
      additionnal[i].enabled = flag;
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
    Vector2 dir = tr.position - transform.position;
    dir = dir.normalized * (getRadius() * 0.6f);
    dir = (Vector2)transform.position + dir;

    Debug.DrawLine(dir, transform.position, Color.green);
    return dir;
  }

#if UNITY_EDITOR
  private void OnDrawGizmosSelected()
  {
    if (Application.isPlaying) return;

    transform.localScale = Vector3.one;

    //draw symbol
    if (transform.childCount > 0)
    {
      Transform tr = transform.GetChild(0);
      tr.localScale = Vector3.one * scaleSize;
    }

  }
#endif

}
