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
        Bounds bnd;

        private void Awake()
        {
            bnd = solveBounds();
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
