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
        bool _debugScene;
        bool _early;
        bool _ready;

        protected void Awake()
        {
            _stampColor = solveStampColor();

            _debugScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene() == gameObject.scene;

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

            yield return null;
            if(_debugScene) setupDebug();
        }

        /// <summary>
        /// if generated at runtime, called during build()
        /// use this to setup more stuff right away after loading
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

        virtual protected void setupDebug()
        {

        }

        private void OnDestroy()
        {
            if (!Application.isPlaying) return;

            onDestroy();
        }

        /// <summary>
        /// only the component
        /// </summary>
        [ContextMenu("destroy")]
        public void destroy()
        {
            GameObject.Destroy(this);
        }

        virtual protected void onDestroy()
        { }

        virtual public string getStamp()
        {
            if(_stampColor.Length > 0) return $"<color={_stampColor}>{GetType()}</color>";
            return GetType().ToString();
        }

        string _stampColor = string.Empty;
        virtual protected string solveStampColor() => "gray";

        virtual public string stringify()
        {
            return getStamp();
        }

        public bool isReady() => _ready;
    }

}