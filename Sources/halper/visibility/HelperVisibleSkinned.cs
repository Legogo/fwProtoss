using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace halper.visibility
{
  public class HelperVisibleSkinned : HelperVisible
  {

    SkinnedMeshRenderer skinRender;
    Material mat;

    public HelperVisibleSkinned(MonoBehaviour parent) : base(parent.transform, parent)
    { }

    protected override void fetchRenders()
    {
      skinRender = _coroutineCarrier.GetComponentInChildren<SkinnedMeshRenderer>();
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

    override public void setVisibility(bool flag)
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

}
