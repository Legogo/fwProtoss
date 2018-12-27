using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnStartup : MonoBehaviour {

  public float timerToWait = 0f;

  IEnumerator Start() {
    
    if(timerToWait > 0f) {
      yield return new WaitForSeconds(timerToWait);
    }

    GameObject.Destroy(gameObject);
  }

}
