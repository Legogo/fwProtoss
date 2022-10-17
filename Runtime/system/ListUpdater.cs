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

        public ListUpdater(string updateName)
        {
            var carry = new GameObject("[ListUpdater]~" + updateName + "~" + Random.Range(0, 10000));
            mono = carry.AddComponent<ListUpdaterComponent>();
        }
        public ListUpdater(MonoBehaviour carry)
        {
            mono = carry;
        }

        public void sub(T candid)
        {
            if (candidates.IndexOf(candid) < 0) candidates.Add(candid);
            setActive(true);
        }

        public void unsub(T candid)
        {
            candidates.Remove(candid);
            setActive(false);

            //Debug.Log("unsub "+candid);
        }

        public void setActive(bool active)
        {
            if(active && coActive == null)
            {
                coActive = mono.StartCoroutine(processActive());
            }
            else if(!active && coActive != null)
            {
                mono.StopCoroutine(coActive);
                coActive = null;
            }
        }

        IEnumerator processActive()
        {
            Debug.Log(GetType() + " <b>updater started</b> on " + mono, mono);

            while (true)
            {
                //Debug.Log(GetType() + " updating x" + candidates.Count);
                
                for (int i = 0; i < candidates.Count; i++)
                {
                    candidates[i].update();
                }

                yield return null;
            }
        }

    }

    /// <summary>
    /// helper
    /// </summary>
    public class ListUpdaterComponent : MonoBehaviour
    { }
}
