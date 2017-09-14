using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnStartup : MonoBehaviour {
  void Awake() {
    DestroyImmediate(gameObject);
  }
}
