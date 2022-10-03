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
        static public bool loading = true;

        static public ScaffoldMgr mgr;

        List<ScaffGroundUpdate> ingCandids = new List<ScaffGroundUpdate>(); // all object gameplay

        public enum IngameState { OFF, PRIMED, LIVE, END };
        IngameState state;

        protected override void build()
        {
            base.build();
            mgr = this;

            Debug.Log(GetType() + " exists !");
        }

        public void sub(ScaffGroundUpdate candid)
        {
            if (ingCandids.IndexOf(candid) < 0) ingCandids.Add(candid);
        }
        public void unsub(ScaffGroundUpdate candid)
        {
            ingCandids.Remove(candid);
        }

        void Update()
        {
            if (state != IngameState.LIVE) return;

            for (int i = 0; i < ingCandids.Count; i++)
            {
                if(ingCandids[i].canUpdate())
                    ingCandids[i].update();
            }
        }

    }

}
