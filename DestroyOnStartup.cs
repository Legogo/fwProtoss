using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnStartup : MonoBehaviour {
  
  IEnumerator Start() {
    yield return new WaitForSeconds(0.5f);

    DestroyImmediate(gameObject);
  }

}
