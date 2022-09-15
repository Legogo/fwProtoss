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

        List<iScaffMenu> uiCandids = new List<iScaffMenu>(); // all menu/ui objects
        List<iScaffGameplay> ingCandids = new List<iScaffGameplay>(); // all object gameplay

        public enum IngameState { OFF, PRIMED, LIVE, END };
        IngameState state;

        protected override void build()
        {
            base.build();
            mgr = this;

            Debug.Log(GetType() + " exists !");
        }

        public void sub(iScaffMenu elmt)
        {
            if (uiCandids.IndexOf(elmt) < 0) uiCandids.Add(elmt);
        }
        public void unsub(iScaffMenu elmt)
        {
            uiCandids.Remove(elmt);
        }

        public void sub(iScaffGameplay elmt)
        {
            if (ingCandids.IndexOf(elmt) < 0) ingCandids.Add(elmt);
        }
        public void unsub(iScaffGameplay elmt)
        {
            ingCandids.Remove(elmt);
        }

        void Update()
        {
            for (int i = 0; i < uiCandids.Count; i++)
            {
                uiCandids[i].menuUpdate();
            }

            if (state != IngameState.LIVE) return;

            for (int i = 0; i < ingCandids.Count; i++)
            {
                ingCandids[i].gpUpdate();
            }

            for (int i = 0; i < ingCandids.Count; i++)
            {
                ingCandids[i].gpUpdateLate();
            }
        }

        public void roundSetup()
        {
            for (int i = 0; i < ingCandids.Count; i++)
            {
                ingCandids[i].gpSetup();
            }

            state = IngameState.PRIMED;
        }

        public void roundRestart()
        {
            for (int i = 0; i < ingCandids.Count; i++)
            {
                ingCandids[i].gpRestart();
            }

            state = IngameState.LIVE;
        }

        public void roundEnd()
        {
            state = IngameState.END;

            for (int i = 0; i < ingCandids.Count; i++)
            {
                ingCandids[i].gpEnd();
            }
        }
    }

}
