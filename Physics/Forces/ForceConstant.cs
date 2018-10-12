using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp
{
  public class ForceConstant : ForceBase
  {

    public ForceConstant(string forceName, Vector2 force) : base(forceName, false)
    {
      _force = force;
    }

    public ForceConstant(string forceName, float strengthX, float strengthY) : base(forceName, false)
    {
      _force.x = strengthX;
      _force.y = strengthY;
    }

    protected override void compute()
    {
      //...
    }
  }

}
