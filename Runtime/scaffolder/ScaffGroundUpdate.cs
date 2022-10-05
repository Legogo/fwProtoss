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
        static ListUpdater<iScaffUpdate> _scaffUpdater;

        protected override void setupLate()
        {
            base.setupLate();

            subUpdate(true);
        }

        protected void subUpdate(bool sub)
        {
            if (_scaffUpdater == null) _scaffUpdater = new ListUpdater<iScaffUpdate>();

            if (sub) _scaffUpdater.sub(this);
            else _scaffUpdater.unsub(this);
        }

        /// <summary>
        /// implem
        /// internal
        /// </summary>
        public void update()
        {
            if (!canUpdate()) return;
            scaffUpdate(Time.deltaTime);
        }

        abstract protected void scaffUpdate(float dt);
        
        virtual public bool canUpdate() => enabled;
    }
}
