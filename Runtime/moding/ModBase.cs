using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.engine.mod
{
    using fwp.engine.scaffolder;

    public interface iModObject
    {
        void modRestart();
        void modUpdate();
        void modEnd();
    }

    /// <summary>
    /// state owner
    /// this auto update after launch() is called
    /// </summary>
    public class ModBase : ScaffGround
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
            Debug.Log(getStamp() + " restart() x"+candidates.Count);
            
            for (int i = 0; i < candidates.Count; i++)
            {
                candidates[i].modRestart();
            }
            
        }
        
        /// <summary>
        /// something must call this to launch the round
        /// </summary>
        virtual public void modLaunch()
        {
            Debug.Log(getStamp() + " launch()");

            Debug.Assert(coActive == null, "already running ?");

            coActive = StartCoroutine(processActive());
        }

        IEnumerator processActive()
        {
            Debug.Log(getStamp() + " update active");

            while (!isModDone())
            {
                modUpdate();
                yield return null;
            }

            Debug.Log(getStamp() + " update ended");

            modEnded();
        }

        virtual protected void modUpdate()
        {
            for (int i = 0; i < candidates.Count; i++)
            {
                candidates[i].modUpdate();
            }
        }

        virtual public bool isModDone()
        {
            return true;
        }

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

        protected override string solveStampColor()  => "orange";

        static ModBase instance;
        static public ModBase getMod()
        {
            return instance;
        }
    }

}
