using UnityEngine;


static public class GameObjectExtensions
{
	static public void removeCloneName(this GameObject gameObject)
	{
		gameObject.name = gameObject.name.Replace("(Clone)", string.Empty);

	}// removeCloneName()

}// class GameObjectExtensions
