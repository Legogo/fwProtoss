using UnityEngine;
using System;
using System.Diagnostics;

/// <summary>
/// tools platform specific related
/// </summary>

static public class HalperNatives
{

    static public string generateUniqId()
    {
        return Guid.NewGuid().ToString();
    }

    /// <summary>
    /// open explorer at path
    /// </summary>
    /// <param name="folderPath"></param>
    static public void os_openFolder(string folderPath)
    {
        folderPath = folderPath.Replace(@"/", @"\");   // explorer doesn't like front slashes

        //https://stackoverflow.com/questions/334630/opening-a-folder-in-explorer-and-selecting-a-file
        string argument = "/select, \"" + folderPath + "\"";

        //https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.start?view=netframework-4.7.2#System_Diagnostics_Process_Start_System_String_System_String_
        System.Diagnostics.Process.Start("explorer.exe", argument);
    }


    //https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html
    /// <summary>
    /// Windows Store Apps: Application.persistentDataPath points to %userprofile%\AppData\Local\Packages\<productname>\LocalState.
    /// iOS: Application.persistentDataPath points to /var/mobile/Containers/Data/Application/<guid>/Documents.
    /// Android: Application.persistentDataPath points to /storage/emulated/0/Android/data/<packagename>/files on most devices
    /// </summary>
    /// <returns></returns>
    static public string getDataPath()
    {
        return Application.persistentDataPath;
    }

    static public bool isWindows()
    {
        return Application.platform == RuntimePlatform.WindowsPlayer
          || Application.platform == RuntimePlatform.WindowsEditor;
    }

    static public bool isOsx()
    {
        return Application.platform == RuntimePlatform.OSXPlayer
          || Application.platform == RuntimePlatform.OSXEditor;
    }

    static public bool isAndroid() { return Application.platform == RuntimePlatform.Android; }
    static public bool isIos() { return Application.platform == RuntimePlatform.IPhonePlayer; }
    static public bool isSwitch() { return Application.platform == RuntimePlatform.Switch; }
    //static public bool isIpad() { return Application.platform == RuntimePlatform.; }

    static public bool isTouch()
    {
        return Input.touchSupported;
    }

    static public bool isMobile()
    {
        if (isIos()) return true;
        if (isAndroid()) return true;
        //if (isTouch()) return true;
        return false;
    }



    /// <summary>
    /// meant to call cmd on windows
    /// </summary>
    /// <param name="fullPath"></param>
    /// <param name="args"></param>
    static public void startCmd(string fullPath, string args = "")
    {
        ProcessStartInfo startInfo = new ProcessStartInfo(fullPath);
        startInfo.WindowStyle = ProcessWindowStyle.Normal;
        if (args.Length > 0) startInfo.Arguments = args;

        //Debug.Log(Environment.CurrentDirectory);

        Process.Start(startInfo);

    }

}
