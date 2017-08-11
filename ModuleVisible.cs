using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleVisible : ArenaObject {

  SpriteRenderer _sprRender;
  Renderer _render;
  Material mat;

  public Renderer[] others;

  protected override void build()
  {
    base.build();

    _render = GetComponent<Renderer>();
    if (_render == null) _render = GetComponentInChildren<Renderer>();

    if(_render == null)
    {
      _sprRender = GetComponent<SpriteRenderer>();
    }

    if (_render == null && _sprRender == null) Debug.LogError("no render ?", gameObject);
    //Debug.Log(_render);

    if(!isSprite())
    {
      mat = _render.material;
      _render.material = mat;
    }

    hide();
  }

  protected bool isSprite()
  {
    return _sprRender != null;
  }

  public override void spawn(Vector3 position)
  {
    base.spawn(position);
    show();
  }

  public void setColor(Color col)
  {
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

    _render.enabled = flag;
    for (int i = 0; i < others.Length; i++)
    {
      others[i].enabled = flag;
    }
  }
}
