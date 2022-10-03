using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Le ModBase manipule le RoundState pour communiquer avec les autres objets
 * */

namespace fwp.engine.mod
{
    using fwp.engine.scaffolder;

    abstract public class ModBase : MonoBehaviour
    {
        private void Awake()
        {
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
