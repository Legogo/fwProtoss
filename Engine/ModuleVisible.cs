using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleVisible : EngineObjectModule
{
  public bool hideOnSpawn = false;
  public bool dontHideOnStartup = false;

  SpriteRenderer _sprRender;
  Renderer _render;
  Material mat;

  public Renderer[] others;

  TextMesh txt;
  Renderer txtRender;
  Text ui_txt;

  public float scaleSize = 1f;
  public string sortingLayerName = "";

  private void Awake()
  {
    ///// render
    _render = GetComponent<Renderer>();
    if (_render == null) _render = GetComponentInChildren<Renderer>();

    _sprRender = GetComponent<SpriteRenderer>();
    if (_sprRender == null) _sprRender = GetComponentInChildren<SpriteRenderer>();
    
    if (_render != null || _sprRender != null)
    {
      
      if (!isSprite())
      {
        mat = _render.material;
        _render.material = mat;
      }

    }

    rescale(scaleSize);

    ///// text

    txt = transform.GetComponent<TextMesh>();
    if (txt != null)
    {
      txtRender = txt.GetComponent<MeshRenderer>();
    }

    ui_txt = GetComponent<Text>();

    if (sortingLayerName.Length > 0)
    {
      if (txtRender != null) txtRender.sortingLayerName = sortingLayerName;
    }
    
    //Debug.Log("ORIGIN : " + original, gameObject);
    if (!dontHideOnStartup)
    {
      hide();
    }
    
  }

  public void rescale(float newScale)
  {
    scaleSize = newScale;

    transform.localScale = Vector3.one * scaleSize;
  }

  // on peut pas utiliser transform.localScale a cause de la valeur qui varie quand on change de parent
  public float getRadius() { return scaleSize * 0.5f; }

  protected bool isSprite()
  {
    return _sprRender != null;
  }
  public Sprite getSprite() { return _sprRender.sprite; }
  
  public void setSprite(Sprite newSprite)
  {
    if (_sprRender == null)
    {
      Debug.LogError("no sprite renderer ?");
      return;
    }

    _sprRender.sprite = newSprite;

    //Debug.Log(name + " was setup with sprite " + newSprite);
  }

  public void setAlpha(float newAlpha)
  {
    Color col = getColor();
    col.a = newAlpha;
    setColor(col);
  }

  public float getAlpha()
  {
    return getColor().a;
  }
  
  public Color getColor()
  {
    if (isSprite())
    {
      return _sprRender.color;
    }
    
    if(mat != null && mat.HasProperty("_EmissionColor"))
    {
      return mat.GetColor("_EmissionColor");
    }

    return Color.white;
  }

  public void changeColor(Color col)
  {
    col.a = getAlpha();
    setColor(col);
  }

  protected void setColor(Color col)
  {
    //Debug.Log(name + " " + col + " " + isSprite());
    
    if (isSprite())
    {
      _sprRender.color = col;
      return;
    }

    mat.SetColor("_EmissionColor", col);

    for (int i = 0; i < others.Length; i++)
    {
      others[i].sharedMaterial.SetColor("_EmissionColor", col);
    }
  }
  
  public Transform getSymbol() { return transform.GetChild(0); }

  virtual public Renderer getRender() { return _render; }
  
  public void updateLabel(string content)
  {
    if (ui_txt != null) ui_txt.text = content;
    if (txt != null) txt.text = content;
  }

  protected void toggleLabel(bool flag)
  {
    if (ui_txt != null) ui_txt.enabled = flag;
    if (txtRender != null) txtRender.enabled = flag;
  }

  virtual public void show()
  {
    setVisibility(true);
  }
  virtual public void hide()
  {
    setVisibility(false);
  }

  protected void setVisibility(bool flag)
  {
    //if(flag) Debug.Log(name + " set visible");

    if (isSprite())
    {
      _sprRender.enabled = flag;
      return;
    }

    if(_render != null)
    {
      _render.enabled = flag;
    }
    
    for (int i = 0; i < others.Length; i++)
    {
      others[i].enabled = flag;
    }

    toggleLabel(flag);
  }

  public bool isVisible()
  {
    if (_render != null) return _render.enabled;
    if (_sprRender != null) return _sprRender.enabled;
    if (ui_txt != null) return ui_txt.enabled;
    if (txtRender != null) return txtRender.enabled;
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

}
