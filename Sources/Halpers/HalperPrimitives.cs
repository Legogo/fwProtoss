using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class HalperPrimitives
{

  static public float parseFloat(string input)
  {
    return float.Parse(input,
      System.Globalization.NumberStyles.Any,
      System.Globalization.CultureInfo.InvariantCulture);
  }

}
