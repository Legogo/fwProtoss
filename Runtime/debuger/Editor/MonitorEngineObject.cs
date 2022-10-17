using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace fwp.engine
{
    using fwp.engine.scaffolder;

    public class EngineObjectMonitoring : EditorWindow
    {

        [MenuItem("Monitoring/object inspector")]
        static void init()
        {
            EditorWindow.GetWindow(typeof(EngineObjectMonitoring));
        }

        string[] options;
        int dropdownIndex = 0;
        
        iScaffLog[] candidateList;
        GameObject currentSelection;
        GUIStyle style = new GUIStyle();

        void OnGUI()
        {
            GUILayout.Label("~Protoss framework~ objects");

            if (!Application.isPlaying)
            {
                return;
            }

            //display droplist & select index
            guiSelection();

            if(currentSelection == null)
            {
                GUILayout.Label("no selection");
                return;
            }

            guiDrawSelection();
        }

        void guiDrawSelection()
        {
            if (candidateList == null) return;
            if (candidateList.Length <= 0) return;

            style.richText = true;
            style.normal.textColor = Color.white;
            
            string output = candidateList[dropdownIndex].stringify();

            GUILayout.Space(10f);
            GUILayout.Label(output, style);
        }

        void guiSelection()
        {

            GameObject obj = Selection.activeGameObject;
            
            if (obj != currentSelection)
            {
                dropdownIndex = 0;
                currentSelection = obj;

                // update candidates
                candidateList = getCandidates();

                guiUpdateOption();

                return;
            }

            // clear for force update
            if (obj == null)
            {
                currentSelection = null;
            }

            if (options == null) return;

            if (options.Length <= 0)
            {
                GUILayout.Label("no options");
                return;
            }

            GUILayout.BeginHorizontal();
            dropdownIndex = EditorGUILayout.Popup("Objects", dropdownIndex, options, EditorStyles.popup);
            GUILayout.EndHorizontal();
        }

        void guiUpdateOption()
        {
            if (candidateList == null) return;

            if (candidateList.Length <= 0)
            {
                GUILayout.Label("no candidates on "+currentSelection.name);
                return;
            }

            List<string> newOptions = new List<string>();
            for (int i = 0; i < candidateList.Length; i++)
            {
                newOptions.Add(candidateList[i].GetType().ToString());
            }
            options = newOptions.ToArray();

        }

        iScaffLog[] getCandidates()
        {
            if (currentSelection == null) return new iScaffLog[0];

            //MonoBehaviour[] tmp = GameObject.FindObjectsOfType<MonoBehaviour>();
            MonoBehaviour[] tmp = currentSelection.GetComponentsInChildren<MonoBehaviour>();
            List<iScaffLog> output = new List<iScaffLog>();
            for (int i = 0; i < tmp.Length; i++)
            {
                iScaffLog inst = tmp[i] as iScaffLog;
                if (inst != null) output.Add(inst);
            }
            return output.ToArray();
        }

        void Update()
        {
            if (!EditorApplication.isPlaying) return;
            if (EditorApplication.isPaused) return;

            Repaint();
        }

    }

}
