using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace brainer
{
    /// <summary>
    /// Le bridge qui gère et update les capacities
    /// Le lien entre un objet du jeu et ses capacités
    /// </summary>
    public class BrainerLogics : MonoBehaviour
    {
        public iBrainCandidate owner;
        public Transform tr; // pivot ?

        //capacities will subscribe to this List on their constructor
        protected List<BrainerLogicCapacity> capacities = new List<BrainerLogicCapacity>();

        public void assign(iBrainCandidate owner)
        {
            this.owner = owner;

            MonoBehaviour mono = owner as MonoBehaviour;
            if (mono != null) tr = mono.transform;
        }

        public void subKappa(BrainerLogicCapacity kappa)
        {
            if (capacities.IndexOf(kappa) > -1) return;
            capacities.Add(kappa);
        }

        /// <summary>
        /// les capacities sont ref quand le brain boot
        /// </summary>
        public T getCapacity<T>() where T : BrainerLogicCapacity
        {
            T tar = (T)capacities.FirstOrDefault(x => x != null && typeof(T).IsAssignableFrom(x.GetType()));
            if (tar != null) return tar;

            tar = tr.GetComponentInChildren<T>();
            if(tar != null)
            {
                subKappa(tar);
            }
            
            return tar;
        }

        virtual public void brainSetup()
        {
            //Debug.Log(GetType() + " , "+ name+" , setup capacs !");
            for (int i = 0; i < capacities.Count; i++) capacities[i].setupCapacityEarly();
            for (int i = 0; i < capacities.Count; i++) capacities[i].setupCapacity();
        }

        virtual public void brainRestart()
        {
            for (int i = 0; i < capacities.Count; i++) capacities[i].restartCapacity();
        }

        virtual public void brainUpdate()
        {
            for (int i = 0; i < capacities.Count; i++)
            {
                //if (!capacities[i].enabled) continue;
                capacities[i].updateCapacity();
            }
        }

        virtual public void brainClean()
        {
            for (int i = 0; i < capacities.Count; i++) capacities[i].clean();
        }

    }

    public interface iBrainCandidate
    {
        BrainerLogics getBrain();
    }
}
