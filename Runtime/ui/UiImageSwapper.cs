using System.Collections;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// pour facilement changer l'image d'un <Image> basé sur une liste
/// </summary>

namespace fwp.engine.ui
{
    public class UiImageSwapper : UiObject
    {
        public Sprite[] frames;

        public void swap(string endName)
        {
            for (int i = 0; i < frames.Length; i++)
            {
                if (frames[i].name.EndsWith(endName))
                {
                    swap(i);
                    return;
                }
            }
        }
        public void swap(int frameIdx)
        {
            visirUi.setSymbol(frames[frameIdx]);
        }

    }

}
