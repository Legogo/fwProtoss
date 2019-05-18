using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscapeGenerator : ArenaObject {

  protected GameSpace gspace;

  public ScriptableSpriteAtlas atlas;

  //public struct LandscapeData { public float pos, public  }
  List<SpriteRenderer> landscapeObjects = new List<SpriteRenderer>();

  protected Vector2 currentGenerationPosition;
  protected Vector2 lastGenerationPosition;

  public long seed = 0;

  float rndLimit = 0.8f;

  protected override void setup()
  {
    base.setup();
    gspace = GameSpace.get();
  }

  public override void arena_round_restart()
  {
    base.arena_round_restart();
    
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
    float border = minInterval * 10f;
    float startx = (qty * minInterval);
    float w = gspace.getWidth();

    int idx = 0;

    //Debug.Log("start : "+startx + " , end : " + (startx + w) + " , interval : " + minInterval);

    //Debug.Log("[]");
    Debug.DrawLine(Vector3.right * startx, Vector3.right * (startx + w), Color.green);

    for (float i = startx - border; i < startx + (w + border); i+=minInterval)
    {
      float val = rand(Mathf.FloorToInt(i * 1000));
      
      if (val > rndLimit)
      {
        //Debug.Log(i + " == " + val);
        //Debug.Log(i+" -> rnd : "+val+" (limit : "+rndLimit+")");

        Debug.DrawLine((Vector3.down * i) + (Vector3.right * startx), (Vector3.down * i) + Vector3.right * (startx + i), Color.green);

        Sprite spr = getSpriteAtPosition(i);
        while (landscapeObjects.Count < idx+1) spawn(spr);

        landscapeObjects[idx].sprite = spr;
        //Debug.Log(idx + " / " + landscapeObjects.Count);
        Vector3 pos = landscapeObjects[idx].transform.position;
        pos.x = i;
        landscapeObjects[idx].transform.position = pos;

        //Debug.Log(i+" <=> "+idx + " at " + pos);
        idx++;
      }
    }

  }

  protected Sprite getSpriteAtPosition(float pos)
  {
    if (atlas.list.Length <= 0) return null;
    float rnd = rand(Mathf.FloorToInt(pos * 1000));
    rnd = Mathf.InverseLerp(rndLimit, 1f, rnd);
    int idx = Mathf.FloorToInt(rnd * (atlas.list.Length - 1));
    if (idx >= atlas.list.Length) Debug.LogError(idx + " > " + atlas.list.Length);
    return atlas.list[idx];
  }

  protected SpriteRenderer spawn(Sprite spr)
  {
    GameObject obj = ResourceManager.getDuplicate(spr.name);
    if (obj == null) Debug.Log("no object for " + spr.name);

    obj.transform.position = transform.position;

    SpriteRenderer render = obj.GetComponent<SpriteRenderer>();
    landscapeObjects.Add(render);

    return render;
  }

}
