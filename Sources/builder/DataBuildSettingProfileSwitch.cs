﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// 
/// application id
/// https://developer.nintendo.com/html/online-docs/g1kr9vj6-en/Packages/SDK/NintendoSDK/Documents/Package/contents/Pages/Page_177636769.html#Anchor_177636769_ApplicationId
/// Application ID
/// A unique ID assigned to each application.
/// Specified using a value in the range from 0x0100 0000 0001 0000 to 0x01FFF FFFF FFFF FFFF.
/// For retail applications, the application ID is the value that was specified on the Nintendo Developer Portal.
/// For development tools and prototypes, use the value 0x0100 XXX0 0348 8000, where XXX is any value between 000 and FFF./// 
/// 
/// 
/// player settings
/// https://developer.nintendo.com/html/online-docs/g1kr9vj6-en/Packages/middleware/UnityForNintendoSwitch/Documents/contents-en/Pages/Page_165366622.html
/// 
/// NMETA
/// https://developer.nintendo.com/html/online-docs/g1kr9vj6-en/Packages/SDK/NintendoSDK/Documents/Package/contents/Pages/Page_166503043.html
/// 
/// Languages
/// https://developer.nintendo.com/group/development/g1kr9vj6/forums/english/-/gts_message_boards/thread/19553057#568124
/// need to now define "small icon" and have bmp icon file (1024x1024)
/// </summary>

namespace builder
{
  [CreateAssetMenu(menuName = "builder/new profil switch", order = 100)]
  public class DataBuildSettingProfileSwitch : DataBuildSettingProfile
  {
    [Header("switch specifics")]

    public bool build_rom = false;

    [Header("not implem")]
    public Il2CppCompilerConfiguration compilerConfig = Il2CppCompilerConfiguration.Release;
    public int maxControllerCount = 8;

    // .../builds/[project]_[date]/?[rom]/?[dev]
    public override string getBasePath()
    {
      string basePath = base.getBasePath();

      if (EditorUserBuildSettings.switchCreateRomFile)
      {
        basePath = Path.Combine(basePath, "roms");
      }

      if (EditorUserBuildSettings.development)
      {
        basePath = Path.Combine(basePath, "dev");
      }

      basePath = Path.Combine(basePath, getBuildFolderName());

      return basePath;
    }

    public override string getExtension()
    {
      if (build_rom) return "nsp";
      return "nspd";
    }

    public override BuildTarget getPlatformTarget() => BuildTarget.Switch;

    public override string getProductName()
    {
      string bn = base.getProductName();

      string tmp = "";
      string[] split = bn.Split(' ');
      for (int i = 0; i < split.Length; i++)
      {
        string sp = split[i].ToLower();

        tmp = sp[0].ToString();

        if (split[i].Length >= 3)
        {
          tmp += sp[Mathf.FloorToInt(sp.Length * 0.5f)];
        }

        tmp += sp[sp.Length - 1];

        split[i] = tmp;
      }

      bn = "";
      for (int i = 0; i < split.Length; i++)
      {
        bn += split[i].ToString().upperFirstLetter();
      }

      return bn + " (" + VersionManager.getFormatedVersion('.') + ")";
    }

    public override void apply()
    {
      base.apply();

      EditorUserBuildSettings.switchCreateRomFile = build_rom;

      //https://developer.nintendo.com/group/development/g1kr9vj6/forums/english/-/gts_message_boards/thread/291538191#968039
      //You can use 0x0100 XXX0 0348 8000, where XXX is any value between 000 and FFF for R&D purpose.
      //0x0100 1110 0348 8000

      //https://developer.nintendo.com/group/development/g1kr9vj6/forums/english/-/gts_message_boards/thread/280010505#718072
      //this is default value
      //0x01004b9000490000

      string appId = "0x0100";

      appId += getRndHexDigit();
      appId += getRndHexDigit();
      appId += getRndHexDigit();

      appId += "003488000";

      PlayerSettings.Switch.applicationID = appId;


    }

    char getRndHexDigit()
    {
      int num = Random.Range(0, 16);

      char output = ' ';

      switch (num)
      {
        case 10: output = 'a'; break;
        case 11: output = 'b'; break;
        case 12: output = 'c'; break;
        case 13: output = 'd'; break;
        case 14: output = 'e'; break;
        case 15: output = 'f'; break;
        default: output = num.ToString()[0]; break;
      }

      return output;
    }
  }

}
