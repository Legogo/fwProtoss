using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiTextSlide : UiAnimation
{
  static public UiTextSlide create(string typeResource)
  {
    return ResourceManager.getDuplicate<UiTextSlide>(typeResource);
    //GameObject obj = GameObject.Instantiate(Resources.Load(typeResource)) as GameObject;
    //return obj.GetComponent<UiTextSlide>();
  }

  protected Vector3 origin;
  protected Vector3 destination;
  protected Vector3 dir;
  protected Text txt;

  [Header("slide data")]
  public float spreadAngle = 0f;
  public float distancePx = 100f;

  [Header("display")]
  public string[] possibleText;

  protected override void build()
  {
    base.build();
    txt = GetComponent<Text>();
  }
  
  protected override void animStart()
  {
    base.animStart();
    origin = rec.position;
    destination = origin + (transform.up * distancePx);
  }

  protected override void animUpdate()
  {
    base.animUpdate();
    
    rec.position = Vector3.Lerp(origin, destination, getProgress());
  }

  protected override void animEnd()
  {
    base.animEnd();
    GameObject.DestroyImmediate(gameObject);
  }

  public void callSlide(Vector3 position, string[] overrideText = null)
  {
    rec.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
    rec.position = Camera.main.WorldToScreenPoint(position);

    if(overrideText != null && overrideText.Length > 0)
    {
      possibleText = overrideText;
    }

    txt.text = possibleText[Random.Range(0, possibleText.Length)];
    
    //Debug.Log(origin.position);

    transform.Rotate(Vector3.forward, Random.Range(-spreadAngle, spreadAngle));

    play();
  }
  
}
