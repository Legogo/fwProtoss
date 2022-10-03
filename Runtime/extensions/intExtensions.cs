using UnityEngine;


/// <summary>
/// 05/01/2017 - Romain Péchot
/// 
/// Classe statique qui rassemble les méthodes d'extensions
/// associées a la structure int.
/// </summary>
static public class intExtensions
{
	/// <summary>
	/// Décale la value d'un int de stepValue et la fait boucler entre valueMin et valueMax.
	/// Utilisé pour déplacer rapidement un index.
	/// </summary>
	/// <param name="currentValue">L'int référent</param>
	/// <param name="stepValue">L'écart appliqué a currentValue</param>
	/// <param name="valueMin">La valeur minimal autorisée</param>
	/// <param name="valueMax">La valeur maximal autorisée</param>
	static public int getStep(this int currentValue, int stepValue, int valueMin, int valueMax)
	{
		if(valueMin >= valueMax)
		{
			Debug.LogError(string.Format("Min ({0}) can't be HigherOrEqual than Max ({1})!", valueMin, valueMax));

			return currentValue;
		}

		currentValue += stepValue;

		int length = valueMax + 1 - valueMin;

		while(currentValue < valueMin)
		{
			currentValue += length;
		}

		while(currentValue > valueMax)
		{
			currentValue -= length;
		}

		return currentValue;

	}// Step()


	/// <summary>
	/// Avance d'un cran la valeur de l'int. Boucle si supérieur a l'index max.
	/// Encapsule la méthode Step() pour simplifier l'appel
	/// lorsque l'int est utilisé comme index d'un tableau.
	/// </summary>
	/// <param name="currentIndex">La valeur de l'index en cours</param>
	/// <param name="indexMax">La valeur max de l'index (souvent égal a Length - 1)</param>
	static public int getIndexNext(this int currentIndex, int indexMax)
	{
		return currentIndex.getStep(1, 0, indexMax);

	}// getIndexNext()
	

	/// <summary>
	/// Recule d'un cran la valeur de l'int. Boucle si inférieur a 0.
	/// Encapsule la méthode Step() pour simplifier l'appel
	/// lorsque l'int est utilisé comme index d'un tableau.
	/// </summary>
	/// <param name="currentIndex">La valeur de l'index en cours</param>
	/// <param name="indexMax">La valeur max de l'index (souvent égal a Length - 1)</param>
	static public int getIndexPrevious(this int currentIndex, int indexMax)
	{
		return currentIndex.getStep(-1, 0, indexMax);

	}// getIndexPrevious()


}// class intExtensions
