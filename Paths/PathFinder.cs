using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class PathFinder : MonoBehaviour {

  public List<PathFinderNode> nodes = new List<PathFinderNode>();
  
  [ContextMenu("gather nodes")]
  protected void gather_nodes() {
    nodes.Clear();

    GameObject[] all = GameObject.FindGameObjectsWithTag("pf_node");
    for (int i = 0; i < all.Length; i++)
    {
      addNode(all[i].transform);
    }

    List<PathFinderNode> links = new List<PathFinderNode>();

    for (int i = 0; i < nodes.Count; i++)
    {
      links.Clear();
      for (int j = 0; j < nodes.Count; j++)
      {
        if (i == j) continue; // skip itself
        if (checkNodeLink(nodes[i], nodes[j])) links.Add(nodes[j]); 
      }

      nodes[i].links = links.ToArray();
    }
  }

  protected PathFinderNode addNode(Transform nodeTr) {
    for (int i = 0; i < nodes.Count; i++)
    {
      if (nodes[i].tr == nodeTr) return nodes[i];
    }
    PathFinderNode node = new PathFinderNode();
    node.tr = nodeTr;
    nodes.Add(node);
    return node;
  }

  bool checkNodeLink(PathFinderNode nodeA, PathFinderNode nodeB) {
    if(nodeA == nodeB) {
      Debug.LogError("same ?");
      return false;
    }

    Vector3 dir = nodeB.tr.position - nodeA.tr.position;
    RaycastHit[] hits = Physics.RaycastAll(nodeA.tr.position, dir.normalized, dir.magnitude);
    
    if(hits.Length <= 0) {
      Debug.DrawLine(nodeA.tr.position, nodeB.tr.position, Color.green, 1f);
      return true;
    }

    for (int i = 0; i < hits.Length; i++)
    {
      Debug.DrawLine(nodeA.tr.position, hits[i].point, Color.red, 1f);
    }
    
    return false;
  }

#if UNITY_EDITOR
  void OnDrawGizmos() {
    for (int i = 0; i < nodes.Count; i++)
    {
      Gizmos.DrawCube(nodes[i].tr.position, Vector3.one * 0.1f);
      string ct = "";
      for (int j = 0; j < nodes[i].links.Length; j++)
      {
        Gizmos.DrawLine(nodes[i].tr.position, nodes[i].links[j].tr.position);
        ct += "\n  - " + nodes[i].links[j].tr.name;
      }

      Handles.Label(nodes[i].tr.position + Vector3.up * 0.2f, ">"+nodes[i].tr.name);
      Handles.Label(nodes[i].tr.position, ct);
    }
  }
#endif

}

[Serializable]
public class PathFinderNode {
  public Transform tr;
  public PathFinderNode[] links;
}
