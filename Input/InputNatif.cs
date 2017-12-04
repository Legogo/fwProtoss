using UnityEngine;
using System.Collections.Generic;

/*
  2016-09-12
  Script pour afficher des infos utilisant l'API input de unity de base
*/

public class InputNatif : MonoBehaviour {

  List<Touch> history = new List<Touch>();

  void OnGUI() {
    string ct = "[NATIF INPUT SCENE]";
    ct += "\n touchCount ? " + Input.touchCount;
    ct += "\n touches[] length ? " + Input.touches.Length;

    for (int i = 0; i < Input.touches.Length; i++)
    {
      history.Add(Input.touches[i]);
    }

    int start = history.Count - 10;
    start = Mathf.Max(start, 0);
    int end = history.Count;

    for (int i = start; i < end; i++)
    {
      ct += "\n " + i + " "+history[i].fingerId+" "+ history[i].phase.ToString() + " " + history[i].position;
    }

    GUI.Label(new Rect(10,10,300,300), ct);
  }
}
