using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscapeGenerator : ArenaObject {

  protected GameSpace gspace;

  public DataResourceAtlas atlas;
  public List<Transform> landscapeObjects;

  protected Vector2 currentGenerationPosition;
  protected Vector2 lastGenerationPosition;

  public long seed = 0;

  protected override void setup()
  {
    base.setup();
    gspace = GameSpace.get();
  }

  public override void restart()
  {
    base.restart();
    
    //fill screen
    //...
  }
  
  float rand(int step)
  {
    //Debug.Log("seed ? "+step);
    Random.InitState(step);
    //Debug.Log(Random.value);
    return Random.value;
  }

  protected override void updateArenaLive(float timeStamp)
  {
    base.updateArenaLive(timeStamp);

    //inf loop !
    if (atlas.list.Length <= 0) return;

    float minInterval = 1f;
    float left = gspace.screenSpace.xMin;
    int qty = Mathf.FloorToInt(left / minInterval);
    float startx = qty * minInterval;
    float w = gspace.getWidth();

    int idx = 0;

    //Debug.Log("start : "+startx + " , end : " + (startx + w) + " , interval : " + minInterval);
    for (float i = startx; i < startx + w; i+=minInterval)
    {
      float val = rand(Mathf.FloorToInt(i * 1000));

      //Debug.Log(startx+" | "+i + " == " + val);

      if(val > 0.8f)
      {
        while (landscapeObjects.Count < idx+1) spawn(Mathf.InverseLerp(0.5f, 1f, val));
        //Debug.Log(idx + " / " + landscapeObjects.Count);
        Vector3 pos = landscapeObjects[idx].transform.position;
        pos.x = i;
        landscapeObjects[idx].transform.position = pos;
        idx++;
      }
    }

  }

  protected SpriteRenderer spawn(float rnd)
  {
    if (atlas.list.Length <= 0) return null;

    int idx = Mathf.FloorToInt(rnd * atlas.list.Length-1);
    if (idx >= atlas.list.Length) Debug.LogError(idx + " > " + atlas.list.Length);

    Sprite spr = atlas.list[idx];
    GameObject obj = ResourceManager.getDuplicate(spr.name);
    if (obj == null) Debug.Log("no object for " + spr.name);

    obj.transform.position = transform.position;

    landscapeObjects.Add(obj.transform);

    return obj.GetComponent<SpriteRenderer>();
  }

}
