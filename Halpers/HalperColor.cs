using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class HalperColor
{
  public const string colorParserSeparator = ";";

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

  //static public Color htmlToColor(string hex){}

  static public string parse(Color color)
  {
    string data = string.Empty;

    for (int i = 0; i < 4; i++)
    {
      data += color[i].ToString();

      if (i < 3)
      {
        data += colorParserSeparator;
      }
    }

    return data;

  }// parse()

  static public Color parse(string data)
  {
    Color color = Color.magenta;

    if (!string.IsNullOrEmpty(data))
    {
      string[] channels = data.Split(new string[1] { colorParserSeparator }, System.StringSplitOptions.None);

      if (channels != null && channels.Length == 4)
      {
        for (int i = 0; i < channels.Length; i++)
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
  }

  static public Color towardAlpha(this Color color, float target = 1f, float speed = 1f)
  {
    color.a = Mathf.MoveTowards(color.a, target, speed);
    return color;
  }

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

}
