using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.engine.mod
{
    using fwp.engine.scaffolder;

    public interface iModObject
    {
        void modRestart(); // creation, loop
        void modLaunch(); // loop engaged
        void modUpdate(); // frame
        void modEnd(); // loop ended
    }

    /// <summary>
    /// state owner
    /// this auto update after launch() is called
    /// </summary>
    abstract public class ModBase : ScaffGround
    {
        Coroutine coActive = null;

        public List<iModObject> candidates;

        protected override void build()
        {
            base.build();
            
            instance = this;

            candidates = new List<iModObject>();
            
            modCreate();
        }

        /// <summary>
        /// build mod, one time, on startup
        /// </summary>
        virtual protected void modCreate()
        {
            Debug.Log(getStamp() + " create()");
        }

        /// <summary>
        /// when the mod is finished loading
        /// setup the mod
        /// </summary>
        virtual protected void modRestart()
        {
            Debug.Log(getStamp() + " restart() , candidates x"+candidates.Count);
            
            for (int i = 0; i < candidates.Count; i++)
            {
                candidates[i].modRestart();
            }
            
        }
        
        /// <summary>
        /// something must call this to launch the round
        /// </summary>
        virtual protected void modLaunch()
        {
            Debug.Log(getStamp() + " launch()");

            for (int i = 0; i < candidates.Count; i++)
            {
                candidates[i].modLaunch();
            }

            Debug.Assert(coActive == null, "already running ?");

            coActive = StartCoroutine(processActive());
        }

        IEnumerator processActive()
        {
            yield return null;

            Debug.Log(getStamp() + " update activating");

            yield return null;

            while (!isModDone())
            {
                modUpdate();
                yield return null;
            }

            Debug.Log(getStamp() + " <b>MOD IS DONE</b> | update ended");

            modEnded();
        }

        virtual protected void modUpdate()
        {
            for (int i = 0; i < candidates.Count; i++)
            {
                candidates[i].modUpdate();
            }
        }

        abstract public bool isModDone();

        virtual protected void modEnded()
        {
            if(coActive != null)
            {
                StopCoroutine(coActive);
                coActive = null;
            }

            Debug.Log(getStamp() + " ended()");

            for (int i = 0; i < candidates.Count; i++)
            {
                candidates[i].modEnd();
            }
            
        }

        protected void callRestartDelayed(float delay)
        {
            if(delay <= 0f)
            {
                modRestart();
                return;
            }

            StartCoroutine(processDelay(delay, () =>
            {
                modRestart();
            }));
        }

        IEnumerator processDelay(float delay, System.Action onCompletion)
        {
            while(delay > 0f)
            {
                delay -= Time.deltaTime;
                yield return null;
            }

            onCompletion?.Invoke();
        }

        public override string stringify()
        {
            string output = base.stringify();

            if (coActive == null) output += "\n NOT ACTIVE";

            output += "\n candidates x" + candidates.Count;

            return output;
        }

        static public T getMod<T>() where T : ModBase
        {
            return GameObject.FindObjectOfType<T>();
        }

        public bool isLive()
        {
            return coActive != null;
        }

        protected override string solveStampColor()  => "orange";

        static ModBase instance;
        static public ModBase getMod()
        {
            return instance;
        }
    }

}
