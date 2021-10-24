using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scaffolder
{

    public class ScaffoldUiMgr : ScaffGround
    {
        static public ScaffoldUiMgr mgr;
        List<iScaffMenu> candidates = new List<iScaffMenu>();

        protected override void build()
        {
            base.build();
            mgr = this;
        }

        public void sub(iScaffMenu elmt)
        {
            if (candidates.IndexOf(elmt) < 0) candidates.Add(elmt);
        }
        public void unsub(iScaffMenu elmt)
        {
            candidates.Remove(elmt);
        }

        private void Update()
        {
            for (int i = 0; i < candidates.Count; i++)
            {
                candidates[i].menuUpdate();
            }
        }
    }

}

