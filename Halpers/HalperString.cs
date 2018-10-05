using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalperString
{
  static public string addZeros(int val, int digit = 2)
  {
    string output = val.ToString();
    if (digit >= 2 && val < 10) output = "0" + output;
    if (digit >= 3 && val < 100) output = "0" + output;
    return output;
  }
}
