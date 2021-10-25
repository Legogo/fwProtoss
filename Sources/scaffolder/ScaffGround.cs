using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scaffolder
{
    /// <summary>
    /// not meant to be updatable
    /// use classes from Ingame/Menu logic instead
    /// </summary>
    abstract public class ScaffGround : MonoBehaviour
    {
        private void Awake()
        {
            build();
        }

        virtual protected void build()
        { }

        private IEnumerator Start()
        {
            Debug.Log(name + " is checking for loading ...");

            enabled = false;

            while (scaffolder.ScaffoldMgr.loading) yield return null;

            Debug.Log(name + " is done loading, setuping ...");

            setupEarly();
            yield return null;
            setup();
            yield return null;
            setupLate();
            yield return null;

            enabled = true;
        }

        virtual protected void setupEarly()
        { }
        virtual protected void setup()
        { }
        virtual protected void setupLate()
        { }

        private void OnDestroy()
        {
            destroy();
        }
        virtual protected void destroy()
        { }

    }

}