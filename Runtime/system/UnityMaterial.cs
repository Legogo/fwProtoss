using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.halpers
{
    public class UnityMaterial
    {
        protected Renderer render;
        protected Material mat;

        public UnityMaterial(Renderer render, bool copy = true, int materialIndex = 0)
        {
            if (render == null) Debug.LogError("no render given ?");

            if (copy) mat = render.materials[materialIndex];
            else mat = render.sharedMaterials[materialIndex];

            Debug.Log(render.name + " using material " + mat.name);
        }

        public IEnumerator processFade(float target, float speed = 1f)
        {
            Color col = mat.color;

            mat.color = col;

            yield return null;

            while (mat.color.a != target)
            {
                col.a = Mathf.MoveTowards(col.a, target, Time.deltaTime * speed);
                mat.color = col;

                if (Input.GetKeyUp(KeyCode.RightShift)) col.a = target;

                yield return null;
            }

        }

        public IEnumerator processTowardFloat(string floatName, float targetValue, float speed)
        {
            float val = mat.GetFloat(floatName);
            while (val != targetValue)
            {
                val = Mathf.MoveTowards(val, targetValue, Time.deltaTime * speed);
                mat.SetFloat(floatName, val);
                yield return null;
            }

            mat.SetFloat(floatName, targetValue);
        }

        public bool hasFloat(string floatName)
        {
            return mat.HasProperty(floatName);
        }

        public void setAlpha(float alpha)
        {
            Color col = mat.color;
            col.a = alpha;
            mat.color = col;
        }

        public void setFloat(string floatName, float floatValue)
        {
            if (!mat.HasProperty(floatName))
            {
                Debug.LogWarning("no property " + floatName + " on " + mat.name);
                return;
            }

            mat.SetFloat(floatName, floatValue);
        }
        public float getFloat(string floatName)
        {
            return mat.GetFloat(floatName);
        }


        static public Renderer getRendererOfTr(Transform carry, bool fetchInChildren = false)
        {
            Renderer render = carry.GetComponent<Renderer>();

            if (render == null && fetchInChildren) render = carry.GetComponentInChildren<Renderer>();
            if (render == null)
            {
                Debug.LogWarning("asking for unitymaterial but no render found on " + carry);
                return null;
            }

            return render;
        }

        static public void forceValue(Transform tr, string trName, string floatName, float val, int matIdx = 0)
        {
            if (!Application.isPlaying)
            {
                Transform halo = tr;
                if (trName.Length > 0) halo = HalperTransform.findChild(tr, trName);

                Renderer render = halo.GetComponentInChildren<Renderer>();
                Material mat = matIdx > 0 ? render.sharedMaterials[matIdx] : render.sharedMaterial;

                mat.SetFloat(floatName, val);

                //Debug.Log(mat.name + " , " + floatName + " , " + val);

                return;
            }

        }
    }

}
