using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.arena
{
    /// <summary>
    /// manager meant to provide tools regarding arena context where the action is
    /// </summary>
    abstract public class ArenaManager : MonoBehaviour
    {
        ArenaSpawn[] spawns;
        Bounds bnd;

        private void Awake()
        {
            bnd = solveBounds();

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
    }


}
