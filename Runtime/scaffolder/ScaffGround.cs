using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.engine.scaffolder
{
    /// <summary>
    /// not meant to be updatable
    /// use classes from Ingame/Menu logic instead
    /// </summary>
    abstract public class ScaffGround : MonoBehaviour
    {
        bool _early;
        bool _ready;

        private void Awake()
        {
            build();
        }

        virtual protected void build()
        {
            if (!ScaffoldMgr.loading)
            {
                setupEarly();
            }
        }

        void Start()
        {
            enabled = false;

            StartCoroutine(processStart());
        }

        IEnumerator processStart()
        {
            Debug.Assert(_ready == false, "nop");

            Debug.Log(name + " is checking for loading ...");

            while (scaffolder.ScaffoldMgr.loading) yield return null;

            Debug.Log(name + " is done loading, setuping ...");

            if (!_early) setupEarly();

            yield return null;
            setup();
            yield return null;
            setupLate();
            yield return null;

            enabled = true;
        }

        /// <summary>
        /// if generated at runtime, called during build()
        /// </summary>
        virtual protected void setupEarly()
        {
            Debug.Assert(_early == false, "nop");
            _early = true;
        }

        /// <summary>
        /// will be called during Start() frame
        /// </summary>
        virtual protected void setup()
        { }

        virtual protected void setupLate()
        {
            _ready = true;
        }

        private void OnDestroy()
        {
            destroy();
        }
        virtual protected void destroy()
        { }

    }

}