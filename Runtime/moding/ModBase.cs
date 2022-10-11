using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.engine.mod
{
    using fwp.engine.scaffolder;

    /// <summary>
    /// state owner
    /// this auto update after launch() is called
    /// </summary>
    public class ModBase : ScaffGround
    {
        public interface ModObject : iListCandidate
        {
            void modRestarted();
            void modEnded();
        }

        Coroutine coActive = null;

        public ListUpdater<ModObject> updater;

        protected override void build()
        {
            base.build();

            updater = new ListUpdater<ModObject>();

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
        /// setup the mod
        /// </summary>
        virtual public void modRestart()
        {
            Debug.Log(getStamp() + " restart()");
            
            if(updater != null)
            {
                for (int i = 0; i < updater.candidates.Count; i++)
                {
                    updater.candidates[i].modRestarted();
                }
            }
            
        }
        
        /// <summary>
        /// starts !
        /// </summary>
        public void launch()
        {
            Debug.Log(getStamp() + " launch()");

            modLaunch();

            if (coActive != null) return;
            coActive = StartCoroutine(processActive());
        }

        virtual protected void modLaunch()
        {
        }

        IEnumerator processActive()
        {
            while (isModDone())
            {
                modUpdate();
                yield return null;
            }

            modEnded();
        }

        virtual protected void modUpdate()
        {
            if(updater != null)
            {
                for (int i = 0; i < updater.candidates.Count; i++)
                {
                    updater.candidates[i].update();
                }
            }
            
        }

        virtual public bool isModDone()
        {
            return true;
        }

        virtual protected void modEnded()
        {
            Debug.Log(getStamp() + " ended()");

            if(updater != null)
            {
                for (int i = 0; i < updater.candidates.Count; i++)
                {
                    updater.candidates[i].modEnded();
                }
            }
            
        }

        static public T getMod<T>() where T : ModBase
        {
            return GameObject.FindObjectOfType<T>();
        }

        protected override string solveStampColor()  => "orange";

    }

}
