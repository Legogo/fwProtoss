using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{

  static protected AudioSource[] sources;
  static public AudioSource call(string name, float offset = 0f)
  {
    if (sources == null) sources = GameObject.FindObjectsOfType<AudioSource>();

    for (int i = 0; i < sources.Length; i++)
    {
      AudioSource src = sources[i];

      //on rejoue pas le même son si il est déjà en train de jouer
      if (src.clip.name.Contains(name))
      {

        //Debug.Log("asking for " + name + " " + src.isPlaying);

        if (!sources[i].isPlaying)
        {

          //Debug.Log("playing " + name);

          sources[i].PlayDelayed(offset);

          return sources[i];
        }

      }


    }

    return null;
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

  static public void stop()
  {
    if (sources == null) sources = GameObject.FindObjectsOfType<AudioSource>();
    for (int i = 0; i < sources.Length; i++)
    {
      sources[i].Stop();
    }
  }
}
