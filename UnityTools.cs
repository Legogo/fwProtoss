using System.Reflection;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Lots of various tools to write things faster in unity
/// Contributors : 
///   André Berlemont @andreberlemont
///   Romain Pechot @opotable
/// </summary>

#region ui

static public class ToolsUi
{
  
  static public Image setupImage(Transform tr, Sprite spr, Color tint, bool visibility = true)
  {
    Image img = tr.GetComponent<Image>();
    if (img == null) return null;

    img.sprite = spr;
    img.color = tint;

    img.enabled = visibility;
    if (img.sprite == null) img.enabled = false;

    return img;
  }

}

#endregion

#region string

static public class ToolsString
{
  static public string addZeros(int val, int digit = 2) {
    string output = val.ToString();
    if (digit >= 2 && val < 10) output = "0" + output;
    if (digit >= 3 && val < 100) output = "0" + output;
    return output;
  }
}

#endregion

#region object

static public class ToolsObject
{
	public const string newLine = "\n";

  static public string getObjectPath(Transform tr) {
    string path = "";
    while(tr != null) {
      path = tr.name + "/" + path;
      tr = tr.parent;
    }
    return path;
  }

	/// <summary>
	/// Encapsule (obj != null && obj.GetType() == typeof())
	/// </summary>
	/// <param name="obj">Le UnityEngine.Object cible.</param>
	/// <param name="type">Le Type de l'objet.</param>
	/// <returns>True si le Type est bon.</returns>
	static public bool isTypeOf(this Object obj, System.Type type)
	{
		return obj != null && obj.GetType() == type;

	}// isTypeOf()
}

#endregion

#region objects

static public class ToolsObjects
{
  static public void destroy(Object obj) {
    if(obj != null) {
      //GameObject go = (GameObject)obj;
      //Debug.Log("<color=red>/!\\ ToolsObject::destroy()</color> "+obj.GetType()+", name : "+obj.name);
      GameObject.DestroyImmediate(obj);
    }
  }

  static public bool isMissingRefs(params Object[] objects) {
    for(int i = 0; i < objects.Length; i++) if(objects[i] == null) return true;
      return false;
  }
}

#endregion

#region color

static public class ToolsColor
{
	public const string parserSeparator = ";";

	/// <summary>
	/// Retourne une Color en utilisant les paramètres [0,255] au lieu de [0,1]
	/// </summary>
	/// <param name="r">Le canal Rouge de la couleur.</param>
	/// <param name="g">Le canal Vert de la couleur.</param>
	/// <param name="b">Le canal Bleu de la couleur.</param>
	/// <param name="a">Le canal Alpha/Transparent de la couleur.</param>
	/// <returns></returns>
	static public Color rgbToColor(float r, float g, float b, float a = 255f)
	{
		return new Color(
		Mathf.InverseLerp(0f, 255f, r),
		Mathf.InverseLerp(0f, 255f, g),
		Mathf.InverseLerp(0f, 255f, b),
		Mathf.InverseLerp(0f, 255f, a)
		);

	}//rgbToColor()

	static public string parse(Color color)
	{
		string data = string.Empty;

		for(int i = 0; i < 4; i++)
		{
			data += color[i].ToString();

			if(i < 3)
			{
				data += parserSeparator;
			}
		}

		return data;

	}// parse()

	static public Color parse(string data)
	{
		Color color = Color.magenta;

		if(!string.IsNullOrEmpty(data))
		{
			string[] channels = data.Split(new string[1] { parserSeparator }, System.StringSplitOptions.None);

			if(channels != null && channels.Length == 4)
			{
				for(int i = 0; i < channels.Length; i++)
				{
					color[i] = float.Parse(channels[i]);
				}
			}
		}

		return color;

	}// parse()

  /// <summary>
  /// Retourne une Color avec le canal d'Alpha modifié.
  /// </summary>
  /// <param name="color">La Color cible.</param>
  /// <param name="a">La valeur du canal Alpha [0,1]</param>
  /// <returns>La Color avec le canal d'alpha modifié.</returns>
  static public Color setAlpha(this Color color, float a = 1f)
	{
		color.a = a;

		return color;

	}// setAlpha()

	/// <summary>
	/// Retourne une Color avec la valeur d'alpha multiplié.
	/// </summary>
	/// <param name="color">La Color cible</param>
	/// <param name="power">Le multiplicateur du canal d'alpha</param>
	/// <returns>La Color ainsi modifiée</returns>
	static public Color multAlpha(this Color color, float power = 1f)
	{
		color.a *= power;

		return color;

	}// multAlpha()

}// class ToolsColor

#endregion

#region component

static public class ToolsComponent
{

  public static T getComponentByCarryName<T>(string carryName) {
    GameObject obj = GameObject.Find(carryName);

    if (obj == null)
    {
      Debug.LogWarning("couldn't find " + carryName);
      return default(T);
    }

    return obj.GetComponent<T>();
  }

  /// <summary>
  /// Permet de virer tout enfants et component (sauf le transform)
  /// </summary>
  public static void cleanTransform(Transform tr)
  {

    //HierarchyAnimatorHighlighter.ShowIcon(!HierarchyAnimatorHighlighter.ShowIcon());
    if (tr == null) return;
    
    Debug.Log("cleaning " + tr.name);

    //remove all children
    while (tr.childCount > 0)
    {
      GameObject.DestroyImmediate(tr.GetChild(0).gameObject);
    }
    
    SpriteRenderer[] renders = tr.GetComponents<SpriteRenderer>();
    foreach (SpriteRenderer render in renders) { GameObject.DestroyImmediate(render); }

    Collider[] colliders = tr.GetComponents<Collider>();
    foreach (Collider collider in colliders) { GameObject.DestroyImmediate(collider); }


  }
  
	static public MeshRenderer fetchMeshRendererInParent(Transform tr)
	{
		MeshRenderer _tmp = null;
		while (_tmp == null && tr != null)
		{
			_tmp = tr.GetComponent<MeshRenderer>();
			if (_tmp == null) tr = tr.parent;
		}
		return _tmp;
	}

	static public SpriteRenderer fetchSpriteRendererInParent(Transform tr)
	{
		SpriteRenderer _tmp = null;
		while (_tmp == null && tr != null)
		{
			_tmp = tr.GetComponent<SpriteRenderer>();
			if (_tmp == null) tr = tr.parent;
		}
		return _tmp;
	}

}

#endregion

#region transform

static public class ToolsTransform
{
	/// <summary>
	/// Retourne le nombre de parent du Transform.
	/// </summary>
	/// <param name="transform">Le Transform ciblé.</param>
	/// <returns>Le nombre de Transform parent, 0 si ce Transform n'a pas de parent (le pauvre).</returns>
	static public int getHierarchyDepth(this Transform transform)
	{
		int depth = 0;

		Transform parent = transform.parent;

		while(parent != null)
		{
			depth++;

			parent = parent.parent;
		}

		return depth;

	}// getHierarchyDepth()

	static public Transform[] getAllTransform(GameObject[] list)
	{
		List<Transform> tmp = new List<Transform>();
		for (int i = 0; i < list.Length; i++)
		{
			tmp.AddRange(getAllTransform(list[i].transform));
		}
		return tmp.ToArray();
	}

	static public Transform[] getAllTransform(Transform t)
	{
		List<Transform> trs = new List<Transform>();
		trs.Add(t);
		foreach (Transform child in t)
		{
			if (child.childCount > 0)
			{
				Transform[] children = getAllTransform(child);
				//for (int i = 0; i < children.Length; i++) Debug.Log(child.name+" >> "+children[i].name);
				trs.AddRange(children);
			}
			else
			{
				trs.Add(child);
			}
		}
		return trs.ToArray();
	}

	static public Transform fetchInChildren(Transform parent, string partName, bool strict = false, bool toLowercase = false)
	{
		foreach (Transform t in parent)
		{
			string nm = t.name;
			if (toLowercase) nm = nm.ToLower();

			if (strict)
			{
				if (nm == partName) return t;
			}
			else
			{
				if (nm.IndexOf(partName) > -1) return t;
			}

			Transform child = fetchInChildren(t, partName, strict, toLowercase);
			if (child != null) return child;
		}
		return null;
	}


	static public bool isInChildren(Transform parent, Transform target)
	{
		bool isIn = false;

		if(parent == target) isIn = true;

		if(!isIn)
		{
			foreach(Transform child in parent)
			{
				if (isIn) continue;

				if (child == target) isIn = true;

				if (child.childCount > 0) isIn = isInChildren(child, target);
			}
		}

		return isIn;
	}

	/// <summary>
	/// Retourne une chaine de la hierarchie du Transform dans la scène.
	/// </summary>
	/// <param name="transform">Le Transform cible.</param>
	/// <returns>Le chemin d'accès au Transform dans la scène.</returns>
	static public string getScenePath(this Transform transform)
	{
		string path = transform.name;

		Transform parent = transform.parent;

		while(parent != null)
		{
			path = parent.name + "/" + path;

			parent = parent.parent;
		}

		return path;
	}

	/// <summary>
	/// Permet de remonter a un parent précis dans la hiérarchie.
	/// </summary>
	/// <param name="transform">Le Transform de départ</param>
	/// <param name="reverseHierarchyStep">Le nombre d'étape pour remontrer la hiérarchie.</param>
	/// <returns>La référence du Transform parent.</returns>
	static public Transform getTransformParent(this Transform transform, int reverseHierarchyStep = -1)
	{
		if(reverseHierarchyStep > 0)
		{
			Debug.LogWarning("reverseHierarchyStep doit toujours être inférieur ou égal à 0 !");

			return null;
		}
		
		while(reverseHierarchyStep < 0 && transform != null)
		{
			transform = transform.parent;

			reverseHierarchyStep++;
		}

		return transform;
	}

	/// <summary>
	/// Renvoit le premier Transform parent qui contient <paramref name="name"/> dedant.
	/// </summary>
	/// <param name="transform">Le Transform de départ de la recherche</param>
	/// <param name="name">Le nom à chercher.</param>
	/// <param name="warning">Génère-t-on un warning si on ne trouve rien ?</param>
	/// <returns>La référence du Transform parent trouvé.</returns>
	static public Transform looselyFindParent(this Transform transform, string name, bool warning = true)
	{
		Transform parent = transform.parent;

		if(!string.IsNullOrEmpty(name))
		{
			while(parent != null)
			{
				if(parent.name.Contains(name))
				{
					return parent;
				}
				else parent = parent.parent;
			}
		}

		if(warning) Debug.LogWarning("Can't find Transform's parent with name \"" + name + "\"", transform);

		return parent;

	}// looselyFindParent()

  

	static public Transform[] findSameChildren(Transform transformOrigin, Transform transformFilter)
	{
		List<Transform> sameChildrenOrigin = new List<Transform>();

		foreach(Transform childOrigin in transformOrigin)
		{
			foreach(Transform childFilter in transformFilter)
			{
				if(string.Compare(childOrigin.name, childFilter.name) == 0)
				{
					sameChildrenOrigin.Add(childOrigin);
				}
			}
		}

		return sameChildrenOrigin.ToArray();

	}// findSameChildren()


}// class ToolsTransform

#endregion

#region colliders

static public class ToolsColliders
{
	static public Collider fetchCapsuleColliderInParent(Transform tr)
	{
		CapsuleCollider _tmp = null;

		while (_tmp == null && tr != null)
		{
			_tmp = tr.GetComponent<CapsuleCollider>();
			if (_tmp == null) tr = tr.parent;
		}

		return _tmp;
	}

	static public Collider fetchColliderInParent(Transform tr)
	{
		Collider _tmp = null;

		while (_tmp == null && tr != null)
		{
			_tmp = tr.GetComponent<Collider>();
			if (_tmp == null) tr = tr.parent;
		}

		return _tmp;
	}
}

#endregion




static public class ToolsLayer
{
  //http://answers.unity3d.com/questions/150690/using-a-bitwise-operator-with-layermask.html
  static public bool isInLayerMask(GameObject obj, LayerMask layerMask)
  {
    return ((layerMask.value & (1 << obj.layer)) > 0);
  }

  static public bool hasLayerMask(GameObject obj, string layerName)
  {
    //Debug.Log(obj.layer);
    //Debug.Log(LayerMask.NameToLayer(layerName));
    return obj.layer == LayerMask.NameToLayer(layerName);
  }

  static public void removeLayerMask(GameObject obj) { if (obj.layer != 0) obj.layer = 0; }

  static public void assignLayerMask(GameObject obj, string layerName)
  {
    int newLayer = LayerMask.NameToLayer(layerName);
    if (newLayer != obj.layer) obj.layer = newLayer;
  }
}

static public class ToolsManager
{

  static public T getManager<T>(string nm) where T : MonoBehaviour
  {
    GameObject obj = GameObject.Find(nm);
    T tmp = null;
    if (obj != null)
    {
      tmp = obj.GetComponent<T>();
    }

    if (tmp != null) return tmp;

    if (obj == null)
    {
      obj = new GameObject(nm, typeof(T));
      tmp = obj.GetComponent<T>();
    }
    else tmp = obj.AddComponent<T>();

    return tmp;
  }




  static public T getManager<T>(string nm, bool dontDestroy = false) where T : MonoBehaviour
  {
    GameObject obj = GameObject.Find(nm);
    T tmp = null;
    if (obj != null)
    {
      tmp = obj.GetComponent<T>();
    }

    if (tmp != null) return tmp;

    if (obj == null)
    {
      obj = new GameObject(nm, typeof(T));
      tmp = obj.GetComponent<T>();
    }
    else tmp = obj.AddComponent<T>();

    if (dontDestroy) GameObject.DontDestroyOnLoad(tmp);

    return tmp;
  }

  static public GameObject getGameObject(string nm)
  {
    GameObject obj = GameObject.Find(nm);
    if (obj == null) obj = new GameObject(nm);
    return obj;
  }

  static public bool openedSceneCategorie(string part)
  {
    Scene sc = SceneManager.GetActiveScene();
    if (sc.name.Contains(part)) return true;
    return false;
  }

}




#if UNITY_EDITOR
static public class ToolsEditor
{
	static public void editorCenterCameraToObject(GameObject obj)
	{
		GameObject tmp = Selection.activeGameObject;

		Selection.activeGameObject = obj;

		if(SceneView.lastActiveSceneView != null)
		{
			SceneView.lastActiveSceneView.FrameSelected();
		}

		if (tmp != null) Selection.activeGameObject = tmp;
	}


	static public string getAssetFullPath(Object obj)
	{
		return Application.dataPath.Remove(Application.dataPath.LastIndexOf("Assets")) + AssetDatabase.GetAssetPath(obj);

	}// getAssetFullPath()

	
	/// <summary>
	/// get the sorting layer names<para/>
	/// from : http://answers.unity3d.com/questions/585108/how-do-you-access-sorting-layers-via-scripting.html
	/// </summary>
	/// <returns>The Sorting Layers (as string[])</returns>
	static public string[] getSortingLayerNames()
	{
		System.Type internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);

		PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);

		return (string[])sortingLayersProperty.GetValue(null, new object[0]);

	}// getSortingLayerNames()

	/// <summary>
	/// get the unique sorting layer IDs<para/>
	/// from : http://answers.unity3d.com/questions/585108/how-do-you-access-sorting-layers-via-scripting.html
	/// </summary>
	/// <returns>The sorting layer unique IDs (as int[])</returns>
	static public int[] getSortingLayerUniqueIDs()
	{
		System.Type internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);

		PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);

		return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);

	}// getSortingLayerUniqueIDs()


	/// <summary>
	/// Récupère un tableau des scènes/chemin d'accès qui sont présente dans les paramètres du build
	/// </summary>
	/// <param name="removePath">Juste le nom (myScene) ou tout le chemin d'accès (Assets/folder/myScene.unity) ?</param>
	/// <returns>Le tableau avec le nom ou chemin d'accès aux scènes.</returns>
	static public string[] getAllBuildScenes(bool includeSceneOnly, bool removePath)
	{
		string[] scenes = new string[] { };

		if(includeSceneOnly)
		{
			scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
		}
		else
		{
			EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

			scenes = new string[buildScenes.Length];

			for(int i = 0; i < scenes.Length; i++)
			{
				scenes[i] = buildScenes[i].path;
			}
		}

		if(removePath)
		{
			for(int i = 0; i < scenes.Length; i++)
			{
				int slashIndex = scenes[i].LastIndexOf('/');

				if(slashIndex >= 0)
				{
					scenes[i] = scenes[i].Substring(slashIndex + 1);
				}

				scenes[i] = scenes[i].Remove(scenes[i].LastIndexOf(".unity"));
			}

			return scenes;
		}
		else return scenes;

	}// getAllBuildScenesNames()
  
	/// <summary>
	/// Joue l'AudioClip visé par assetPath.<para/>
	/// Le chemin commence a la racine du projets et fini par l'extension de l'AudioClip.<para/>
	/// Exemple : "Assets/Unexported/Sounds/onDebugLogError.ogg"
	/// </summary>
	/// <param name="audioClipAssetPath">Le chemin relatif au projet de l'AudioClip.</param>
	static public bool playAudioClip(string audioClipAssetPath)
	{
		AudioClip audioClip = (AudioClip)EditorGUIUtility.Load(audioClipAssetPath);

		playAudioClip(audioClip);

		return audioClip != null;

	}// playAudioClip()


	/// <summary>
	/// Joue un son dans l'éditeur Unity.<para/>
	/// ATTENTION : utilise AudioPreview pour jouer le son, du coup le son est joué a son volume maximum !<para/>
	/// Nb.: Utilisez UnityEditor.EditorGUIUtility.Load("Assets/.../audioClipName.ext") pour récupérer votre référence a l'audioClip.
	/// </summary>
	/// <param name="audioClip">L'AudioClip a jouer.</param>
	static public void playAudioClip(AudioClip audioClip)
	{
		if(audioClip == null) return;

		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

		System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

		MethodInfo method = audioUtilClass.GetMethod("PlayClip", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { typeof(AudioClip) }, null );
		
		method.Invoke(null, new object[] { audioClip } );

	}// playClip()


	/// <summary>
	/// Stop tous les son actif de l'éditeur (pour la plupart lancés via playClip().
	/// </summary>
	static public void stopAllClips()
	{
		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

		System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

		MethodInfo method = audioUtilClass.GetMethod("StopAllClips", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { }, null);

		method.Invoke(null, new object[] { });

	}// stopAllClips()


	/// <summary>
	/// Retourne l'ID local d'un UnityEngine.Object dans une scène<para/>
	/// Viens de https://forum.unity3d.com/threads/how-to-get-the-local-identifier-in-file-for-scene-objects.265686/
	/// </summary>
	/// <param name="obj">L'objet cible.</param>
	/// <returns>L'ID de l'objet. Retourne 0 ou -1 si pas sauvegardé.</returns>
	static public int getLocalIdInFile(Object obj)
	{
		if(obj == null) return -1;

		PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);

		SerializedObject serializeObject = new SerializedObject(obj);

		inspectorModeInfo.SetValue(serializeObject, InspectorMode.Debug, null);

		SerializedProperty propertyLocalID = serializeObject.FindProperty("m_LocalIdentfierInFile");

		int localID = propertyLocalID.intValue;

		inspectorModeInfo.SetValue(serializeObject, InspectorMode.Normal, null);

		return localID;

	}// getLocalIdInFile()


}// class ToolsEditor
#endif
