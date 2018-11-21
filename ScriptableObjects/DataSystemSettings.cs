using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "protoss/create DataSystemSettings", order = 100)]
public class DataSystemSettings : ScriptableObject {

  public bool mobile = false;
  public bool fullscreen = false;

  public Vector2 defaultResolution = new Vector2(1920f, 1080f);


}
