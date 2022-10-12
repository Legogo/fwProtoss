using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// base for all objects in the arena framework
/// </summary>

namespace fwp.arena
{
    using fwp.engine.scaffolder;
    using fwp.engine.camera;
    
    abstract public class ArenaObject : ScaffGround, iCameraTarget
    {
        public bool isCameraTarget() => false;

        public Vector3 getPosition()
        {
            return transform.position;
        }

        virtual public void arenaRestart()
        { }

        virtual public void arenaUpdate()
        { }

        virtual public void arenaEnd()
        { }
    }
}
