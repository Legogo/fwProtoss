using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.engine.scaffolder
{
    /// <summary>
    /// have capacity to be updated
    /// meant for a "session" integration (setup, live, end)
    /// </summary>
    public class ScaffoldMgr : ScaffGround
    {
        List<iScaffUpdate> ingCandids = new List<iScaffUpdate>(); // all object gameplay

        protected override void build()
        {
            base.build();
            mgr = this;
        }

        public void sub(iScaffUpdate candid)
        {
            if (ingCandids.IndexOf(candid) < 0) ingCandids.Add(candid);
        }
        public void unsub(iScaffUpdate candid)
        {
            ingCandids.Remove(candid);
        }

        void Update()
        {
            for (int i = 0; i < ingCandids.Count; i++)
            {
                ingCandids[i].update();
            }
        }

        static ScaffoldMgr mgr;
        static public ScaffoldMgr get()
        {
            if(mgr == null)
            {
                mgr = new GameObject("{scaffolder}").AddComponent<ScaffoldMgr>();
            }
            return mgr;
        }
    }

}
