using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// entry point is callSlide()
/// </summary>

public class UiTextSlide : UiAnimation
{
  protected Vector3 origin;
  protected Vector3 destination;
  protected Vector3 dir;
  public Text txt;

  [Header("slide data")]
  public float spreadAngle = 0f;
  public float distancePx = 100f;

  [Header("display")]
  public string[] possibleText;

  protected override void build()
  {
    base.build();
    if(txt == null)
    {
      txt = GetComponent<Text>();
      if (txt == null) txt = GetComponentInChildren<Text>();
    }
  }
  
  protected override void animStart()
  {
    base.animStart();
    origin = rec.position;
    destination = origin + (transform.up * distancePx);
  }

  protected override void updateAnimationProcess()
  {
    rec.position = Vector3.Lerp(origin, destination, getProgress());
  }

  public UiTextSlide callSlide(Vector3 worldPosition, string[] overrideText = null)
  {
    //rec.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
    Camera cam = Camera.main;
    worldPosition.z = -cam.transform.position.z;
    rec.position = cam.WorldToScreenPoint(worldPosition);
    //rec.position = worldPosition;

    //Debug.Log(rec.transform.parent, rec.transform.parent);
    //Debug.Log(Camera.main, Camera.main.transform);
    //Debug.Log(worldPosition);
    //Debug.Log(rec.position);

    if(overrideText != null)
    {
      setupText(overrideText);
    }

    if(spreadAngle != 0f)
    {
      transform.Rotate(Vector3.forward, Random.Range(-spreadAngle, spreadAngle));
    }
    
    play();

    return this;
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

  public void swapColor(Color newTextColor)
  {
    txt.color = newTextColor;
  }

  public string getCurrentText()
  {
    if (txt == null) {
      Debug.LogWarning("asking for text but UI Text doesn't exists");
      return "";
    }
    
    return txt.text;
  }

  /// <summary>
  /// type is resource name
  /// </summary>
  /// <param name="typeResource"></param>
  /// <returns></returns>
  static public UiTextSlide create(string typeResource)
  {
    return ResourceManager.getDuplicate<UiTextSlide>(typeResource);
    //GameObject obj = GameObject.Instantiate(Resources.Load(typeResource)) as GameObject;
    //return obj.GetComponent<UiTextSlide>();
  }

}
