using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLooping : ArenaObject {

  public Sprite ground;
  public float translationSpeed;

  SpriteRenderer[] renders;

  protected override void setup()
  {
    base.setup();

    List<SpriteRenderer> list = new List<SpriteRenderer>();
    for (int i = 0; i < 3; i++)
    {
      list.Add(ResourceManager.getDuplicate<SpriteRenderer>("ground"));
    }

    renders = list.ToArray();

    for (int i = 0; i < renders.Length; i++)
    {
      //renders[i].gameObject.tag = "ground";
      renders[i].transform.SetParent(transform);
      //renders[i].transform.position = Vector3
    }

    renders[0].transform.localPosition = Vector3.left * renders[0].bounds.extents.x;
    renders[1].transform.localPosition = Vector3.zero;
    renders[2].transform.localPosition = Vector3.right * renders[0].bounds.extents.x;
  }

  protected override void updateArenaLive(float timeStamp)
  {
    base.updateArenaLive(timeStamp);
    
    for (int i = 0; i < renders.Length; i++)
    {
      Vector3 pos = renders[i].transform.position;
      pos.x += -translationSpeed * Time.deltaTime;
      if(pos.x < -5f - renders[i].bounds.extents.x)
      {
        pos.x = 5f + renders[i].bounds.extents.x;
      }
    }

  }

}
