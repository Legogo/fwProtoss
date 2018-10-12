﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp
{
  public class ForceGravity : ForceBase
  {

    public ForceGravity(float gravityPower) : base("gravity", false)
    {
      _force.y = gravityPower;
    }

    protected override void compute() { }
  }

}
