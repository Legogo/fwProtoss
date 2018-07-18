using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{

  static protected AudioSource[] sources;
  
  static public AudioSource call(string name, float offset = 0f, bool fx = false)
  {
    //Debug.Log("<color=yellow>~SoundManager~</color> call(<b>" + name + "</b>)");
    if (sources == null) sources = GameObject.FindObjectsOfType<AudioSource>();

    for (int i = 0; i < sources.Length; i++)
    {
      AudioSource src = sources[i];

      if (src.clip == null) Debug.LogError("<color=red>AUDIOSOURCE HAS NO CLIP</color> need cleaning in resource sound scene");

      //on rejoue pas le même son si il est déjà en train de jouer
      if (src.clip.name.Contains(name))
      {

        //Debug.Log("asking for " + name + " " + src.isPlaying);

        if(fx)
        {
          sources[i].PlayOneShot(sources[i].clip);
        }
        else
        {

          if (!sources[i].isPlaying)
          {

            //Debug.Log("playing " + name);

            sources[i].PlayDelayed(offset);

            return sources[i];
          }

        }

      }


    }

    return null;
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
