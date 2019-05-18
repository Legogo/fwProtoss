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
    
  }

  protected bool isInFolder(string path, string folder)
  {
    return (path.ToLower().IndexOf("/" + folder + "/") == -1) ;
  }

  void solveMoulinexAsset()
  {
    TextureImporter ti = (TextureImporter)assetImporter;

    Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D));

    if (asset != null)
    {
      EditorUtility.SetDirty(asset);
      return;
    }

    solveStandardMapSprite();

    Debug.Log("<b>[import]</b> | texture | path : " + assetPath);
  }

  void solveStandardMapSprite()
  {
    TextureImporter ti = (TextureImporter)assetImporter;

    ti.textureCompression = 0;
    //ti.spritePixelsPerUnit = LabyConst.spritePixelPerUnit;
    
    //ti.filterMode = FilterMode.Point;
    ti.textureType = TextureImporterType.Sprite;
    ti.spriteImportMode = SpriteImportMode.Single;
    //ti.maxTextureSize = 4096;

    //ti.spritePivot = new Vector2(asset.texture.width * -0.5f, asset.texture.height * 0.5f);
    

    TextureImporterSettings texSettings = new TextureImporterSettings();
    ti.ReadTextureSettings(texSettings);
    texSettings.spriteAlignment = (int)SpriteAlignment.TopLeft;
    ti.SetTextureSettings(texSettings);

    //ti.spritePivot = new Vector2(0f, 1f);
  }
}