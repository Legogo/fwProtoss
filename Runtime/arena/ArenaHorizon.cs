using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generate a line of horizon based on assets given in inspector
/// </summary>

namespace fwp.arena
{
    public class ArenaHorizon : MonoBehaviour
    {

        public Transform arenaBounds;
        public Sprite[] sprs;

        SpriteRenderer[] renders;

        void Start()
        {
            generate();
        }

        [ContextMenu("generate")]
        void generate()
        {

            while (transform.childCount > 0) GameObject.DestroyImmediate(transform.GetChild(0).gameObject);

            float width = sprs[0].bounds.extents.x * 2f;
            int qty = Mathf.FloorToInt(arenaBounds.transform.localScale.x / width);
            float start = transform.position.x - (arenaBounds.transform.localScale.x * 0.5f);

            for (int i = 0; i < qty; i++)
            {
                SpriteRenderer render = getRender();
                render.sprite = getRandomSprite();
                render.sortingOrder = 20;
                render.transform.localPosition = new Vector3(start + i * width, 0f, 0f);
            }
        }

        Sprite getRandomSprite()
        {
            return sprs[Random.Range(0, sprs.Length)];
        }

        SpriteRenderer getRender()
        {
            GameObject obj = new GameObject("render");
            SpriteRenderer spr = obj.AddComponent<SpriteRenderer>();
            obj.transform.parent = transform;
            return spr;
        }
    }

}
