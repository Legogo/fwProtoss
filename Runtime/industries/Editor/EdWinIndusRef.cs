using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

using fwp.halpers;
using fwp.halpers.editor;

public class EdWinIndusRef : EditorWindow
{

    [MenuItem("Monitoring/(window) indus ref")]
    static void init()
    {
        EditorWindow.GetWindow(typeof(EdWinIndusRef));
    }

    /// <summary>
    /// called on window creation
    /// </summary>
    void OnEnabled()
    {
        refTypes = IndusReferenceMgr.getAllTypes();
    }

    Type[] refTypes;
    MonoBehaviour cursorMono = null;

    Vector2 _cursorPosition = Vector2.zero;
    Vector2 cursorPosition = Vector2.zero;

    Vector2 scroll;

    void Update()
    {
        _cursorPosition = HalperMouse.world_position;
        if (cursorPosition.sqrMagnitude != _cursorPosition.sqrMagnitude)
        {
            cursorPosition = _cursorPosition;

            MonoBehaviour curMono = IndusReferenceMgr.getClosestToPosition(typeof(BrainBase), cursorPosition);
            if (curMono != cursorMono)
            {
                cursorMono = curMono;
            }

            Repaint();
        }
    }

    void OnGUI()
    {
        if(!Application.isPlaying)
        {
            refTypes = null;
        }

        GUILayout.BeginHorizontal();

        GUILayout.Label(cursorPosition.ToString());

        if (cursorMono != null) GUILayout.Label(cursorMono.name.ToString());
        else GUILayout.Label("nothing close");

        GUILayout.EndHorizontal();

        if(IndusReferenceMgr.hasAnyType() && refTypes == null)
        {
            refTypes = IndusReferenceMgr.getAllTypes();
        }

        if (GUILayout.Button("uber refresh types list"))
        {
            IndusReferenceMgr.edRefresh(); // ed window
            IndusReferenceMgr.refreshAll();
            refTypes = IndusReferenceMgr.getAllTypes();
        }

        if(refTypes == null)
        {
            GUILayout.Label("no types, plz refresh");
            return;
        }
        if(refTypes.Length <= 0)
        {
            GUILayout.Label("types count 0, plz refresh");
            return;
        }

        scroll = GUILayout.BeginScrollView(scroll);

        GUILayout.Label("x" + refTypes.Length + " in facebook");
        foreach (var typ in refTypes)
        {
            EditorGUILayout.Separator();

            GUILayout.BeginHorizontal();

            List<iIndusReference> refs = IndusReferenceMgr.getGroupByType(typ);

            GUILayout.Label(typ.ToString(), HalperGuiStyle.getCategoryBold());
            GUILayout.Label("x" + refs.Count + " elmt(s)");
            
            GUILayout.EndHorizontal();
            
            foreach(var elmt in refs)
            {
                if (elmt == null)
                {
                    GUILayout.Label("null");
                    continue;
                }

                GUILayout.BeginHorizontal();

                MonoBehaviour mono = elmt as MonoBehaviour;
                if (mono != null) EditorGUILayout.ObjectField(mono.name, mono, typeof(MonoBehaviour), true);
                else GUILayout.Label(elmt.GetType().ToString());

                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndScrollView();
        //EditorGUILayout.ObjectField("Title", objectHandle, typeof(objectClassName), true);
    }
}
