using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// https://docs.unity3d.com/ScriptReference/AssetPostprocessor.html
/// https://docs.unity3d.com/ScriptReference/AssetPostprocessor.OnPostprocessTexture.html
/// https://answers.unity.com/questions/14703/how-to-start-coding-with-assetpostprocessor.html
/// https://answers.unity.com/questions/55118/changing-texture-import-default-settings.html
/// </summary>

// Disable import of materials if the file contains
// the @ sign marking it as an animation.
public class PostImport : AssetPostprocessor
{
  void OnPreprocessModel()
  {
    //Debug.Log(assetPath);

    if (assetPath.Contains("@"))
    {
      //ModelImporter modelImporter = assetImporter as ModelImporter;
      //modelImporter.importMaterials = false;
    }
  }

  //void OnPostprocessTexture(Texture2D texture)
  void OnPreprocessTexture()
  {
    TextureImporter ti = (TextureImporter)assetImporter;

    //Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D));

    //this filters went it's an editor modification and not first import
    Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D));
    if (asset != null)
    {
      EditorUtility.SetDirty(asset);
      return;
    }

    Debug.Log("<b>[import]</b> | texture");
    Debug.Log("  L path : " + assetPath);
    //Debug.Log("  L asset : " + asset.name);

    if (assetPath.ToLower().Contains("icon"))
    {
      Debug.Log("  L solving icon !");

      ti.textureCompression = 0;
      ti.filterMode = FilterMode.Point;
      ti.textureType = TextureImporterType.Sprite;
      ti.spriteImportMode = SpriteImportMode.Single;
      //ti.spritePivot = (Vector2.right * 0.5f);
    }

    //assetImporter.SaveAndReimport();
  }

  protected bool isInFolder(string path, string folder)
  {
    return (path.ToLower().IndexOf("/" + folder + "/") == -1) ;
  }
}