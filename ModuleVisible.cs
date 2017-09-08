﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleVisible : ArenaObject {

  public bool dontHideOnStartup = false;

  SpriteRenderer _sprRender;
  Renderer _render;
  Material mat;

  protected Color original;

  public Renderer[] others;

  protected override void build()
  {
    base.build();

    _render = GetComponent<Renderer>();
    if (_render == null) _render = GetComponentInChildren<Renderer>();

    _sprRender = GetComponent<SpriteRenderer>();
    if (_sprRender == null) _sprRender = GetComponentInChildren<SpriteRenderer>();

    //Debug.Log(name + " -> " + _sprRender + " | " + _render);

    if (_render != null || _sprRender != null)
    {
      
      if (!isSprite())
      {
        mat = _render.material;
        _render.material = mat;
      }

    }

    original = getColor();
    original.a = getAlpha();

    //Debug.Log("ORIGIN : " + original, gameObject);
    if (dontHideOnStartup) return;

    hide();
  }

  protected bool isSprite()
  {
    return _sprRender != null;
  }
  public Sprite getSprite() { return _sprRender.sprite; }

  protected override void spawnProcess(Vector3 position)
  {
    base.spawnProcess(position);
    show();
  }

  public void setSprite(Sprite newSprite)
  {
    if (_sprRender == null) return;

    _sprRender.sprite = newSprite;
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

  public void setOriginColor() {
    //Debug.Log(original, gameObject);
    setColor(original);
    //setAlpha(1f);
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

  public void setColor(Color col)
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

  public override void kill()
  {
    base.kill();
    hide();
  }
  
  public Transform getSymbol() { return transform.GetChild(0); }

  virtual public Renderer getRender() { return _render; }

  public bool isVisible() { return _render.enabled; }
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
  }
}
