using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.state
{
    public class State
    {
        int _stateUid;

        bool? active = false; // false = entering, true = active

        public State(int uid)
        {
            _stateUid = uid;

            active = true;

            enter();
        }

        virtual public void enter()
        { }

        public void update()
        {
            if (active == null) return;
            if (!active.Value) return;

            if (checkIfDone())
            {
                exit();
                active = null;
            }
            else updateMod(); //do something this frame
        }

        virtual public void updateMod()
        { }

        virtual protected bool checkIfDone() => true;

        virtual public void exit()
        { }

        public bool isDone() => active == null;

        public bool isUid(int uid) => _stateUid == uid;
    }

}
