using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace fwp.engine.ui
{
    public class UiBump : UiAnimation
    {

        public float bumpStr = 2f;
        protected Vector3 scaleBase;

        protected override void created()
        {
            base.created();

            scaleBase = rec.transform.localScale;
            if (scaleBase.magnitude == 0f) Debug.LogError("scale is zero");
        }

        protected override void animStart()
        {
            base.animStart();
            rec.localScale = scaleBase;
        }

        protected override void updateAnimationProcess()
        {
            rec.transform.localScale = Vector3.Lerp(scaleBase, scaleBase * bumpStr, getProgress());

            //Debug.Log(scaleBase+" , "+ rec.transform.localScale);
        }

        static public void callBump(GameObject obj)
        {

            UiBump bump = obj.GetComponent<UiBump>();
            if (bump != null) bump.play();

        }
    }
}
