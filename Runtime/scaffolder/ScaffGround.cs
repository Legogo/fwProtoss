using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.engine.scaffolder
{
    using fwp.engine.scaffolder.engineer;

    /// <summary>
    /// not meant to be updatable
    /// use classes from Ingame/Menu logic instead
    /// 
    /// if created at runtime, early() is done right away
    /// setup() and late() will be done in the next frames
    /// </summary>
    abstract public class ScaffGround : MonoBehaviour, iScaffLog
    {
        bool _early;
        bool _ready;

        private void Awake()
        {
            build();
        }

        virtual protected void build()
        {
            if (!EngineBoot.isLoading())
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
            Debug.Assert(!_ready, "nop");

            //Debug.Log(name + " is checking for loading ...");

            while (EngineBoot.isLoading()) yield return null;

            //Debug.Log(name + " is done loading, setuping ...");

            // not twice
            if (!_early) setupEarly();

            yield return null;
            setup();

            yield return null;
            setupLate();
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

            enabled = true;
        }

        private void OnDestroy()
        {
            destroy();
        }
        virtual protected void destroy()
        { }

        public string getStamp()
        {
            return GetType().ToString();
        }

        public string getColoredStamp(string hex)
        {
            return "<color=" + getStampColor() + ">" + getStamp() + "</color>";
        }

        virtual protected string getStampColor() => "gray";

        virtual public string stringify()
        {
            return getStamp();
        }
    }

}