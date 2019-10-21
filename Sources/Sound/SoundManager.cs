using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace fwp.sound
{
  public class SoundManager
  {

    static public void generate(MonoBehaviour coroutineCarry, string soundSceneName = "sounds")
    {
      coroutineCarry.StartCoroutine(processLoading(soundSceneName));
    }

    static protected IEnumerator processLoading(string soundSceneName)
    {
      Debug.Log("process generating sounds for manager");

      Debug.Log("  L loading 'sounds' scene ... ");
      AsyncOperation async = SceneManager.LoadSceneAsync(soundSceneName, LoadSceneMode.Additive);
      while (!async.isDone)
      {
        yield return null;
        //Debug.Log(async.progress);
      }

      Debug.Log("  L ... " + soundSceneName + " scene loaded");

      AudioSource[] srcs = GameObject.FindObjectsOfType<AudioSource>();
      sources = new List<AudioSource>();
      sources.AddRange(srcs);

      Debug.Log("  L ... found <b>" + sources.Count + " AudioSource</b> in scene " + soundSceneName);

      /*
      List<AudioClip> clips = new List<AudioClip>();
      for (int i = 0; i < srcs.Length; i++)
      {
        if (srcs[i].clip != null && clips.IndexOf(srcs[i].clip) < 0) clips.Add(srcs[i].clip);
      }

      Debug.Log("  L ... found " + clips.Count + " AudioClip");
      */

      /*
      for (int i = 0; i < clips.Count; i++)
      {
        AudioClip clip = clips[i];
        GameObject local = new GameObject(clip.name);
        AudioSource src = local.AddComponent<AudioSource>();
        local.transform.SetParent(carry.transform);
        src.clip = clip;
        if(mixer != null) src.outputAudioMixerGroup = mixer;
      }
      */

      //if (sources == null) sources = GameObject.FindObjectsOfType<AudioSource>();

      //Debug.Log("SoundManager created, " + sources.Count + " sources ref");
    }

    static protected List<AudioSource> sources;

    static protected AudioSource getMatchingSource(string containsClipName)
    {
      if (sources == null) return null;

      //Debug.Log("searching for clip " + containsClipName + " in " + sources.Length + " sources");

      for (int i = 0; i < sources.Count; i++)
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

    static public bool isPlaying(string clipName)
    {
      AudioSource src = getMatchingSource(clipName);
      if (src == null) return false;
      return src.isPlaying;
    }
    static public void stop(string clipName)
    {
      AudioSource src = getMatchingSource(clipName);
      if (src == null) return;
      src.Stop();
    }

    static public void stopAllSources()
    {
      for (int i = 0; i < sources.Count; i++)
      {
        sources[i].Stop();
      }
    }
  }
}
