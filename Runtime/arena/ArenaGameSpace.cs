﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// only works for 2D orthographics game
/// </summary>

namespace fwp.arena
{
    public class ArenaGameSpace : MonoBehaviour
    {

        [Header("setup")]

        public float width = 3f;
        public float height = 3f;

        public bool matchScreen = false;

        public Vector2 offset = Vector2.zero;
        public float topBorder;
        public float bottomBorder;

        [Header("solved")]
        [HideInInspector] public Rect screenSpace = new Rect();
        [HideInInspector] public Rect offsetSpace = new Rect(); // screenspace + offset

        protected Vector2 screenBotLeft = Vector2.zero;
        protected Vector2 screenTopRight = Vector2.zero;

        [Header("tools")]
        public Transform[] borders;

        static public ArenaGameSpace gameSpace;

        private void Awake()
        {
            gameSpace = this;
            updateSize();
        }

        private void Update()
        {
            updateSize();
        }

        [ContextMenu("resize")]
        public void updateSize()
        {

            if (Camera.main == null)
            {
                Debug.LogWarning("no main camera");
                return;
            }

            Camera c = Camera.main;

            if (matchScreen)
            {
                if (c.orthographic)
                {
                    screenBotLeft = Camera.main.ScreenToWorldPoint(Vector2.zero);
                    //screenTopRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
                    screenTopRight = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight));
                }

            }
            else
            {

                screenBotLeft.x = width * -0.5f;
                screenBotLeft.y = height * -0.5f;

                screenTopRight.x = width * 0.5f;
                screenTopRight.y = height * 0.5f;
            }

            //screen final
            screenSpace.xMin = screenBotLeft.x;
            screenSpace.xMax = screenTopRight.x;
            screenSpace.yMin = screenBotLeft.y;
            screenSpace.yMax = screenTopRight.y;

            //offset final
            offsetSpace.xMin = screenSpace.xMin + offset.x;
            offsetSpace.xMax = screenSpace.xMax + offset.x;
            offsetSpace.yMin = screenSpace.yMin + offset.y + bottomBorder;
            offsetSpace.yMax = screenSpace.yMax + offset.y - topBorder;

        }

        public Vector2 getRandomPositionBorder(float distance)
        {
            //Vector2 pos = Random.onUnitSphere * 10f;
            Vector2 pos = Vector2.zero;


            if (Random.value > 0.5f)
            {
                //sides

                pos.y = Random.Range(screenBotLeft.y, screenTopRight.y);

                if (Random.value > 0.5f)
                {
                    //left
                    pos.x = screenBotLeft.x - distance;
                }
                else
                {
                    //right
                    pos.x = screenTopRight.x + distance;
                }


            }
            else
            {
                //tops

                pos.x = Random.Range(screenBotLeft.x, screenTopRight.x);

                if (Random.value > 0.5f)
                {
                    //top
                    pos.y = screenTopRight.y + distance;
                }
                else
                {
                    //bottom
                    pos.y = screenBotLeft.y - distance;
                }

            }

            return pos;
        }

        public float getWidth() { return screenTopRight.x - screenBotLeft.x; }
        public float getHeight() { return screenTopRight.y - screenBotLeft.y; }

        public Vector2 getRandomPosition(float borderGap)
        {
            Vector2 pos = Vector2.zero;
            pos.x = Random.Range(screenBotLeft.x + borderGap, screenTopRight.x - borderGap);
            pos.y = Random.Range(screenBotLeft.y + borderGap, screenTopRight.y - borderGap);
            return pos;
        }

#if UNITY_EDITOR

        void OnDrawGizmos()
        {

            if (!Application.isPlaying)
            {
                updateSize();
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(new Vector2(screenSpace.xMin, screenSpace.yMin), new Vector2(screenSpace.xMax, screenSpace.yMax));

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(new Vector2(offsetSpace.xMin, offsetSpace.yMin), new Vector2(offsetSpace.xMax, offsetSpace.yMax));

            string ct = "~GameSpace~";
            ct += "\nScreen.width/height : " + Screen.width + "x" + Screen.height;
            ct += "\nCamera.width/heiht : " + Camera.main.pixelWidth + "x" + Camera.main.pixelHeight;
            ct += "\ncomputer resolution : " + Screen.currentResolution;
            UnityEditor.Handles.Label(transform.position + (Vector3.up * 4f) + Vector3.right, ct);

            UnityEditor.Handles.Label(new Vector3(screenSpace.xMax, screenSpace.yMax, 0f) + Vector3.right + Vector3.up, screenSpace.xMax + "x" + screenSpace.yMax);
            UnityEditor.Handles.Label(new Vector3(screenSpace.xMin, screenSpace.yMin, 0f) + Vector3.right + Vector3.down, screenSpace.xMin + "x" + screenSpace.yMin);

            if (borders != null && borders.Length > 0)
            {
                if (borders.Length >= 1)
                {
                    borders[0].position = new Vector2(-width * 0.5f, 0f);
                    borders[0].localScale = new Vector2(0.02f, width);
                    borders[0].rotation = Quaternion.identity;
                }
                if (borders.Length >= 2)
                {
                    borders[1].position = new Vector2(width * 0.5f, 0f);
                    borders[1].localScale = new Vector2(0.02f, width);
                    borders[1].rotation = Quaternion.identity;
                }
                if (borders.Length >= 3)
                {
                    borders[2].position = new Vector2(0f, height * 0.5f);
                    borders[2].localScale = new Vector2(0.02f, height);
                    borders[2].rotation = Quaternion.AngleAxis(90f, Vector3.forward);
                }
                if (borders.Length >= 4)
                {
                    borders[3].position = new Vector2(0f, -height * 0.5f);
                    borders[3].localScale = new Vector2(0.02f, height);
                    borders[3].rotation = Quaternion.AngleAxis(90f, Vector3.forward);
                }
            }
        }

#endif








        Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
        {
            // Get A,B,C of first line - points : ps1 to pe1
            float A1 = pe1.y - ps1.y;
            float B1 = ps1.x - pe1.x;
            float C1 = A1 * ps1.x + B1 * ps1.y;

            // Get A,B,C of second line - points : ps2 to pe2
            float A2 = pe2.y - ps2.y;
            float B2 = ps2.x - pe2.x;
            float C2 = A2 * ps2.x + B2 * ps2.y;

            // Get delta and check if the lines are parallel
            float delta = A1 * B2 - A2 * B1;
            if (delta == 0)
                throw new System.Exception("Lines are parallel");

            // now return the Vector2 intersection point
            return new Vector2(
                (B2 * C1 - B1 * C2) / delta,
                (A1 * C2 - A2 * C1) / delta
            );
        }


        static public ArenaGameSpace get()
        {
            ArenaGameSpace gs = GameObject.FindObjectOfType<ArenaGameSpace>();
            if (gs == null) return null;
            gs.updateSize();
            return gs;
        }
    }

}
