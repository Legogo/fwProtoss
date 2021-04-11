using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>
namespace builder
{
  [CreateAssetMenu(menuName = "builder/new profil windows", order = 100)]
  public class DataBuildSettingProfileWindows : DataBuildSettingProfile
  {
    public override string getExtension() => "exe";
    public override BuildTarget getPlatformTarget() => BuildTarget.StandaloneWindows;

    public override string getBasePath()
    {
      string basePath = base.getBasePath();

      basePath = Path.Combine(basePath, getBuildNameVersion());

      return basePath;
    }

  }

}
