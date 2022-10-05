using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.engine
{
    public interface iListCandidate
    {
        void update();
    }

    public class ListUpdater<T> where T : iListCandidate
    {
        public List<T> candidates = new List<T>();

        MonoBehaviour mono;
        Coroutine coActive;

        public ListUpdater()
        {
            var carry = new GameObject("[ListUpdater]~" + Random.Range(0, 10000));
            mono = carry.AddComponent<ListUpdaterComponent>();
        }
        public ListUpdater(MonoBehaviour carry)
        {
            mono = carry;
        }

        public void sub(T candid)
        {
            int cnt = candidates.Count;

            if (candidates.IndexOf(candid) < 0) candidates.Add(candid);

            if (cnt == 0 && candidates.Count > 0)
            {
                coActive = mono.StartCoroutine(processActive());
            }
        }

        public void unsub(T candid)
        {
            int cnt = candidates.Count;
            candidates.Remove(candid);

            if (cnt > 0 && candidates.Count <= 0)
            {
                mono.StopCoroutine(coActive);
                coActive = null;
            }
        }

        IEnumerator processActive()
        {
            while (true)
            {
                for (int i = 0; i < candidates.Count; i++)
                {
                    candidates[i].update();
                }
            }
        }

    }

    /// <summary>
    /// helper
    /// </summary>
    public class ListUpdaterComponent : MonoBehaviour
    { }
}
