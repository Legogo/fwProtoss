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

        void Awake()
        {
            Debug.Log(getStamp() + " Awake()");

            updater = new ListUpdater<ModObject>();

            modCreate();
        }

        virtual protected void modCreate()
        { }

        virtual public void modRestart()
        {
            for (int i = 0; i < updater.candidates.Count; i++)
            {
                updater.candidates[i].modRestarted();
            }
        }
        
        public void launch()
        {
            modLaunch();

            if (coActive != null) return;
            coActive = StartCoroutine(processActive());
        }

        virtual protected void modLaunch()
        {
            Debug.Log(getStamp() + "| now live");
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
            for (int i = 0; i < updater.candidates.Count; i++)
            {
                updater.candidates[i].update();
            }
        }

        virtual public bool isModDone()
        {
            return true;
        }

        virtual protected void modEnded()
        {
            for (int i = 0; i < updater.candidates.Count; i++)
            {
                updater.candidates[i].modEnded();
            }
        }

        public override string getStamp()
        {
            return "[mod]"+base.getStamp();
        }

        static public T getMod<T>() where T : ModBase
        {
            return GameObject.FindObjectOfType<T>();
        }

    }

}
