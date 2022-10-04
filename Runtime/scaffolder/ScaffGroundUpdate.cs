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
        protected override void setupLate()
        {
            base.setupLate();

            subUpdate(true);
        }

        protected void subUpdate(bool sub)
        {
            if (sub) ScaffoldMgr.get().sub(this);
            else ScaffoldMgr.get().unsub(this);
        }

        /// <summary>
        /// implem
        /// internal
        /// </summary>
        public void update()
        {
            if (!canUpdate()) return;
            update(Time.deltaTime);
        }

        virtual protected void update(float dt)
        {

        }

        virtual public bool canUpdate() => enabled;
    }
}
