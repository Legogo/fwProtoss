using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager
{

  static public void generate(DataSounds data, AudioMixerGroup mixer = null)
  {
    GameObject carry = HalperGameObject.getGameObject("[sounds]");
    for (int i = 0; i < data.clips.Length; i++)
    {
      AudioClip clip = data.clips[i];

      GameObject local = new GameObject(clip.name);
      AudioSource src = local.AddComponent<AudioSource>();

      local.transform.SetParent(carry.transform);

      src.clip = clip;

      if(mixer != null) src.outputAudioMixerGroup = mixer;
    }
    
    if (sources == null) sources = GameObject.FindObjectsOfType<AudioSource>();

    Debug.Log("SoundManager created, " + sources.Length + " sources ref");
  }

  static protected AudioSource[] sources;
  
  static protected AudioSource getMatchingSource(string containsClipName)
  {
    if (sources == null) sources = GameObject.FindObjectsOfType<AudioSource>();

    Debug.Log("searching for clip " + containsClipName + " in " + sources.Length + " sources");

    for (int i = 0; i < sources.Length; i++)
    {
      AudioSource src = sources[i];

      if (src.clip == null) Debug.LogError("<color=red>AUDIOSOURCE HAS NO CLIP</color> need cleaning in resource sound scene");

      //on rejoue pas le même son si il est déjà en train de jouer
      if (src.clip.name.Contains(containsClipName)) return src;
    }

    return null;
  }

  /// <summary>
  /// search for matching source with clip and call it as "playOneShot", no delay
  /// </summary>
  /// <param name="sndName"></param>
  /// <returns></returns>
  static public AudioSource playOneShot(string sndName)
  {
    AudioSource src = getMatchingSource(sndName);
    if (src == null) return null;

    src.PlayOneShot(src.clip);

    return src;
  }

  /// <summary>
  /// normal play on source, 0 delay
  /// </summary>
  /// <param name="sndName"></param>
  /// <returns></returns>
  static public AudioSource play(string sndName)
  {
    AudioSource src = getMatchingSource(sndName);
    if (src == null) return null;

    src.Play();

    return src;
  }

  /// <summary>
  /// normal play on source with delay offset
  /// </summary>
  /// <param name="clipName"></param>
  /// <param name="offset"></param>
  /// <returns></returns>
  static public AudioSource playOffset(string clipName, float offset)
  {
    AudioSource src = getMatchingSource(clipName);
    if (src == null) return null;

    if (src.isPlaying) return src;
    
    src.PlayDelayed(offset);
    
    return src;
  }

  static public bool isPlaying(string name)
  {
    if (sources == null) sources = GameObject.FindObjectsOfType<AudioSource>();
    for (int i = 0; i < sources.Length; i++)
    {
      if (sources[i].clip.name.Contains(name))
      {
        return sources[i].isPlaying;
      }
    }
    return false;
  }
  static public void stop(string name)
  {
    if (sources == null) sources = GameObject.FindObjectsOfType<AudioSource>();
    for (int i = 0; i < sources.Length; i++)
    {
      if (sources[i].clip.name.Contains(name))
      {
        sources[i].Stop();
      }
    }
  }

  static public void stopAllSources()
  {
    if (sources == null) sources = GameObject.FindObjectsOfType<AudioSource>();
    for (int i = 0; i < sources.Length; i++)
    {
      sources[i].Stop();
    }
  }
}
