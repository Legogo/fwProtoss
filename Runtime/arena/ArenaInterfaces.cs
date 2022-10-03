using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  for an object to receive collision events with arena obstacles
*/

namespace fwp.engine.arena
{
    public interface iArenaGameplayEntity
    {
        // quand l'objet se fait toucher par un truc de l'arène
        void hit(ArenaObject obstacle);

        // récup les box de collisions
        BoxCollider2D[] getColliders();

    }
}