using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace fwp.logs
{
	using fwp.halpers;
	using fwp.halpers.editor;

	public class WinEditorLogs : EditorWindow
	{
		[MenuItem("Tools/(win) editor logs filter")]
		static void init()
		{
			EditorWindow.GetWindow(typeof(WinEditorLogs));
		}

		private void OnGUI()
		{
			GUILayout.Label($"LOGS ED-MONITOR", HalperGuiStyle.getWinTitle());

			bool forceClear = false;
			if (GUILayout.Button("clear all"))
			{
				forceClear = true;
			}

			string[] lvls = System.Enum.GetNames(typeof(VerbosityLevel));
			for (int i = 0; i < lvls.Length; i++)
			{
				if (forceClear) WinEdFieldsHelper.drawToggle(lvls[i], lvls[i], false);
				else WinEdFieldsHelper.drawToggle(lvls[i], lvls[i]);
			}
		}

	}

}
