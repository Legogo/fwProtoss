using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// this class is meant to have a quick looping ground
/// 
/// a object 'ground' must be available in resource
/// x=0 of object must be aligned to left of symbol (sprite : pivot = left)
/// 
/// </summary>

public class GroundLooping : ArenaObject {
  
  SpriteRenderer[] renders;
  
  Vector3 gspacePosition;
  GameSpace gspace;

  SpriteRenderer original;

  protected override void setup()
  {
    base.setup();

    gspace = GameSpace.get();

    //origin sprite center NEED to be in middle
    original = ResourceManager.getDuplicate<SpriteRenderer>("ground");

    //deduce how many needed
    float w = original.bounds.extents.x * 2f;
    int count = Mathf.FloorToInt(gspace.getWidth() / w);

    //generate ground from resources
    List<SpriteRenderer> list = new List<SpriteRenderer>();
    list.Add(original);
    for (int i = 0; i < count+1; i++)
    {
      list.Add(ResourceManager.getDuplicate<SpriteRenderer>("ground"));
    }
    renders = list.ToArray();

    //Vector3 localOffset = new Vector3(-w * 0.5f, 0f, 0f);

    //set all child of current transform and init position
    for (int i = 0; i < renders.Length; i++)
    {
      //renders[i].gameObject.tag = "ground";
      renders[i].transform.SetParent(transform);
      renders[i].transform.localPosition = Vector3.zero;
      //renders[i].transform.localPosition = localOffset + new Vector3(i * w, 0f, 0f);
      //renders[i].transform.position = Vector3
    }
    
  }
  
  protected override void updateArenaLive(float timeStamp)
  {
    base.updateArenaLive(timeStamp);
    updateGroundByGameSpace();
  }
  
  public void updateGroundByGameSpace()
  {
    float left = gspace.screenSpace.xMin;
    float w = original.bounds.extents.x * 2f;
    int qty = Mathf.FloorToInt(left / w);
    float diff = left - (qty * w);
    float result = left - diff;
    //Debug.Log(left + " - " + diff+" ("+w+") = "+result);
    updatePositionsFromOriginal(result);
  }
  
  protected void updatePositionsFromOriginal(float originalPosition)
  {
    Vector3 pos = original.transform.position;
    pos.x = originalPosition;
    original.transform.position = pos;

    for (int i = 1; i < renders.Length; i++)
    {
      pos = renders[i-1].transform.position;
      pos.x += (original.bounds.extents.x * 2f);
      renders[i].transform.position = pos;
    }
  }

  public void updateGroundByTranslation(float speed)
  {
    speed *= Time.deltaTime;

    for (int i = 0; i < renders.Length; i++)
    {
      Vector3 pos = renders[i].transform.position;
      pos.x -= speed; // going left

      //loop
      if (pos.x < renders[i].bounds.extents.x)
      {
        pos.x = gspace.getWidth() + renders[i].bounds.extents.x;
      }
    }
  }

}
