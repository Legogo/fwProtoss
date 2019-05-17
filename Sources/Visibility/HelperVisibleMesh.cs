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
  {
    //Debug.Log("created " + _render);
  }

  protected override Transform fetchCarrySymbol()
  {
    return _t.GetChild(0);
  }

  /// <summary>
  /// called in build() and later
  /// </summary>
  protected override void fetchRenders()
  {
    //if already setup
    if (_render != null) return;

    ///// render

    //Debug.Log(HalperLogs.gatherDataHierarchyInfo(_t));

    //Debug.Log(_t);
    _render = _t.GetComponent<MeshRenderer>();
    //Debug.Log(_render);
    if (_render == null) _render = _t.GetComponentInChildren<MeshRenderer>();
    //Debug.Log(_render);

    if (_render == null)
    {
      Debug.LogWarning("no mesh fetched for " + _t.name, _t);
      Debug.Log("  L GameObject active ? " + _t.gameObject.activeSelf);
      MeshRenderer[] all = _t.GetComponentsInChildren<MeshRenderer>();
      Debug.Log("  L " + all.Length);
    }
    
  }

  public override void setup()
  {
    base.setup(); //fetch renders, symbolcarry, updatebounds
    
    //no use of module visible if there is nothing to show
    if (_render == null)
    {
      Debug.LogWarning("no render for <b>" + _coroutineCarrier.name+"</b>", _coroutineCarrier.gameObject);
      return;
    }

    if (_render != null)
    {
      //default is shared material
      _mat = _render.sharedMaterial;
      //_render.material = _mat;
    }
    
    ///// text

    _label = _t.GetComponent<TextMesh>();
    if (_label != null)
    {
      _render = _label.GetComponent<MeshRenderer>();
    }
  }

  /// <summary>
  /// make material uniq
  /// </summary>
  public void isolateMaterial()
  {
    _mat = _render.material;
    _render.material = _mat;
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
  
  protected Material getSharedMaterialOfName(string containsName)
  {
    if (_render == null) Debug.LogError("no render ? "+containsName);

    for (int i = 0; i < _render.sharedMaterials.Length; i++)
    {
      if (_render.sharedMaterials[i].name.Contains(containsName)) return _render.sharedMaterials[i];
    }

    Debug.LogWarning("no material with name " + containsName + " found on " + _render, _render.transform);

    return null;
  }
  
  /// <summary>
  /// if name is provided it will be applied to shared material
  /// </summary>
  /// <param name="tex"></param>
  /// <param name="materialName"></param>
  public void setMainTexture(Texture tex, string materialName = "")
  {
    setMainTexture(_render, tex, materialName);
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
  
  static public void setMainTexture(MeshRenderer msh, Texture newTexture, string matName)
  {
    Material targetMaterial = getSharedMaterialOfName(msh, matName);
    if (targetMaterial != null) targetMaterial.mainTexture = newTexture;
  }

  static public Material getSharedMaterialOfName(MeshRenderer _render, string containsName)
  {
    if (_render == null) Debug.LogError("no render ? " + containsName);

    for (int i = 0; i < _render.sharedMaterials.Length; i++)
    {
      if (_render.sharedMaterials[i].name.Contains(containsName)) return _render.sharedMaterials[i];
    }

    //Debug.LogWarning("no material with name " + containsName + " found on " + _render, _render.transform);

    return null;
  }

  static public void setMainTextures(Transform parent, Texture newTexture, string materialContainsName)
  {

    MeshRenderer[] meshs = parent.GetComponentsInChildren<MeshRenderer>();
    for (int i = 0; i < meshs.Length; i++)
    {
      setMainTexture(meshs[i], newTexture, materialContainsName);
    }

  }
}
