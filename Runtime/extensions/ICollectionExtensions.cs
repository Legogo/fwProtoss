using System.Collections.Generic;
using System.Collections;
using UnityEngine;


/// <summary>
/// 2016-01-10 Romain Péchot
/// 
/// Regroupe les méthodes statique et d'extensions associées
/// a l'interface ICollection. ICollection est intégré par défaut sur toutes
/// les classes de type Array (T[], List générique, etc).
/// Nb.: ne marche pas sur les HashSet !
/// https://msdn.microsoft.com/fr-fr/library/92t2ye13(v=vs.110).aspx
/// </summary>
static public class ICollectionExtensions
{
	/// <summary>
	/// Permet de rapidement vérifier si un tableau est null ou vide de valeurs.
	/// </summary>
	/// <param name="icol">La Collection cible.</param>
	/// <returns>Renvois True si la référence de la ICollection est null ou si la ICollection ne contient pas d'éléments.</returns>
	static public bool isNullOrEmpty(this ICollection icol)
	{
		return icol == null || icol.Count == 0;

	}// isNullOrEmpty()


	/// <summary>
	/// Retourne l'index du dernier élément de la ICollection.
	/// </summary>
	/// <param name="icol">La ICollection cible.</param>
	/// <returns>Renvois ICollection.Count - 1.</returns>
	static public int getLastIndex(this ICollection icol)
	{
		return icol.Count - 1;

	}// getLastIndex()


	/// <summary>
	/// Retourne une valeur aléatoire entre 0 et ICollection.Count - 1.
	/// </summary>
	/// <param name="icol">La ICollection cible.</param>
	/// <returns>L'index aléatoire.</returns>
	static public int getRandomIndex(this ICollection icol)
	{
		return Random.Range(0, icol.Count);

	}// getRandomIndex()


	/// <summary>
	/// Retourne une valeur d'index aléatoire qui exclut un index spécifique.
	/// </summary>
	/// <param name="icol">La ICollection cible.</param>
	/// <param name="excludeIndex">L'index exclut de l'aléatoire.</param>
	/// <returns>L'index généré aléatoirement.</returns>
	static public int getRandomIndex(this ICollection icol, int excludeIndex)
	{
		List<int> indexes = new List<int>(icol.Count);

		for(int i = 0; i < icol.Count; i++)
		{
			indexes.Add(i);
		}

		indexes.RemoveAt(excludeIndex);

		return indexes[indexes.getRandomIndex()];

	}// getRandomIndex()


	/// <summary>
	/// Removes at source and index.
	/// </summary>
	/// <returns>Returns the original collection without the </returns>
	/// <param name="source">Source is the original collection</param>
	/// <param name="index"></param>
	public static T[] removeAt<T>(this T[] source, int index)
	{
		T[] dest = new T[source.Length - 1];
		if( index > 0 )
			System.Array.Copy(source, 0, dest, 0, index);

		if( index < source.Length - 1 )
			System.Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

		return dest;
	}


	/// Récupère un élément au hasard dans un tableau générique.
	/// </summary>
	/// <typeparam name="T">Le Type du tableau générique.</typeparam>
	/// <param name="array">Le tableau cible.</param>
	/// <returns>L'instance aléatoire.</returns>
	static public T getRandomValue<T>(this T[] array)
	{
		return array[array.getRandomIndex()];

	}// getRandomValue()


	/// <summary>
	/// Récupère un élément au hasard dans une liste générique.
	/// </summary>
	/// <typeparam name="T">Le Type de la liste générique.</typeparam>
	/// <param name="list">La liste cible.</param>
	/// <returns>L'élément sélectionné aléatoirement.</returns>
	static public T getRandomValue<T>(this List<T> list)
	{
		return list[list.getRandomIndex()];

	}// getRandomValue()


	/// <summary>
	/// Cherche dans un talbeau si la référence ou valeur indiqué existe.
	/// https://msdn.microsoft.com/fr-fr/library/x0b5b5bc(v=vs.110).aspx
	/// </summary>
	/// <typeparam name="T">Le type du tableau générique.</typeparam>
	/// <param name="array">Le tableau cible.</param>
	/// <param name="val">L'instance (class) ou la valeur (struct) a trouver dans le tableau.</param>
	/// <returns>True si l'élément existe dans le tableau.</returns>
	static public bool contains<T>(this IEnumerable<T> array, T val)
	{
		return array.findIndex(val) > -1;

	}// contains()

	
	/// <summary>
	/// Croise deux tableau afin de vérifier s'ils ont des valeurs en commun.
	/// </summary>
	/// <typeparam name="T">Le Type des deux tableaux.</typeparam>
	/// <param name="array0">Le premier tableau.</param>
	/// <param name="array1">Le second talbeau</param>
	/// <returns>Vrai si les deux talbeaux ont des valeurs en commun.</returns>
	static public bool cross<T>(IEnumerable<T> array0, IEnumerable<T> array1)
	{
		IEnumerator iEnumerator = array0.GetEnumerator();

		while(iEnumerator.MoveNext())
		{
			if(array1.contains((T)iEnumerator.Current))
			{
				return true;
			}
		}

		return false;

	}// cross()

	
	/// <summary>
	/// Cherche un élément dans un tableau en utilisant le EqualityComparer.Default.
	/// Renvois un index supérieur a -1 si l'élément a été trouvé.
	/// </summary>
	/// <typeparam name="T">Le Type du talbeau/élément.</typeparam>
	/// <param name="array">Le Tableau cible.</param>
	/// <param name="val">L'élément cible.</param>
	/// <returns>L'index de position de l'élément dans le tableau.</returns>
	static public int findIndex<T>(this IEnumerable<T> array, T val)
	{
		IEnumerator iEnumerator = array.GetEnumerator();

		int index = 0;

		while(iEnumerator.MoveNext())
		{
			if(EqualityComparer<T>.Default.Equals((T)iEnumerator.Current, val))
			{
				return index;
			}

			index++;
		}
    
		return -1;

	}// findIndex()


	/// <summary>
	/// Retourne le nombre de valeur/instance équivalente à la valeur/ref donnée dans notre tableau.
	/// </summary>
	/// <typeparam name="T">Le Type principal (struct/class)</typeparam>
	/// <param name="array">Le tableau recherché.</param>
	/// <param name="val">La valeur recherchée.</param>
	/// <returns>Le nombre d'occurence(s).</returns>
	static public int countIndexes<T>(this IEnumerable<T> array, T val)
	{
		IEnumerator iEnumerator = array.GetEnumerator();

		int count = 0;

		while(iEnumerator.MoveNext())
		{
			if(EqualityComparer<T>.Default.Equals((T)iEnumerator.Current, val))
			{
				count++;
			}
		}

		return count;

	}// countIndexes()


}// static public class ICollectionExtensions
