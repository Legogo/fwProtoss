using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine.SceneManagement;

static public class HalperScene {

  /// <summary>
  /// active scene
  /// </summary>
  /// <param name="partOfSceneName"></param>
  /// <returns></returns>
  static public bool isActiveScene(string partOfSceneName)
  {
    Scene sc = SceneManager.GetActiveScene();
    if (sc.name.Contains(partOfSceneName)) return true;
    return false;
  }


#if UNITY_EDITOR

  /// <summary>
  /// Récupère un tableau des scènes/chemin d'accès qui sont présente dans les paramètres du build
  /// </summary>
  /// <param name="removePath">Juste le nom (myScene) ou tout le chemin d'accès (Assets/folder/myScene.unity) ?</param>
  /// <returns>Le tableau avec le nom ou chemin d'accès aux scènes.</returns>
  static public string[] getAllBuildScenes(bool includeSceneOnly, bool removePath)
  {
    string[] scenes = new string[] { };

    if (includeSceneOnly)
    {
      scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
    }
    else
    {
      EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

      scenes = new string[buildScenes.Length];

      for (int i = 0; i < scenes.Length; i++)
      {
        scenes[i] = buildScenes[i].path;
      }
    }

    if (removePath)
    {
      for (int i = 0; i < scenes.Length; i++)
      {
        int slashIndex = scenes[i].LastIndexOf('/');

        if (slashIndex >= 0)
        {
          scenes[i] = scenes[i].Substring(slashIndex + 1);
        }

        scenes[i] = scenes[i].Remove(scenes[i].LastIndexOf(".unity"));
      }

      return scenes;
    }
    else return scenes;

  }// getAllBuildScenesNames()

#endif

}
