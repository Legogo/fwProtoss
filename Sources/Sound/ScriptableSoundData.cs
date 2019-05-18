using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "fwp/create ScriptableSoundData", order = 100)]
public class ScriptableSoundData : ScriptableObject {

  public AudioClip[] common;

  public AudioClip[] menu_feedback;
  public AudioClip[] ingame_feedback;

  public AudioClip[] musics;
  
  public List<AudioClip> getCombinedLists()
  {
    List<AudioClip> clips = new List<AudioClip>();

    clips.AddRange(common);
    clips.AddRange(menu_feedback);
    clips.AddRange(ingame_feedback);
    clips.AddRange(musics);

    return clips;
  }
}
