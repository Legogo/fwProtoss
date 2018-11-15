using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp
{
  public class Path : MonoBehaviour
  {
    public List<Vector3> nodes;
    
    public int getNextIndex(int current)
    {
      int nextIdx = current + 1;
      if (nextIdx >= nodes.Count) nextIdx = 0;
      return nextIdx;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

      if (nodes != null)
      {
        for (int i = 0; i < nodes.Count; i++)
        {
          int nextIdx = getNextIndex(i);
          Gizmos.DrawLine(nodes[i], nodes[nextIdx]);
        }
      }

    }
#endif

  }

}
