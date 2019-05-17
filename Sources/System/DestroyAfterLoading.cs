using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterLoading : MonoBehaviour
{
  IEnumerator Start()
  {
    while (EngineManager.isLoading()) yield return null;

    GameObject.Destroy(gameObject);
  }
  
}
