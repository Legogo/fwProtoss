using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.engine.scaffolder
{
    abstract public class ScaffGroundUpdate : ScaffGround
    {
        abstract public void update();
        virtual public bool canUpdate() => true;
    }
}
