using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// base for all objects in the arena framework
/// </summary>

namespace fwp.engine.arena
{
    using fwp.engine.scaffolder;
    using fwp.engine.camera;

    abstract public class ArenaObject : ScaffGround, CameraDynamicZoom.iCameraTarget
    {
        virtual protected void roundRestart() { }
        virtual protected void roundLaunch() { }
        virtual protected void roundUpdate() { }
        virtual protected void roundEnd() { }

        public bool isCameraTarget() => false;

        public Vector3 getPosition()
        {
            return transform.position;
        }
    }
}
