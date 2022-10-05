using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// to define spawns for gameplay entities
/// </summary>

namespace fwp.arena
{
    public class ArenaSpawn : MonoBehaviour
    {
        public Vector3 getSpawnPosition() { return transform.position; }
        public Transform getSpawn() { return transform; }
    }
}