using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// to define spawns for gameplay entities
/// </summary>

namespace fwp.arena
{
    public interface iArenaSpawnable
    {
    }

    public class ArenaSpawn : MonoBehaviour
    {
        private void Awake()
        {

            // remove guides
            while(transform.childCount > 0)
            {
                GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
            }

        }

        public void spawnOn(iArenaSpawnable spawnable)
        {
            //_spawnable = spawnable;

            MonoBehaviour mono = spawnable as MonoBehaviour;
            Debug.Assert(mono != null);

            Vector3 pos = transform.position;
            //pos.z = 0f;

            mono.transform.position = pos;
        }
    }

}