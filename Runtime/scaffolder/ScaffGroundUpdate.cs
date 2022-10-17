using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.engine.scaffolder
{
    /// <summary>
    /// this is a helper class
    /// you can implement iScaffUpdate on anything
    /// </summary>
    abstract public class ScaffGroundUpdate : ScaffGround, iScaffUpdate
    {
        //generic update for all scaff updatables
        static ListUpdater<iScaffUpdate> _scaffUpdater;

        int debug_lastFrameUpdate = -1;

        protected override void setupLate()
        {
            base.setupLate();

            subUpdate(true);
        }

        protected void subUpdate(bool sub)
        {
            if (_scaffUpdater == null) _scaffUpdater = new ListUpdater<iScaffUpdate>("scaff");

            if (sub) _scaffUpdater.sub(this);
            else _scaffUpdater.unsub(this);
        }

        /// <summary>
        /// implem
        /// internal
        /// </summary>
        public void update()
        {
            //Debug.Log(getStamp() + " update()");

            if (!canUpdate()) return;

            debug_lastFrameUpdate = Time.frameCount;

            scaffUpdate(Time.deltaTime);
        }

        abstract protected void scaffUpdate(float dt);
        
        virtual public bool canUpdate() => enabled;

        public override string stringify()
        {
            return base.stringify() + " ("+ debug_lastFrameUpdate + ") (update?" + canUpdate() + ")";
        }
    }
}
