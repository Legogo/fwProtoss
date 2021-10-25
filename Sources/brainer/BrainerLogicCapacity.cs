using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace brainer
{
    /// <summary>
    /// Some specific capacity that an LogicItem can have
    /// It's updated by it's owner
    /// capacities are mono to get param in inspector
    /// </summary>
    abstract public class BrainerLogicCapacity : MonoBehaviour
    {
        protected BrainerLogics brain; // to fetch other capac

        List<MonoBehaviour> lockers = new List<MonoBehaviour>();

        /// <summary>
        /// this kappa is already sub to brain
        /// that's how brain is calling this function
        /// </summary>
        public void assign(BrainerLogics brain)
        {
            this.brain = brain;
            //this.brain.subCapacity(this);
        }

        virtual public void setupCapacityEarly() { }
        virtual public void setupCapacity() { }

        /// <summary>
        /// reboot all params
        /// </summary>
        virtual public void restartCapacity() { }

        virtual public void updateCapacity() { }

        public bool isLocked() => lockers.Count > 0;
        public void lockCapacity(MonoBehaviour locker)
        {
            if (lockers.IndexOf(locker) < 0)
            {
                lockers.Add(locker);
            }
        }
        public void unlockCapacity(MonoBehaviour locker)
        {
            if (lockers.IndexOf(locker) > -1)
            {
                lockers.Remove(locker);
            }
        }

        virtual public void clean()
        { }

        private void OnDestroy()
        { }

        public bool hasSameBrain(BrainerLogics other) => brain == other;
        public BrainerLogics getBrain() => brain;

    }

}
