using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp
{

  public class FollowPath : EngineObject
  {
    public Path path;

    protected int idx = 0;

    public float speed = 1f;
    public Transform target;

    public override void updateEngine()
    {
      base.updateEngine();

      int next = path.getNextIndex(idx);

      //Vector3 origin = path.nodes[idx];
      Vector3 destination = path.nodes[next];

      target.position = Vector3.MoveTowards(target.position, destination, Time.deltaTime * speed);
      
    }
  }

}
