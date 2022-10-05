using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.state
{
    using fwp.engine.scaffolder;

    /// <summary>
    /// state owner
    /// </summary>
    public class StateMachine : ScaffGroundUpdate
    {
        State currState = null;
        State nextState = null;

        static protected StateMachine _manager;

        protected override void build()
        {
            base.build();

            _manager = this;
        }

        /// <summary>
        /// 
        /// </summary>
        virtual protected void modStateChange()
        { }

        protected override void scaffUpdate(float dt)
        {
            // need to swap state
            if(nextState != null && currState.isDone())
            {
                currState = nextState;
                nextState = null;

                modStateChange();
                return;
            }

            // update current
            if (currState != null)
            {
                currState.update();
            }
        }

        protected void queueState<T>() where T : State
        {
            if (isAtState<T>()) return;

            nextState = (T)System.Activator.CreateInstance(typeof(T));

        }

        bool isAtState<T>() where T : State
        {
            if (currState != null)
            {
                if (currState.GetType() == typeof(T))
                {
                    return true;
                }
            }
            return false;
        }

        static public StateMachine get()
        {
            if (_manager != null) return _manager;
            _manager = GameObject.FindObjectOfType<StateMachine>();
            return _manager;
        }

    }

}
