using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.engine.scaffolder.engineer
{
    /// <summary>
    /// pour gérer les callbacks lié au lancemnet du moteur
    /// </summary>
    abstract public class EngineBoot : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        //abstract public void boot();

        /// <summary>
        /// this is called when engine is done loading basic stuff
        /// </summary>
        abstract public void loadingCompleted();
    }

}
