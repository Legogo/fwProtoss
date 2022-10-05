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
    using fwp.engine.mod;

    abstract public class ArenaObject : ScaffGround, ModBase.ModObject, iCameraTarget
    {
        public bool isCameraTarget() => false;

        public Vector3 getPosition()
        {
            return transform.position;
        }

        virtual public void modRestarted()
        {
        }

        virtual public void modEnded()
        {
        }

        public void update()
        {
            arenaUpdate();
        }

        virtual protected void arenaUpdate()
        { }
    }
}
