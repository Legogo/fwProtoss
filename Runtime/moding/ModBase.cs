using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.engine.mod
{
    using fwp.engine.scaffolder;

    abstract public class ModBase : ScaffGroundUpdate
    {
        protected override void build()
        {
            base.build();

            _manager = this;
        }

        static protected ModBase _manager;
        static public ModBase getMod() { return _manager; }
        static public T getMod<T>() where T : ModBase
        {
            if (_manager == null)
            {
                _manager = GameObject.FindObjectOfType<T>();
                //if (_manager != null) Debug.Log("<color=yellow>ref game mod is " + _manager.GetType() + "</color>");
                //else Debug.LogWarning("no mods");
            }

            return (T)_manager;
        }

    }

}
