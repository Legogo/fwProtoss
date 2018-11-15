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
    if (txt == null) txt = GetComponentInChildren<Text>();
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

    if(overrideText != null)
    {
      setupText(overrideText);
    }

    transform.Rotate(Vector3.forward, Random.Range(-spreadAngle, spreadAngle));

    play();
  }
  
  public void setupText(string singleText)
  {
    setupText(new string[] { singleText });
  }
  public void setupText(string[] overrideText = null)
  {
    if (txt == null) return;

    if (overrideText != null && overrideText.Length > 0)
    {
      possibleText = overrideText;
    }

    if (possibleText != null && possibleText.Length > 0)
    {
      txt.text = possibleText[Random.Range(0, possibleText.Length)];
    }
    else
    {
      txt.text = "";
    }
    
  }

  public string getCurrentText()
  {
    if (txt == null) {
      Debug.LogWarning("asking for text but UI Text doesn't exists");
      return "";
    }
    
    return txt.text;
  }
}
