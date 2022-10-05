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
            updater = new ListUpdater<ModObject>();

            create();
        }

        virtual protected void create()
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
        { }

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

        static public T getMod<T>() where T : ModBase
        {
            return GameObject.FindObjectOfType<T>();
        }

    }

}
