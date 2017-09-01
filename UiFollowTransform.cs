using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiFollowTransform : MonoBehaviour {

  protected RectTransform rectTransform;
  public Transform target;

  void Awake()
  {
    rectTransform = GetComponent<RectTransform>();
  }
  
  public void Update()
  {
    rectTransform.position = target.position;
  }

}
