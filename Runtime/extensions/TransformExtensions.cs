using UnityEngine;


public enum eTransfertType
{
	Instant = 0,
	Lerp = 1,
	MoveToward = 2
}


/// <summary>
/// 2017-01-11 Romain Péchot<para/>
/// 
/// Classe pour les méthodes statiques et d'extensions associées<para/>
/// a la classe Transform https://docs.unity3d.com/ScriptReference/Transform.html
/// </summary>
public static class TransformExtensions
{
	public enum eScaleAxis
	{
		XAxis = 0,
		YAxis = 1,
		ZAxis = 2
	}

	static private Vector3 localScale = Vector3.one;

	/// <summary>
	/// Une version 2D de la méthode LookAt() https://docs.unity3d.com/ScriptReference/Transform.LookAt.html <para/>
	/// Utilise l'axe X positif du Transform pour viser la position monde.
	/// </summary>
	/// <param name="transform">Le Transform qui va être réorienter.</param>
	/// <param name="worldTarget">La position monde cible.</param>
	static public void lookAt2D(this Transform transform, Vector3 worldTarget)
	{
		transform.right = worldTarget - transform.position;

	}// lookAt2D()

	static public Vector3 getPosition(this Transform transform, Space space)
	{
		return (space == Space.Self) ? transform.localPosition : transform.position;

	}// getPosition()

	static public void setPosition(this Transform transform, Vector3 position, Space space)
	{
		switch(space)
		{
			case Space.World: transform.position = position; break;

			case Space.Self: transform.localPosition = position; break;

			default:break;

		}// switch()

	}// setPosition()

	static public Quaternion getRotation(this Transform transform, Space space)
	{
		switch(space)
		{
			case Space.World: return transform.rotation;

			case Space.Self: return transform.localRotation;

			default:break;
		}

		return Quaternion.identity;

	}// getRotation()

	static public void setRotationEulerAngles(this Transform transform, Vector3 rotation, Space space)
	{
		switch(space)
		{
			case Space.World: transform.eulerAngles = rotation; break;

			case Space.Self: transform.localEulerAngles = rotation; break;

			default:break;

		}// switch()

	}// setRotationEulerAngles()

	static public void setRotation(this Transform transform, Quaternion rotation, Space space)
	{
		switch(space)
		{
			case Space.World: transform.rotation = rotation;break;

			case Space.Self: transform.localRotation = rotation;break;

			default:break;

		}// switch()

	}// setRotation()

	static public void setLocalScale(this Transform transform, eScaleAxis axis, float value)
	{
		localScale = transform.localScale;

		switch(axis)
		{
			case eScaleAxis.XAxis: localScale.x = value;break;

			case eScaleAxis.YAxis: localScale.y = value;break;

			case eScaleAxis.ZAxis: localScale.z = value;break;

			default:break;

		}// switch()

		transform.localScale = localScale;

	}// setLocalScale()
	
	static public string getHierarchyPath(this Transform transform, Transform scope = null)
	{
		string path = string.Empty;

		Transform target = transform;

		bool inScope = true;

		do
		{
			path = target.name + "/" + path;

			inScope = (scope == null || !scope.IsChildOf(target));

			target = target.parent;
		}
		while(target != null && inScope);

		if(!string.IsNullOrEmpty(path))
		{
			// remove last '/'
			path = path.Remove(path.Length - 1);
		}

		return path;

	}// getHierarchyPath()

}// class TransformExtensions
