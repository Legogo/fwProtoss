using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperVisibleSkinned : HelperVisible {

  SkinnedMeshRenderer skinRender;
  Material mat;

  public HelperVisibleSkinned(EngineObject parent) : base(parent)
  {
  }

  protected override void fetchRenders()
  {
    skinRender = _owner.GetComponentInChildren<SkinnedMeshRenderer>();
    mat = skinRender.material;
  }

  protected override Transform fetchCarrySymbol()
  {
    return skinRender.transform;
  }

  public override Color getColor()
  {
    return mat.color;
  }

  protected override void swapColor(Color col)
  {
    mat.color = col;
  }

  override protected void setVisibility(bool flag)
  {
    skinRender.enabled = flag;
  }

  public override bool isVisible()
  {
    return skinRender.enabled;
  }

  public override Bounds getSymbolBounds()
  {
    return skinRender.bounds;
  }
}
