using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.arena
{
    using fwp.engine.mod;

    /// <summary>
    /// manager meant to provide tools regarding arena context where the action is
    /// </summary>
    abstract public class ArenaManager : MonoBehaviour, iModObject
    {
        ArenaObject[] candidates;

        ArenaSpawn[] spawns;
        Bounds bnd;

        ModBase mod;

        private void Awake()
        {
            bnd = solveBounds();

            build();
        }

        virtual protected void build()
        { }

        void Start()
        {
            mod = ModBase.getMod();
            Debug.Assert(mod != null);

            mod.candidates.Add(this);
        }

        void OnDestroy()
        {
            mod.candidates.Remove(this);
        }

        public void fetchSpawns()
        {
            spawns = GameObject.FindObjectsOfType<ArenaSpawn>();
        }

        public Transform getSpawn(int playerIndex)
        {
            if (!hasSpawns()) return null;
            if (spawns.Length > playerIndex) return spawns[playerIndex].transform;
            return null;
        }

        public bool hasSpawns()
        {
            if (spawns == null) return false;
            return spawns.Length > 0;
        }

        abstract protected Bounds solveBounds();

        Vector3 getLimitLeft()
        {
            return bnd.min;
        }

        Vector3 getLimitRight()
        {
            return bnd.max;
        }

        public Vector3 getRandomPositionInArena(float gap)
        {
            Vector3 pos = Vector3.zero;
            pos.x = Random.Range(getLimitLeft().x + gap, getLimitRight().x - gap);
            return pos;
        }

        public Bounds getBounds() => bnd;

        virtual public void modRestart()
        {
            candidates = GameObject.FindObjectsOfType<ArenaObject>();
            for (int i = 0; i < candidates.Length; i++)
            {
                candidates[i].arenaRestart();
            }

            Debug.Log(GetType() + " mod restart x" + candidates.Length, this);
        }

        virtual public void modUpdate()
        {
            //Debug.Log(GetType() + " mod update x"+candidates.Length, this);

            for (int i = 0; i < candidates.Length; i++)
            {
                candidates[i].arenaUpdate();
            }
        }

        virtual public void modEnd()
        {
            for (int i = 0; i < candidates.Length; i++)
            {
                candidates[i].arenaEnd();
            }

            candidates = null;
        }
    }


}
